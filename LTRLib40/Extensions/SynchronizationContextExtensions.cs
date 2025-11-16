using System.ComponentModel;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
using System;
using LTRLib.LTRGeneric;


#endif
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Threading;

namespace LTRLib.Extensions;

public static class SynchronizationContextExtensions
{
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static SynchronizationContext? GetSynchronizationContext(this ISynchronizeInvoke owner) =>
        owner.InvokeRequired && owner.Invoke(() => SynchronizationContext.Current, null) is SynchronizationContext context
        ? context : SynchronizationContext.Current;
#endif

    #region ISynchronizeInvoke typed extensions

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

    public static IAsyncResult BeginInvoke(this ISynchronizeInvoke target, Action method) => target.BeginInvoke(method, default);

    public static void QueueInvoke(this ISynchronizeInvoke target, Action method) => RuntimeSupport.QueueInvoke(target.Invoke, method);

    public static void Invoke(this ISynchronizeInvoke target, Action method) => target.Invoke(method, default);

    public static void QueueInvoke<T>(this ISynchronizeInvoke target, Action<T> method, T param) => RuntimeSupport.QueueInvoke(target.Invoke, method, param);

    public static void Invoke<T>(this ISynchronizeInvoke target, Action<T> method, T param) => target.Invoke(method, [param]);

    public static IAsyncResult BeginInvoke<T1, T2>(this ISynchronizeInvoke target, Action<T1, T2> method, T1 param1, T2 param2) => target.BeginInvoke(method, [param1, param2]);

    public static void QueueInvoke<T1, T2>(this ISynchronizeInvoke target, Action<T1, T2> method, T1 param1, T2 param2) => RuntimeSupport.QueueInvoke(target.Invoke, method, param1, param2);

    public static void Invoke<T1, T2>(this ISynchronizeInvoke target, Action<T1, T2> method, T1 param1, T2 param2) => target.Invoke(method, [param1, param2]);

    public static IAsyncResult BeginInvoke<T1, T2, T3>(this ISynchronizeInvoke target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => target.BeginInvoke(method, [param1, param2, param3]);

    public static void QueueInvoke<T1, T2, T3>(this ISynchronizeInvoke target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => RuntimeSupport.QueueInvoke(target.Invoke, method, param1, param2, param3);

    public static void Invoke<T1, T2, T3>(this ISynchronizeInvoke target, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => target.Invoke(method, [param1, param2, param3]);

    public static IAsyncResult BeginInvoke<T1, T2, T3, T4>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.BeginInvoke(method, [param1, param2, param3, param4]);

    public static void Invoke<T1, T2, T3, T4>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.Invoke(method, [param1, param2, param3, param4]);

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

    public static void QueueInvoke<T1, T2, T3, T4>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => RuntimeSupport.QueueInvoke(target.Invoke, method, param1, param2, param3, param4);

    public static void Invoke<T1, T2, T3, T4, T5>(this ISynchronizeInvoke target, Action<T1, T2, T3, T4, T5> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => target.Invoke(method, [param1, param2, param3, param4, param5]);

#endif


    public static IAsyncResult BeginInvoke<TResult>(this ISynchronizeInvoke target, Func<TResult> method) => target.BeginInvoke(method, default);

    public static TResult? Invoke<TResult>(this ISynchronizeInvoke target, Func<TResult> method) => (TResult?)target.Invoke(method, default);

    public static IAsyncResult BeginInvoke<T, TResult>(this ISynchronizeInvoke target, Func<T, TResult> method, T param) => target.BeginInvoke(method, [param]);

    public static TResult? Invoke<T, TResult>(this ISynchronizeInvoke target, Func<T, TResult> method, T param) => (TResult?)target.Invoke(method, [param]);

    public static IAsyncResult BeginInvoke<T1, T2, TResult>(this ISynchronizeInvoke target, Func<T1, T2, TResult> method, T1 param1, T2 param2) => target.BeginInvoke(method, [param1, param2]);

    public static TResult? Invoke<T1, T2, TResult>(this ISynchronizeInvoke target, Func<T1, T2, TResult> method, T1 param1, T2 param2) => (TResult?)target.Invoke(method, [param1, param2]);

    public static IAsyncResult BeginInvoke<T1, T2, T3, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3) => target.BeginInvoke(method, [param1, param2, param3]);

    public static TResult? Invoke<T1, T2, T3, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3) => (TResult?)target.Invoke(method, [param1, param2, param3]);

    public static IAsyncResult BeginInvoke<T1, T2, T3, T4, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4) => target.BeginInvoke(method, [param1, param2, param3, param4]);

    public static TResult? Invoke<T1, T2, T3, T4, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4) => (TResult?)target.Invoke(method, [param1, param2, param3, param4]);

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP3_0_OR_GREATER

    public static IAsyncResult BeginInvoke<T1, T2, T3, T4, T5, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, T5, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => target.BeginInvoke(method, [param1, param2, param3, param4, param5]);

    public static TResult? Invoke<T1, T2, T3, T4, T5, TResult>(this ISynchronizeInvoke target, Func<T1, T2, T3, T4, T5, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => (TResult?)target.Invoke(method, [param1, param2, param3, param4, param5]);

#endif

#endif

    #endregion

    #region SynchronizationContext typed extensions

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static void Post(this SynchronizationContext target, Action method) => target.Post(_ => method(), default);

    public static void QueueInvoke(this SynchronizationContext target, Action method) => RuntimeSupport.QueueInvoke(target.Post, method);

    public static void Send(this SynchronizationContext target, Action method) => target.Send(_ => method(), default);

    public static void Post<T>(this SynchronizationContext target, Action<T> method, T param) => target.Post(o => method(param), null);

    public static void QueueInvoke<T>(this SynchronizationContext target, Action<T> method, T param) => RuntimeSupport.QueueInvoke(target.Post, method, param);

    public static void Send<T>(this SynchronizationContext target, Action<T> method, T param) => target.Send(_ => method(param), null);

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
