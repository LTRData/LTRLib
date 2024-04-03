﻿#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP || NETSTANDARD || NET461_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#endif

namespace LTRLib.Web;

public static class HttpShared
{
    [Flags]
    public enum DynDocFeatures
    {
        None = 0x0,
        DynDoc = 0x1,
        ReloadApp = 0x2,
        Checksum = 0x4,
#if NET47_OR_GREATER || NETSTANDARD || NETCOREAPP
        DenyBotNets = 0x8,
#endif
        Redir = 0x10
    }

    public static string GetMimeType(string requestExt)
    {
        return (requestExt?.ToLowerInvariant()) switch
        {
            ".html" => "text/html",
            ".txt" => "text/plain",
            ".css" => "text/css",
            ".js" => "application/js",
            ".kml" => "application/kml",
            ".kmz" => "application/kmz",
            ".xhtml" => "text/xhtml",
            ".jpg" => "image/jpeg",
            ".gif" => "image/gif",
            ".png" => "image/png",
            _ => "application/octet-stream",
        };
    }

    public static readonly string[] PublicCacheableExtensions =
    [
        ".html",
        ".txt",
        ".css",
        ".js",
        ".kml",
        ".xml",
        ".xhtml",
        ".zip",
        ".7z",
        ".gz",
        ".bz2",
        ".lzma",
        ".exe",
        ".sys",
        ".cpl",
        ".iso",
        ".dll",
        ".pdb",
        ".jpg",
        ".gif",
        ".png"
    ];

    public static readonly string[] EncodableExtensions =
    [
        ".html",
        ".txt",
        ".css",
        ".js",
        ".kml",
        ".xml",
        ".xhtml",
        ".exe",
        ".sys",
        ".cpl",
        ".dll",
        ".pdb"
    ];

    #if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static readonly Dictionary<string, Func<HashAlgorithm>> CheckSums = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".md5", MD5.Create },
        { ".sha1", SHA1.Create },
        { ".sha256", SHA256.Create }
    };
    #endif

    public static string GetEncodedFileExtension(string? encoding)
    {
        return (encoding?.ToLowerInvariant()) switch
        {
            "gzip" => ".gz",
            "deflate" => ".z",
            _ => string.Concat(".", encoding),
        };
    }

    public static string GetDynDocUrl(string UrlScheme, string UrlAuthority, string Path, string Query, string DocName)
        => $"{UrlScheme}{Uri.SchemeDelimiter}{UrlAuthority}/dyndoc{Path}/{Convert.ToBase64String(Encoding.UTF8.GetBytes(Query)).Replace('/', '_')}/{DocName}";

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static async Task<string?> GetHostNameSafeAsync(this IPAddress? address)
    {
        if (address is null)
        {
            return null;
        }

        try
        {
            return (await Dns.GetHostEntryAsync(address).ConfigureAwait(false)).HostName;
        }
        catch
        {
            return address.ToString();
        }
    }
#endif

#if NETCOREAPP || NETSTANDARD || NET461_OR_GREATER

    [Obsolete("Use QueryString methods instead")]
    public static string? Get(this IHeaderDictionary dict, string key) => dict[key].FirstOrDefault();

    public static void AppendHeader(this HttpResponse response, string key, StringValues value) => response.Headers.Append(key, value);

    public static void AppendHeader(this HttpResponse response, string key, DateTime value) => response.Headers.Append(key, value.ToUniversalTime().ToString("R"));

    [Obsolete("Use QueryString methods instead")]
    public static string? GetParam(this HttpRequest request, string key) => request.Query[key].FirstOrDefault();

#endif
}

#endif
