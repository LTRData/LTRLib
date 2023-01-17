#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LTRLib.Net;

public static class DownloadCache
{
    private static readonly HttpClient httpClient = new();

    private static readonly ConcurrentDictionary<string, Task<byte[]>> downloadCache = new();

    public static async Task<byte[]?> DownloadAndCacheDataAsync(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        try
        {
            return await downloadCache.GetOrAdd(str, httpClient.GetByteArrayAsync).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to download image {str}: {ex}");
            downloadCache.TryRemove(str, out _);
            return null;
        }
    }
}

#endif
