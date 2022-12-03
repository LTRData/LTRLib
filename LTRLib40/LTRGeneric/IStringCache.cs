// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LTRLib.LTRGeneric;

public interface IStringCache : ICloneable, IDisposable
{
    Task<int> RemoveItemAsync(string key, CancellationToken cancellationToken);

    Task<int> RemoveAllAsync(CancellationToken cancellationToken);

    Task<int> CleanAsync(CancellationToken cancellationToken);

    [ComVisible(false)]
    Task<int> SetItemAsync(string key, string? data, DateTime? expirydate, CancellationToken cancellationToken);

    Task<string?> GetItemAsync(string key, CancellationToken cancellationToken);
    
    new IStringCache Clone();
}

#endif
