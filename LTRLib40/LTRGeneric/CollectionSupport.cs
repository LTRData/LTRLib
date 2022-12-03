/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif

namespace LTRLib.LTRGeneric;

public static class AsyncSupport
{
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static Task DisposeAsync(params IDisposable[] objects)
    {
        var tasks = Array.ConvertAll(objects, obj => Task.Run(obj.Dispose));

        return Task.WhenAll(tasks);
    }
#endif
}
