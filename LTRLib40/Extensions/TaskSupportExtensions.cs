using System;
using System.Collections.Generic;
using System.ComponentModel;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Text;
using System.Threading;
using System.IO;

namespace LTRLib.Extensions;

public static class TaskSupportExtensions
{
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    private static class Immuatables<T>
    {
        public static readonly Task<T?> DefaultCompletedTask = Task.FromResult<T?>(default);

        public static readonly Task<T[]> EmptyArrayCompletedTask = Task.FromResult(Array.Empty<T>());

        public static readonly Task<IEnumerable<T>> EmptyEnumerationCompletedTask = Task.FromResult(Enumerable.Empty<T>());
    }

    public static Task<T?> DefaultCompletedTask<T>() => Immuatables<T>.DefaultCompletedTask;

    public static Task<T[]> EmptyArrayCompletedTask<T>() => Immuatables<T>.EmptyArrayCompletedTask;

    public static Task<IEnumerable<T>> EmptyEnumerationCompletedTask<T>() => Immuatables<T>.EmptyEnumerationCompletedTask;

    public static IAsyncResult AsAsyncResult<T>(this Task<T> task, AsyncCallback? callback, object? state)
    {
        var returntask = task.ContinueWith((t, _) => t.Result, state, TaskScheduler.Default);

        if (callback is not null)
        {
            returntask.ContinueWith(callback.Invoke, TaskScheduler.Default);
        }

        return returntask;
    }

    public static IAsyncResult AsAsyncResult(this Task task, AsyncCallback? callback, object? state)
    {
        var returntask = task.ContinueWith((t, _) => { }, state, TaskScheduler.Default);

        if (callback is not null)
        {
            returntask.ContinueWith(callback.Invoke, TaskScheduler.Default);
        }

        return returntask;
    }

#if !NET7_0_OR_GREATER
    public static Task<string> ReadLineAsync(this TextReader reader, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return reader.ReadLineAsync();
    }
#endif

#if !NET6_0_OR_GREATER
    public static Task WaitAsync(this Task task, CancellationToken cancellationToken)
        => task.WaitAsync(TimeSpan.FromMilliseconds(-1), cancellationToken);

    public async static Task WaitAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var timeoutTask = Task.Delay(timeout, cancellationToken);
        var result = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
        if (result == timeoutTask)
        {
            throw new TimeoutException();
        }
    }

    public static Task<T> WaitAsync<T>(this Task<T> task, CancellationToken cancellationToken)
        => task.WaitAsync(TimeSpan.FromMilliseconds(-1), cancellationToken);

    public async static Task<T> WaitAsync<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var timeoutTask = Task.Delay(timeout, cancellationToken);
        var result = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
        if (result == timeoutTask)
        {
            throw new TimeoutException();
        }

        return task.Result;
    }
#endif
#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static SynchronizationContext? GetSynchronizationContext(this ISynchronizeInvoke owner) =>
        owner.InvokeRequired && owner.Invoke(() => SynchronizationContext.Current, null) is SynchronizationContext context
        ? context : SynchronizationContext.Current;
#endif
}
