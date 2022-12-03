// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace LTRLib.LTRGeneric;

[ComVisible(false)]
public static class RuntimeSupport
{
    #if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static void TryCall(Action method, Action<Exception> handler)
    {
        try
        {
            method();
        }
        catch (Exception ex)
        {
            handler?.Invoke(ex);
        }
    }

    #endif

    public static void TryCall<T>(Action<T> method, T param, Action<Exception> handler)
    {
        try
        {
            method(param);
        }
        catch (Exception ex)
        {
            handler?.Invoke(ex);
        }
    }

    #if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static void TryCall<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2, Action<Exception> handler)
    {
        try
        {
            method(param1, param2);
        }

        catch (Exception ex)
        {
            handler?.Invoke(ex);
        }
    }

    public static void TryCall<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3, Action<Exception> handler)
    {
        try
        {
            method(param1, param2, param3);
        }

        catch (Exception ex)
        {
            handler?.Invoke(ex);
        }
    }

    public static void TryCall<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4, Action<Exception> handler)
    {
        try
        {
            method(param1, param2, param3, param4);
        }

        catch (Exception ex)
        {
            handler?.Invoke(ex);
        }
    }

    public static TResult? TryCall<TResult>(Func<TResult> method, Func<Exception, TResult> handler)
    {
        try
        {
            return method();
        }
        catch (Exception ex)
        {
            if (handler is null)
            {
                return default;
            }
            else
            {
                return handler(ex);
            }
        }
    }

    public static TResult? TryCall<T, TResult>(Func<T, TResult> method, T param, Func<Exception, TResult> handler)
    {
        try
        {
            return method(param);
        }
        catch (Exception ex)
        {
            if (handler is null)
            {
                return default;
            }
            else
            {
                return handler(ex);
            }
        }
    }

    public static TResult? TryCall<T1, T2, TResult>(Func<T1, T2, TResult> method, T1 param1, T2 param2, Func<Exception, TResult> handler)
    {
        try
        {
            return method(param1, param2);
        }
        catch (Exception ex)
        {
            if (handler is null)
            {
                return default;
            }
            else
            {
                return handler(ex);
            }
        }
    }

    public static TResult? TryCall<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3, Func<Exception, TResult> handler)
    {
        try
        {
            return method(param1, param2, param3);
        }

        catch (Exception ex)
        {
            if (handler is null)
            {
                return default;
            }
            else
            {
                return handler(ex);
            }
        }
    }

    public static TResult? TryCall<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4, Func<Exception, TResult> handler)
    {
        try
        {
            return method(param1, param2, param3, param4);
        }
        catch (Exception ex)
        {
            if (handler is null)
            {
                return default;
            }
            else
            {
                return handler(ex);
            }

        }
    }

    public static void TryCall(Action method)
    {
        try
        {
            method();
        }
        catch
        {
        }
    }

    #endif

    public static void TryCall<T>(Action<T> method, T param)
    {
        try
        {
            method(param);
        }
        catch
        {
        }
    }

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static void TryCall<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2)
    {
        try
        {
            method(param1, param2);
        }
        catch
        {
        }
    }

    public static void TryCall<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3)
    {
        try
        {
            method(param1, param2, param3);
        }
        catch
        {
        }
    }

    public static void TryCall<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        try
        {
            method(param1, param2, param3, param4);
        }
        catch
        {
        }
    }

    public static TResult? TryCall<TResult>(Func<TResult> method)
    {
        try
        {
            return method();
        }
        catch
        {
            return default;
        }
    }

    public static TResult? TryCall<T, TResult>(Func<T, TResult> method, T param)
    {
        try
        {
            return method(param);
        }
        catch
        {
            return default;
        }
    }

    public static TResult? TryCall<T1, T2, TResult>(Func<T1, T2, TResult> method, T1 param1, T2 param2)
    {
        try
        {
            return method(param1, param2);
        }
        catch
        {
            return default;
        }
    }

    public static TResult? TryCall<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method, T1 param1, T2 param2, T3 param3)
    {
        try
        {
            return method(param1, param2, param3);
        }
        catch
        {
            return default;
        }
    }

    public static TResult? TryCall<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        try
        {
            return method(param1, param2, param3, param4);
        }
        catch
        {
            return default;
        }
    }

    public static void QueueInvoke(Action method) => ThreadPool.QueueUserWorkItem(_ => method());

#endif

    public static void QueueInvoke<T>(Action<T> method, T param) => ThreadPool.QueueUserWorkItem(_ => method(param));

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static void QueueInvoke<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2) => ThreadPool.QueueUserWorkItem(_ => method(param1, param2));


    public static void QueueInvoke<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3) => ThreadPool.QueueUserWorkItem(_ => method(param1, param2, param3));


    public static void QueueInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4) => ThreadPool.QueueUserWorkItem(_ => method(param1, param2, param3, param4));

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static void QueueInvoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => ThreadPool.QueueUserWorkItem(_ => method(param1, param2, param3, param4, param5));

#endif

#endif

}

