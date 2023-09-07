// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace LTRLib.LTRGeneric;

[ComVisible(false)]
public static class ExpressionSupport
{
    private class ParameterCompatibilityComparer : IEqualityComparer<Type>
    {
        private ParameterCompatibilityComparer()
        {
        }

        private static readonly ParameterCompatibilityComparer Instance = new();

        public bool Equals(Type? x, Type? y) => ReferenceEquals(x, y) || (x is not null && y is not null && x.IsAssignableFrom(y));

        public int GetHashCode(Type obj) => obj.GetHashCode();

        public static bool Compatible(MethodInfo dest, Type sourceReturnType, Type[] sourceParameters)
        {
            return dest.ReturnType.IsAssignableFrom(sourceReturnType)
                   && dest.GetParameters().Select(dparam => dparam.ParameterType).SequenceEqual(sourceParameters, Instance);
        }
    }

    public static Delegate CreateLocalFallbackFunction(string MethodName, Type[] GenericArguments, IEnumerable<Expression> MethodArguments, Type ReturnType, bool InvertResult, bool RuntimeMethodDetection)
    {
        var staticArgs = (from arg in MethodArguments
                          select arg.NodeType == ExpressionType.Quote ? ((UnaryExpression)arg).Operand : arg).ToArray();

        var staticArgsTypes = Array.ConvertAll(staticArgs, arg => arg.Type);

        var newMethod = GetCompatibleMethod(typeof(Enumerable), true, MethodName, GenericArguments, ReturnType, staticArgsTypes)
            ?? throw new NotSupportedException($"Expression calls unsupported method {MethodName}.");

        // ' Substitute first argument (extension method source object) with a parameter that
        // ' will be substituted with the result sequence when resulting lambda conversion
        // ' routine is called locally after data has been fetched from external data service.
        var sourceObject = Expression.Parameter(newMethod.GetParameters()[0].ParameterType, "source");

        staticArgs[0] = sourceObject;

        Expression newCall = Expression.Call(newMethod, staticArgs);

        if (InvertResult)
        {
            newCall = Expression.Not(newCall);
        }

        var enumerableStaticDelegate = Expression.Lambda(newCall, sourceObject).Compile();

        if (RuntimeMethodDetection)
        {
            var instanceArgs = staticArgs.Skip(1).ToArray();
            var instanceArgsTypes = staticArgsTypes.Skip(1).ToArray();

            var delegateType = enumerableStaticDelegate.GetType();
            var delegateInvokeMethod = delegateType.GetMethod("Invoke")!;

            Expression<Func<object, object>> getDelegateInstanceOrDefault = obj
                => GetCompatibleMethodAsDelegate(obj.GetType(), false, InvertResult, MethodName, GenericArguments, ReturnType, instanceArgsTypes, sourceObject, instanceArgs)
                ?? enumerableStaticDelegate;

            var exprGetDelegateInstanceOrDefault = Expression.Invoke(getDelegateInstanceOrDefault, sourceObject);

            var exprCallDelegateInvokeMethod = Expression.Call(Expression.TypeAs(exprGetDelegateInstanceOrDefault, delegateType), delegateInvokeMethod, sourceObject);

            return Expression.Lambda(exprCallDelegateInvokeMethod, sourceObject).Compile();
        }
        else
        {
            return enumerableStaticDelegate;
        }
    }

    public static Delegate? GetCompatibleMethodAsDelegate(Type TypeToSearch, bool FindStaticMethod, bool InvertResult, string MethodName, Type[] GenericArguments, Type ReturnType, Type[] AlternateArgsTypes, ParameterExpression Instance, Expression[] Args)
    {
        var dynMethod = GetCompatibleMethod(TypeToSearch, FindStaticMethod, MethodName, GenericArguments, ReturnType, AlternateArgsTypes);

        if (dynMethod is null)
        {
            return null;
        }

        var callExpr = FindStaticMethod
            ? Expression.Call(dynMethod, Args)
            : (Expression)Expression.Call(Expression.TypeAs(Instance, dynMethod.DeclaringType!), dynMethod, Args);
        if (InvertResult)
        {
            callExpr = Expression.Not(callExpr);
        }

        return Expression.Lambda(callExpr, Instance).Compile();
    }

    private static readonly Dictionary<string, MethodInfo?> _GetCompatibleMethod_methodCache = new();

    public static MethodInfo? GetCompatibleMethod(Type TypeToSearch, bool FindStaticMethod, string MethodName, Type[] GenericArguments, Type ReturnType, Type[] AlternateArgsTypes)
    {
        MethodInfo? newMethod = null;

        var key = $"{ReturnType}:{MethodName}:{string.Join(":", Array.ConvertAll(AlternateArgsTypes, argType => argType.ToString()))}";

        lock (_GetCompatibleMethod_methodCache)
        {
            if (!_GetCompatibleMethod_methodCache.TryGetValue(key, out newMethod))
            {
                var methodNames = new[] { MethodName, $"get_{MethodName}" };

                newMethod = (from m in TypeToSearch.GetMethods(BindingFlags.Public | (FindStaticMethod ? BindingFlags.Static : BindingFlags.Instance))
                             where methodNames.Contains(m.Name) && m.GetParameters().Length == AlternateArgsTypes.Length && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == GenericArguments.Length
                             select m.MakeGenericMethod(GenericArguments)).FirstOrDefault(m => ParameterCompatibilityComparer.Compatible(m, ReturnType, AlternateArgsTypes));

                if (newMethod is null && !FindStaticMethod)
                {
                    foreach (var interf in from i in TypeToSearch.GetInterfaces()
                                           where i.IsGenericType && i.GetGenericArguments().Length == GenericArguments.Length
                                           select i)
                    {
                        newMethod = interf.GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => methodNames.Contains(m.Name) && ParameterCompatibilityComparer.Compatible(m, ReturnType, AlternateArgsTypes));

                        if (newMethod is not null)
                        {
                            break;
                        }
                    }
                }

                _GetCompatibleMethod_methodCache.Add(key, newMethod);
            }
        }

        return newMethod;
    }

    private static readonly Dictionary<Type, Type[]> _GetListItemsType_listItemsTypes = new();

    public static Type GetListItemsType(Type Type)
    {
        while (Type.HasElementType)
        {
            Type = Type.GetElementType()!;
        }

        Type[]? i = null;

        lock (_GetListItemsType_listItemsTypes)
        {
            if (!_GetListItemsType_listItemsTypes.TryGetValue(Type, out i))
            {
                i = (from ifc in Type.GetInterfaces()
                     where ifc.IsGenericType && ifc.GetGenericTypeDefinition() == typeof(IList<>)
                     select ifc.GetGenericArguments()[0]).ToArray();

                _GetListItemsType_listItemsTypes.Add(Type, i);
            }
        }

        if (i.Length == 0)
        {
            return Type;
        }
        else if (i.Length == 1)
        {
            return i[0];
        }

        throw new NotSupportedException($"More than one element type detected for list type {Type}.");
    }

    #if NETCOREAPP3_0_OR_GREATER || !NETCOREAPP

    private sealed class ExpressionMemberEqualityComparer : IEqualityComparer<MemberInfo>
    {
        public bool Equals(MemberInfo? x, MemberInfo? y)
            => ReferenceEquals(x, y)
                || ReferenceEquals(x?.DeclaringType, y?.DeclaringType)
                && x?.MetadataToken == y?.MetadataToken;

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
        public int GetHashCode(MemberInfo obj) => HashCode.Combine(obj.DeclaringType?.MetadataToken, obj.MetadataToken);
#else
        public int GetHashCode(MemberInfo obj) => (obj.DeclaringType?.MetadataToken ?? 0) ^ obj.MetadataToken;
#endif
    }

    public static Dictionary<IEnumerable<MemberInfo>, string> CreateMemberToFieldDictionary()
        => new(new SequenceEqualityComparer<MemberInfo>(new ExpressionMemberEqualityComparer()));

    private static readonly Dictionary<Type, Dictionary<IEnumerable<MemberInfo>, string>> _GetDataFieldMappings_dataMappings = new();

    public static Dictionary<IEnumerable<MemberInfo>, string> GetDataFieldMappings(Type ElementType)
    {

        Dictionary<IEnumerable<MemberInfo>, string>? mappings = null;

        lock (_GetDataFieldMappings_dataMappings)
        {

            if (!_GetDataFieldMappings_dataMappings.TryGetValue(ElementType, out mappings))
            {

                mappings = CreateMemberToFieldDictionary();

                mappings.AddRange(from prop in ElementType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                                  where prop.MemberType == MemberTypes.Property
                                        && ((PropertyInfo)prop).GetIndexParameters().Length == 0
                                        && ((PropertyInfo)prop).CanRead
                                        && ((PropertyInfo)prop).CanWrite
                                        || prop.MemberType == MemberTypes.Field
                                        && !((FieldInfo)prop).IsInitOnly
                                  select new KeyValuePair<IEnumerable<MemberInfo>, string>(new[] { prop }, prop.Name));

                var submappings = from props in mappings.Keys.ToArray()
                                  let prop = props.First()
                                  where prop.MemberType == MemberTypes.Property
                                        && ((PropertyInfo)prop).GetIndexParameters().Length == 0
                                        && ((PropertyInfo)prop).CanRead
                                        && ((PropertyInfo)prop).CanWrite
                                        || prop.MemberType == MemberTypes.Field
                                        && !((FieldInfo)prop).IsInitOnly
                                  let type = GetListItemsType(prop.MemberType == MemberTypes.Property ? ((PropertyInfo)prop).PropertyType : ((FieldInfo)prop).FieldType)
                                  where !type.IsPrimitive && !ReferenceEquals(type, typeof(string))
                                  from submapping in GetDataFieldMappings(type)
                                  select new KeyValuePair<IEnumerable<MemberInfo>, string>(submapping.Key.Concat(props), $"{prop.Name}.{submapping.Value}");

                mappings.AddRange(submappings);

                _GetDataFieldMappings_dataMappings.Add(ElementType, mappings);
            }
        }

        return mappings;
    }

    public static ReadOnlyCollection<string> GetPropertiesWithAttributes<TAttribute>(Type type) where TAttribute : Attribute
        => AttributedMemberFinder<TAttribute>.GetPropertiesWithAttributes(type);

    private static readonly Dictionary<Type, ReadOnlyCollection<string>> _GetPropertiesWithAttributes_cache = new();

    private class AttributedMemberFinder<TAttribute> where TAttribute : Attribute
    {
        public static ReadOnlyCollection<string> GetPropertiesWithAttributes(Type type)
        {
            ReadOnlyCollection<string>? prop = null;

            lock (_GetPropertiesWithAttributes_cache)
            {

                if (!_GetPropertiesWithAttributes_cache.TryGetValue(type, out prop))
                {
                    prop = Array.AsReadOnly((from p in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                                             where Attribute.IsDefined(p, typeof(TAttribute))
                                             select p.Name).ToArray());

                    _GetPropertiesWithAttributes_cache.Add(type, prop);
                }

            }

            return prop;
        }
    }

#endif

    public static string GetDataTableName<TContext>(Type entityType) => DataContextPropertyFinder<TContext>.GetDataTableName(entityType);

    private static readonly Dictionary<Type, string> _GetDataTableName_properties = new();

    private class DataContextPropertyFinder<TContext>
    {
        public static string GetDataTableName(Type entityType)
        {
            PropertyInfo itemsProperty = typeof(TContext).GetProperty("Items")
                ?? throw new MissingMemberException("Property Items not found");

            string? prop = null;

            lock (_GetDataTableName_properties)
            {
                if (!_GetDataTableName_properties.TryGetValue(entityType, out prop))
                {
                    prop = itemsProperty
                        .GetCustomAttributes(true)
                        .OfType<XmlElementAttribute>()
                        .Single(attr => attr.Type is not null && attr.Type.IsAssignableFrom(entityType)).ElementName;

                    _GetDataTableName_properties.Add(entityType, prop);
                }
            }

            return prop;
        }
    }

    public static Expression GetLambdaBody(Expression expression) => GetLambdaBody(expression, out _);

    public static Expression GetLambdaBody(Expression expression, out ReadOnlyCollection<ParameterExpression>? parameters)
    {
        if (expression.NodeType != ExpressionType.Quote)
        {
            parameters = null;
            return expression;
        }

        var expr = (LambdaExpression)((UnaryExpression)expression).Operand;

        parameters = expr.Parameters;

        return expr.Body;
    }

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
    internal class PropertiesAssigners<T>
    {
        public static Dictionary<string, Func<T, object?>> Getters { get; }

        public static Dictionary<string, Action<T, object?>> Setters { get; }

        public static Dictionary<string, Type> Types { get; }

        static PropertiesAssigners()
        {
            var target = Expression.Parameter(typeof(T), "targetObject");

            var props = (from m in typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance)
                         let p = m as PropertyInfo
                         let f = m as FieldInfo
                         where p is not null && p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 || f is not null && !f.IsInitOnly
                         let proptype = p is not null ? p.PropertyType : f.FieldType
                         let name = m.Name
                         let member = Expression.PropertyOrField(target, m.Name)
                         select new { name, proptype, member }).ToArray();

            var getters = from m in props
                          select new {
                              m.name,
                              valueconverted = m.proptype.IsValueType ? Expression.Convert(m.member, typeof(object)) : Expression.TypeAs(m.member, typeof(object))
                          };

            Getters = getters.ToDictionary(m => m.name,
                m => Expression.Lambda<Func<T, object?>>(m.valueconverted, target).Compile(),
                StringComparer.OrdinalIgnoreCase);

            var setters = from m in props
                          let value = Expression.Parameter(typeof(object), "value")
                          let valueconverted = m.proptype.IsValueType
                            ? (Expression)Expression.ConvertChecked(value, m.proptype)
                            : Expression.Condition(Expression.TypeIs(value, m.proptype), Expression.TypeAs(value, m.proptype), Expression.ConvertChecked(value, m.proptype))
                          select new {
                              m.name,
                              assign = Expression.Assign(m.member, valueconverted),
                              value
                          };

            Setters = setters.ToDictionary(m => m.name,
                m => Expression.Lambda<Action<T, object?>>(m.assign, target, m.value).Compile(),
                StringComparer.OrdinalIgnoreCase);

            Types = props.ToDictionary(m => m.name,
                m => m.proptype,
                StringComparer.OrdinalIgnoreCase);
        }
    }

    public static Dictionary<string, Func<T, object?>> GetPropertyGetters<T>() where T : new() => new(PropertiesAssigners<T>.Getters);

    public static Dictionary<string, Action<T, object?>> GetPropertySetters<T>() where T : new() => new(PropertiesAssigners<T>.Setters);

    public static Dictionary<string, Type> GetPropertyTypes<T>() where T : new() => new(PropertiesAssigners<T>.Types);
#endif

}

#endif
