#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using System.Threading.Tasks;

namespace LTRLib.Extensions;

public static class AsyncExtensions
{
    
#if NET462_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Waits for a ValueTask to complete, or throws AggregateException if the ValueTask fails
    /// </summary>
    /// <param name="task">ValueTask</param>
    public static void Wait(this ValueTask task)
    {
        if (!task.IsCompletedSuccessfully)
        {
            task.AsTask().Wait();
        }
    }

    /// <summary>
    /// Waits for a ValueTask to complete and returns result value, or throws AggregateException if the ValueTask fails
    /// </summary>
    /// <param name="task">ValueTask</param>
    public static void Wait<T>(this ValueTask<T> task)
    {
        if (!task.IsCompletedSuccessfully)
        {
            task.AsTask().Wait();
        }
    }

    /// <summary>
    /// Waits for a ValueTask to complete and returns result value, or throws AggregateException if the ValueTask fails
    /// </summary>
    /// <param name="task">ValueTask</param>
    public static T WaitForResult<T>(this ValueTask<T> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }
        else
        {
            return task.AsTask().Result;
        }
    }

#endif

}

#endif
