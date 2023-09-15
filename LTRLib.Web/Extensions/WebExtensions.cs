#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LTRLib.Extensions;

public static class WebExtensions
{
    public static IApplicationBuilder UseApplicationFeatures(this IApplicationBuilder app, IFileProvider FileProvider, HttpShared.DynDocFeatures Features)
    {
        return app.Use(async (context, next) =>
        {
            if (await WebCore.HttpServerSupport.ApplicationAddFeatures(context, context.Request, context.Response, FileProvider, Features).ConfigureAwait(continueOnCapturedContext: false))
            {
                return;
            }

            await next().ConfigureAwait(continueOnCapturedContext: false);
        });
    }

#if NETFRAMEWORK

    public static string Get(this NameValueCollection dict, string key) => dict[key];

    public static void AppendHeader(this System.Web.HttpResponse response, string key, string value) => response.Headers.Add(key, value);

    public static void AppendHeader(this System.Web.HttpResponse response, string key, DateTime value) => response.Headers.Add(key, value.ToUniversalTime().ToString("R"));

#endif

#if !NET5_0_OR_GREATER
    public static Task<Stream> GetStreamAsync(this HttpClient httpClient, string uri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return httpClient.GetStreamAsync(uri);
    }

    public static Task<Stream> GetStreamAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return httpClient.GetStreamAsync(uri);
    }

    public static Task<byte[]> ReadAsByteArrayAsync(this HttpContent httpClient, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return httpClient.ReadAsByteArrayAsync();
    }

    public static Task<Stream> ReadAsStreamAsync(this HttpContent httpClient, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return httpClient.ReadAsStreamAsync();
    }

    public static Task<string> ReadAsStringAsync(this HttpContent httpClient, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return httpClient.ReadAsStringAsync();
    }
#endif
}

#endif
