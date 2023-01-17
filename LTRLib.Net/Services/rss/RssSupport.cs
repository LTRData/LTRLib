#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.LTRGeneric;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LTRLib.Services.rss;

public static class RssSupport
{
    private static readonly HttpClient httpClient = new();

    public static async Task<rss?> DownloadAsync(string url)
    {
        return XmlSupport.XmlDeserialize<rss>(await httpClient.GetByteArrayAsync(url).ConfigureAwait(false));
    }

    public static async Task<rss?> DownloadAsync(Uri url)
    {
        return XmlSupport.XmlDeserialize<rss>(await httpClient.GetByteArrayAsync(url).ConfigureAwait(false));
    }
}

#endif

