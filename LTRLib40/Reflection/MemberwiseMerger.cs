// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using ArraySupport = System.Array;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Linq;

namespace LTRLib.Reflection;

[ComVisible(false)]
public static class MemberwiseMerger<T> where T : class
{
    static MemberwiseMerger()
    {
        FieldsAccessors = ArraySupport.AsReadOnly(ArraySupport.ConvertAll(typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), FieldAccessors.CreateFromFieldInfo));
    }

    protected internal class FieldAccessors
    {
        public FieldAccessors(FieldInfo member)
        {
            var param_source_object = Expression.Parameter(typeof(T), "source_object");
            var param_target_object = Expression.Parameter(typeof(T), "target_object");
            var source_field = Expression.Field(param_source_object, member);
            var target_field = Expression.Field(param_target_object, member);
            var target_field_is_null_or_default = member.FieldType.IsValueType ? Expression.Equal(target_field, Expression.Default(member.FieldType)) : Expression.ReferenceEqual(Expression.TypeAs(target_field, typeof(object)), Expression.Constant(null));
            var copy_source_to_target = Expression.Assign(target_field, source_field);
            var target_field_is_null_or_default_lambda = Expression.Lambda<Func<T, bool>>(target_field_is_null_or_default, param_target_object);
            var copy_source_to_target_lambda = Expression.Lambda<Action<T, T>>(copy_source_to_target, param_source_object, param_target_object);

            FieldName = member.Name;
            HasDefaultValue = target_field_is_null_or_default_lambda.Compile();
            CopyValue = copy_source_to_target_lambda.Compile();
        }

        public static FieldAccessors CreateFromFieldInfo(FieldInfo member)
        {
            return new FieldAccessors(member);
        }

        public readonly string FieldName;

        public readonly Func<T, bool> HasDefaultValue;

        public readonly Action<T, T> CopyValue;
    }

    internal static readonly ReadOnlyCollection<FieldAccessors> FieldsAccessors;

    public static T? MergeSequence(params T[] sequence)
    {
        return MergeSequence((IEnumerable<T>)sequence);
    }

    public static T? MergeSequence(IEnumerable<T> sequence)
    {
        using var enumerator = sequence.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return null;
        }

        var target = enumerator.Current;

        var remainingfields = (from fld in FieldsAccessors
                               where fld.HasDefaultValue(target)
                               select fld).ToList();

        while (remainingfields.Count > 0 && enumerator.MoveNext())
        {
            var current = enumerator.Current;

            foreach (var newfield in (from fld in remainingfields
                                      where !fld.HasDefaultValue(current)
                                      select fld).ToArray())
            {
                newfield.CopyValue(current, target);

                remainingfields.Remove(newfield);
            }
        }

        return target;
    }
}

#endif
