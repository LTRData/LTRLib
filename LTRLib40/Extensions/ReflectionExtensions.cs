using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Security;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif

#if !NET5_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit
    {
    }
}

#endif

#if NETFRAMEWORK && !NET35_OR_GREATER

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
    }
}

namespace System
{
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);

    public delegate T2 Func<in T1, out T2>(T1 arg1);
}

#endif

#if NETFRAMEWORK || NETSTANDARD

namespace System.Runtime.Versioning
{
    // 
    // Summary:
    // Indicates that an API is supported for a specified platform or operating system.
    // If a version is specified, the API cannot be called from an earlier version.
    // Multiple attributes can be applied to indicate support on multiple operating
    // systems.
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedOSPlatformAttribute : Attribute
    {

        public string PlatformName { get; set; }

        public SupportedOSPlatformAttribute(string platformName)
        {
            PlatformName = platformName;
        }
    }
}

#endif

#if NET35_OR_GREATER && !NET45_OR_GREATER

namespace System.Runtime.CompilerServices
{

    // 
    // Summary:
    // Represents an operation that schedules continuations when it completes.
    public partial interface ICriticalNotifyCompletion
    {
        // 
        // Summary:
        // Schedules the continuation action that's invoked when the instance completes.
        // 
        // Parameters:
        // continuation:
        // The action to invoke when the operation completes.
        // 
        // Exceptions:
        // T:System.ArgumentNullException:
        // The continuation argument is null (Nothing in Visual Basic).
        void OnCompleted(Action continuation);

        // 
        // Summary:
        // Schedules the continuation action that's invoked when the instance completes.
        // 
        // Parameters:
        // continuation:
        // The action to invoke when the operation completes.
        // 
        // Exceptions:
        // T:System.ArgumentNullException:
        // The continuation argument is null (Nothing in Visual Basic).
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }

}

#endif

namespace LTRLib.Extensions
{

    public static class ReflectionExtensions
    {
#if NETFRAMEWORK && !NET46_OR_GREATER

    private sealed class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }

    public static T[] Empty<T>() => EmptyArray<T>.Value;

#else

        public static T[] Empty<T>() => Array.Empty<T>();

#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

        /// <summary>
        /// Returns an IEnumerable(Of MethodInfo) object that enumerates static methods in an
        /// assembly.
        /// </summary>
        /// <param name="Assembly">Assembly to search</param>
        /// <param name="Name">Name of static method to search for</param>
        public static IEnumerable<MethodInfo> GetStaticMethods(this Assembly Assembly, string Name)
        {
            return from TypeDefinition in Assembly.GetTypes()
                   let FoundMethod = TypeDefinition.GetMethod(Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                   where FoundMethod is not null
                   select FoundMethod;
        }

        /// <summary>
        /// Returns an IEnumerable(Of MethodInfo) object that enumerates static methods in an
        /// assembly.
        /// </summary>
        /// <param name="Assembly">Assembly to search</param>
        /// <param name="Name">Name of static method to search for</param>
        /// <param name="ParameterTypes">Types of parameters that searched method accepts</param>
        /// <param name="ReturnType">Return type from searched method</param>
        public static IEnumerable<MethodInfo> GetStaticMethods(this Assembly Assembly, string Name, Type[] ParameterTypes, Type ReturnType)
        {
            return from TypeDefinition in Assembly.GetTypes()
                   let FoundMethod = TypeDefinition.GetMethod(Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, ParameterTypes, null)
                   where FoundMethod is not null && ReferenceEquals(FoundMethod.ReturnParameter.ParameterType, ReturnType)
                   select FoundMethod;
        }

#endif

        /// <summary>
        /// Returns a custom attribute object for a type or member.
        /// </summary>
        /// <typeparam name="T">Type of custom attribute to find</typeparam>
        /// <param name="MemberInfo">Type or member to search</param>
        /// <param name="Inherit">Search inherited custom attributes</param>
        /// <returns>Returns custom attribute object if found, otherwise Nothing is returned.</returns>
        public static T? GetCustomAttribute<T>(this MemberInfo MemberInfo, bool Inherit) where T : Attribute
        {
            var attribs = MemberInfo.GetCustomAttributes(typeof(T), Inherit);

            if (attribs is null || attribs.Length == 0)
            {
                return null;
            }

            return (T)attribs[0];
        }

        /// <summary>
        /// Returns an array of custom attribute objects for a type or member.
        /// </summary>
        /// <typeparam name="T">Type of custom attribute to find</typeparam>
        /// <param name="MemberInfo">Type or member to search</param>
        /// <param name="inherit">Search inherited custom attributes</param>
        public static T[] GetCustomAttributes<T>(this MemberInfo MemberInfo, bool inherit) where T : Attribute => (T[])MemberInfo.GetCustomAttributes(typeof(T), inherit);

        /// <summary>
        /// Returns the value of any EnumDescriptionAttribute object associated with
        /// an enumeration value
        /// </summary>
        /// <returns>Returns Description field of EnumDescriptionAttribute object if found,
        /// otherwise name of enumeration member is returned</returns>
        public static string GetEnumDescription(this Enum enumVar)
        {
            var enumMembers = enumVar.GetType().GetMember(enumVar.ToString());
            if (enumMembers.Length != 1)
            {
                return enumVar.ToString();
            }

            var AttrArray = enumMembers[0].GetCustomAttributes<EnumDescriptionAttribute>(false);

            if (AttrArray.Length == 0)
            {
                return enumVar.ToString();
            }
            else
            {
                return AttrArray[0].Description;
            }
        }

        /// <summary>
        /// Returns reference to object.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Reference to object</returns>
        public static T Me<T>(this T obj) where T : class => obj;

        /// <summary>
        /// Returns referenced string if not Nothing, otherwise a reference to an empty string is
        /// returned.
        /// </summary>
        public static string Nz(this string? s) => s ?? string.Empty;

        /// <summary>
        /// Returns reference to array if not Nothing, otherwise a new empty instance of array is
        /// created and returned.
        /// </summary>
        /// <typeparam name="T">Type of elements in array</typeparam>
        /// <param name="o">Array reference variable to check for Null reference</param>
        public static T[] Nz<T>(this T[]? o) => o ?? Empty<T>();

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

        /// <summary>
        /// Returns reference to enumerable if not Nothing, otherwise a new empty enumeration is
        /// returned.
        /// </summary>
        /// <typeparam name="T">Type of elements in enumeration</typeparam>
        /// <param name="o">IEnumerable reference variable to check for Null reference</param>
        public static IEnumerable<T> Nz<T>(this IEnumerable<T>? o) => o ?? Enumerable.Empty<T>();

#endif

        /// <summary>
        /// Returns reference to object if not Nothing, otherwise a new instance of type is created and
        /// returned.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="o">Reference variable to check for Null reference</param>
        public static T Nz<T>(this T? o) where T : class, new() => o ?? new T();

        /// <summary>
        /// Returns reference to object if not Nothing, otherwise value of defaultvalue parameter is
        /// returned.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="o">Reference variable to check for Null reference</param>
        /// <param name="defaultvalue">A default instance to return if object reference is Nothing</param>
        public static T Nz<T>(this T? o, T defaultvalue) where T : class => o ?? defaultvalue;

        /// <summary>
        /// Clones ICloneable object and returns clone cast to same type as source variable.
        /// </summary>
        /// <typeparam name="T">Type of source variable</typeparam>
        /// <param name="obj">Source object to clone</param>
        public static T CreateTypedClone<T>(this T obj) where T : ICloneable => (T)obj.Clone();

        #region ISynchronizeInvoke typed extensions

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

        public static IAsyncResult BeginInvoke(this ISynchronizeInvoke target, Action method) => target.BeginInvoke(method, default);

        public static void QueueInvoke(this ISynchronizeInvoke target, Action method) => RuntimeSupport.QueueInvoke(target.Invoke, method);

        public static void Invoke(this ISynchronizeInvoke target, Action method) => target.Invoke(method, default);

#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

        public static void QueueInvoke<T>(this ISynchronizeInvoke target, Action<T> method, T param) => RuntimeSupport.QueueInvoke(target.Invoke, method, param);

#endif

        public static void Invoke<T>(this ISynchronizeInvoke target, Action<T> method, T param) => target.Invoke(method, new object?[] { param });

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

        public static IAsyncResult BeginInvoke<T1, T2>(this ISynchronizeInvoke target, Action<T1, T2> method, T1 param1, T2 param2) => target.BeginInvoke(method, new object?[] { param1, param2 });

        public static void QueueInvoke<T1, T2>(this ISynchronizeInvoke target, Action<T1, T2> method, T1 param1, T2 param2) => RuntimeSupport.QueueInvoke(target.Invoke, method, param1, param2);

        public static void Invoke<T1, T2>(this ISynchronizeInvoke target, Action<T1, T2> method, T1 param1, T2 param2) => target.Invoke(method, new object?[] { param1, param2 });

        public static IAsyncResult BeginInvoke<T1, T2, T3>(this ISynchronizeInvoke target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => target.BeginInvoke(method, new object?[] { param1, param2, param3 });

        public static void QueueInvoke<T1, T2, T3>(this ISynchronizeInvoke target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => RuntimeSupport.QueueInvoke(target.Invoke, method, param1, param2, param3);

        public static void Invoke<T1, T2, T3>(this ISynchronizeInvoke target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => target.Invoke(method, new object?[] { param1, param2, param3 });

        public static IAsyncResult BeginInvoke<T1, T2, T3, T4>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.BeginInvoke(method, new object?[] { param1, param2, param3, param4 });

        public static void Invoke<T1, T2, T3, T4>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.Invoke(method, new object?[] { param1, param2, param3, param4 });

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

        public static void QueueInvoke<T1, T2, T3, T4>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => RuntimeSupport.QueueInvoke(target.Invoke, method, param1, param2, param3, param4);

        public static void Invoke<T1, T2, T3, T4, T5>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4, T5> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => target.Invoke(method, new object?[] { param1, param2, param3, param4, param5 });

#endif


        public static IAsyncResult BeginInvoke<TResult>(this ISynchronizeInvoke target, Func<TResult> method) => target.BeginInvoke(method, default);

        public static TResult? Invoke<TResult>(this ISynchronizeInvoke target, Func<TResult> method) => (TResult?)target.Invoke(method, default);

        public static IAsyncResult BeginInvoke<T, TResult>(this ISynchronizeInvoke target, Func<T, TResult> method, T param) => target.BeginInvoke(method, new object?[] { param });

        public static TResult? Invoke<T, TResult>(this ISynchronizeInvoke target, Func<T, TResult> method, T param) => (TResult?)target.Invoke(method, new object?[] { param });

        public static IAsyncResult BeginInvoke<T1, T2, TResult>(this ISynchronizeInvoke target, Func<T1, T2, TResult> method, T1 param1, T2 param2) => target.BeginInvoke(method, new object?[] { param1, param2 });

        public static TResult? Invoke<T1, T2, TResult>(this ISynchronizeInvoke target, Func<T1, T2, TResult> method, T1 param1, T2 param2) => (TResult?)target.Invoke(method, new object?[] { param1, param2 });

        public static IAsyncResult BeginInvoke<T1, T2, T3, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3) => target.BeginInvoke(method, new object?[] { param1, param2, param3 });

        public static TResult? Invoke<T1, T2, T3, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3) => (TResult?)target.Invoke(method, new object?[] { param1, param2, param3 });

        public static IAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.BeginInvoke(method, new object?[] { param1, param2, param3, param4 });

        public static TResult? Invoke<T1, T2, T3, T4, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4) => (TResult?)target.Invoke(method, new object?[] { param1, param2, param3, param4 });

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

        public static IAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, T5, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => target.BeginInvoke(method, new object?[] { param1, param2, param3, param4, param5 });

        public static TResult? Invoke<T1, T2, T3, T4, T5, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, T5, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => (TResult?)target.Invoke(method, new object?[] { param1, param2, param3, param4, param5 });

#endif

#endif

        #endregion

        #region SynchronizationContext typed extensions

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

        public static void Post(this SynchronizationContext target, Action method) => target.Post(_ => method(), default);

        public static void QueueInvoke(this SynchronizationContext target, Action method) => RuntimeSupport.QueueInvoke(target.Post, method);

        public static void Send(this SynchronizationContext target, Action method) => target.Send(_ => method(), default);

#endif

        public static void Post<T>(this SynchronizationContext target, Action<T> method, T param) => target.Post(o => method(param), null);

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

        public static void QueueInvoke<T>(this SynchronizationContext target, Action<T> method, T param) => RuntimeSupport.QueueInvoke(target.Post, method, param);

#endif

        public static void Send<T>(this SynchronizationContext target, Action<T> method, T param) => target.Send(_ => method(param), null);

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

        public static void Post<T1, T2>(this SynchronizationContext target, Action<T1, T2> method, T1 param1, T2 param2) => target.Post(_ => method(param1, param2), default);

        public static void QueueInvoke<T1, T2>(this SynchronizationContext target, Action<T1, T2> method, T1 param1, T2 param2) => RuntimeSupport.QueueInvoke(target.Post, method, param1, param2);

        public static void Send<T1, T2>(this SynchronizationContext target, Action<T1, T2> method, T1 param1, T2 param2) => target.Send(_ => method(param1, param2), default);

        public static void Post<T1, T2, T3>(this SynchronizationContext target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => target.Post(_ => method(param1, param2, param3), default);

        public static void QueueInvoke<T1, T2, T3>(this SynchronizationContext target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => RuntimeSupport.QueueInvoke(target.Post, method, param1, param2, param3);

        public static void Send<T1, T2, T3>(this SynchronizationContext target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => target.Send(_ => method(param1, param2, param3), default);

        public static void Post<T1, T2, T3, T4>(this SynchronizationContext target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.Post(_ => method(param1, param2, param3, param4), default);

        public static void Send<T1, T2, T3, T4>(this SynchronizationContext target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.Send(_ => method(param1, param2, param3, param4), default);

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

        public static void QueueInvoke<T1, T2, T3, T4>(this SynchronizationContext target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => RuntimeSupport.QueueInvoke(target.Post, method, param1, param2, param3, param4);

        public static void Send<T1, T2, T3, T4, T5>(this SynchronizationContext target, Action<T1, T2, T3, T4, T5> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => target.Send(_ => method(param1, param2, param3, param4, param5), default);

#endif


        public static void Post<TResult>(this SynchronizationContext target, Func<TResult> method) => target.Post(_ => method(), default);

        public static TResult? Send<TResult>(this SynchronizationContext target, Func<TResult> method)
        {
            var result = default(TResult);
            target.Send(_ => result = method(), default);
            return result;
        }

        public static void Post<T, TResult>(this SynchronizationContext target, Func<T, TResult> method, T param) => target.Post(_ => method(param), default);

        public static TResult? Send<T, TResult>(this SynchronizationContext target, Func<T, TResult> method, T param)
        {
            var result = default(TResult);
            target.Send(_ => result = method(param), default);
            return result;
        }

        public static void Post<T1, T2, TResult>(this SynchronizationContext target, Func<T1, T2, TResult> method, T1 param1, T2 param2) => target.Post(_ => method(param1, param2), default);

        public static TResult? Send<T1, T2, TResult>(this SynchronizationContext target, Func<T1, T2, TResult> method, T1 param1, T2 param2)
        {
            var result = default(TResult);
            target.Send(_ => result = method(param1, param2), default);
            return result;
        }

        public static void Post<T1, T2, T3, TResult>(this SynchronizationContext target, Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3) => target.Post(_ => method(param1, param2, param3), default);

        public static TResult? Send<T1, T2, T3, TResult>(this SynchronizationContext target, Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3)
        {
            var result = default(TResult);
            target.Send(_ => result = method(param1, param2, param3), default);
            return result;
        }

        public static void Post<T1, T2, T3, T4, TResult>(this SynchronizationContext target, Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.Post(_ => method(param1, param2, param3, param4), default);

        public static TResult? Send<T1, T2, T3, T4, TResult>(this SynchronizationContext target, Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var result = default(TResult);
            target.Send(_ => result = method(param1, param2, param3, param4), default);
            return result;
        }

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

        public static void Post<T1, T2, T3, T4, T5, TResult>(this SynchronizationContext target, Func<T1, T2, T3, T4, T5, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => target.Post(_ => method(param1, param2, param3, param4, param5), default);

        public static TResult? Send<T1, T2, T3, T4, T5, TResult>(this SynchronizationContext target, Func<T1, T2, T3, T4, T5, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var result = default(TResult);
            target.Send(_ => result = method(param1, param2, param3, param4, param5), default);
            return result;
        }

#endif

#endif

        #endregion

    }
}

