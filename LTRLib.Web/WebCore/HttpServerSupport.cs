
// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

#if NETCOREAPP || NETSTANDARD || NET461_OR_GREATER

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using LTRLib.Extensions;
using LTRLib.IO;
using LTRLib.LTRGeneric;
using LTRLib.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.FileProviders;
using static LTRLib.Web.HttpShared;
using static LTRData.Extensions.NetExtensions;
using Microsoft.AspNetCore.Http.Extensions;

using HttpUtility = System.Web.HttpUtility;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using LTRData.Extensions.Split;
using LTRData.Extensions.Formatting;
using LTRData.Extensions.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
using System.Threading;
using LTRData.Extensions.IO;
using LTRData.Extensions.Async;

namespace LTRLib.WebCore;


[ComVisible(false)]
[XmlType("WebCoreHttpServerSupport")]
public static class HttpServerSupport
{
    private static readonly string[] _compressionHeaderIndicators =
    [
        "gzip",
        "deflate"
    ];

    public static string GetRequestCompressionEncoding(this HttpRequest request)
    {
#if NET6_0_OR_GREATER
        var acceptEncoding = request.Headers.AcceptEncoding.FirstOrDefault()
            ?? request.Headers.TransferEncoding.FirstOrDefault()
            ?? request.Headers.TE.FirstOrDefault();
#else
        var acceptEncoding = request.Headers["Accept-Encoding"].FirstOrDefault()
            ?? request.Headers["Transfer-Encoding"].FirstOrDefault()
            ?? request.Headers["TE"].FirstOrDefault();
#endif

        if (acceptEncoding is not null && !string.IsNullOrWhiteSpace(acceptEncoding))
        {
            var encodings = acceptEncoding
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(aenc => _compressionHeaderIndicators.Any(ienc => ienc.Equals(aenc, StringComparison.OrdinalIgnoreCase)));

            if (encodings is not null)
            {
                return encodings;
            }
        }

        return "none";
    }

    public static IApplicationBuilder UseApplicationFeatures(this IApplicationBuilder app, IFileProvider fileProvider, DynDocFeatures features)
    {
        return app.Use(async (context, next) =>
        {
            if (await ApplicationAddFeatures(context,
                                             context.Request,
                                             context.Response,
                                             fileProvider,
                                             features,
                                             context.RequestAborted).ConfigureAwait(continueOnCapturedContext: false))
            {
                return;
            }

            await next().ConfigureAwait(continueOnCapturedContext: false);
        });
    }

    /// <summary>
    /// Adds dyndoc features and /redir
    /// feature to a web server request.
    /// </summary>
    /// <returns>True if response is redirected and completed, False if response is allowed to continue</returns>
    public static ValueTask<bool> ApplicationAddFeatures(this HttpContext context,
                                                         HttpRequest request,
                                                         HttpResponse response,
                                                         IFileProvider fileProvider,
                                                         DynDocFeatures features,
                                                         CancellationToken cancellationToken)
    {
        if (context.Connection.RemoteIpAddress is not null
            && !request.Path.StartsWithSegments("/robots.txt", StringComparison.Ordinal)
            && (features & DynDocFeatures.DenyBotNets) != 0
            && BotNetRanges.Encompasses(context.Connection.RemoteIpAddress))
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return new(true);
        }

        var requestExt = Path.GetExtension(request.Path);

        if ((features & DynDocFeatures.DynDoc) != 0 &&
            request.Path.StartsWithSegments("/dyndoc", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var encodedPath = request.Query["path"].FirstOrDefault();
                var encodedQuery = request.Query["query"].FirstOrDefault();

                if ((string.IsNullOrWhiteSpace(encodedQuery)
                    || string.IsNullOrWhiteSpace(encodedPath))
                    && request.Path.HasValue)
                {
                    var requestPath = request.Path.Value;
                    var docSeparator = requestPath.LastIndexOf('/');
                    var querySeparator = requestPath.LastIndexOf('/', docSeparator - 1);

                    encodedPath = requestPath.Substring("/dyndoc".Length, querySeparator - "/dyndoc".Length);
                    encodedQuery = requestPath.Substring(querySeparator + 1, docSeparator - querySeparator - 1).Replace('_', '/');
                }
                
                if (!string.IsNullOrWhiteSpace(encodedQuery)
                    && !string.IsNullOrWhiteSpace(encodedPath))
                {
                    var query = Encoding.UTF8.GetString(Convert.FromBase64String(encodedQuery));

                    request.Path = PathString.FromUriComponent(encodedPath);
                    request.QueryString = QueryString.FromUriComponent(query);

                    requestExt = Path.GetExtension(request.Path);
                }
            }
            catch
            {
            }
        }
        else if ((features & DynDocFeatures.DynDoc) != 0 &&
            request.Path.StartsWithSegments("/docid", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var encodedPath = request.Query["path"].FirstOrDefault();
                var encodedQuery = request.Query["query"].FirstOrDefault();

                if ((string.IsNullOrWhiteSpace(encodedQuery)
                    || string.IsNullOrWhiteSpace(encodedPath))
                    && request.Path.HasValue)
                {
                    var requestPath = request.Path.Value;
                    var docSeparator = requestPath.LastIndexOf('/');
                    var querySeparator = requestPath.LastIndexOf('/', docSeparator - 1);

                    encodedPath = requestPath.Substring("/docid".Length, querySeparator - "/docid".Length);
                    encodedQuery = requestPath.Substring(querySeparator + 1, docSeparator - querySeparator - 1).Replace('_', '/');
                }

                if (encodedQuery is not null
                    && !string.IsNullOrWhiteSpace(encodedQuery)
                    && !string.IsNullOrWhiteSpace(encodedPath))
                {
                    var query = GetLinkFromShort(encodedQuery);

                    if (query is null)
                    {
                        return new(false);
                    }

                    request.Path = PathString.FromUriComponent(encodedPath);
                    request.QueryString = QueryString.FromUriComponent(query);

                    requestExt = Path.GetExtension(request.Path);
                }
            }
            catch
            {
            }
        }

        if (requestExt.Equals(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            request.Path = request.Path.Value!.Remove(request.Path.Value.Length - requestExt.Length);
        }
        else if (requestExt.Equals(".asp", StringComparison.OrdinalIgnoreCase))
        {
            request.Path = request.Path.Value!.Remove(request.Path.Value.Length - requestExt.Length);
        }
        else if ((requestExt.Equals(".html", StringComparison.OrdinalIgnoreCase) ||
            requestExt.Equals(".htm", StringComparison.OrdinalIgnoreCase)) &&
            !fileProvider.GetFileInfo(request.Path).Exists &&
            !fileProvider.GetDirectoryContents(request.Path).Exists)
        {
            request.Path = request.Path.Value!.Remove(request.Path.Value.Length - requestExt.Length);
        }

        if ((features & DynDocFeatures.Redir) != 0 &&
            request.Path.StartsWithSegments("/redir", StringComparison.OrdinalIgnoreCase))
        {

            string msg;
            
            try
            {
                var encodedQuery = request.Query["query"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(encodedQuery))
                {
                    encodedQuery = request.Path.Value
                        .AsSpan("/redir/".Length)
                        .TokenEnum('/')
                        .ElementAtOrDefault(0)
                        .ToString();
                }
                
                var docquery = Encoding.UTF8.GetString(Convert.FromBase64String(encodedQuery));
                
                response.Redirect(docquery, permanent: true);
                
                return new(true);
            }
            catch (Exception ex)
            {
                msg = ex.JoinMessages();
            }

            static async ValueTask<bool> WriteError(string msg, HttpResponse response, CancellationToken cancellationToken)
            {
                await response.WriteAsync(msg, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                return true;
            }

            return WriteError(msg, response, cancellationToken);
        }

        if ((features & DynDocFeatures.Checksum) != 0
            && !string.IsNullOrWhiteSpace(requestExt)
            && CheckSums.TryGetValue(requestExt, out var hashProviderFunc))
        {
            var checksumFile = fileProvider.GetFileInfo(request.Path).PhysicalPath;
            var baseFile = checksumFile?.Remove(checksumFile.Length - requestExt.Length);
            var baseFileExtension = Path.GetExtension(baseFile);

            if (baseFile is not null
                && checksumFile is not null
                && File.Exists(baseFile)
                && PublicCacheableExtensions.Any(ext => ext.Equals(baseFileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                var baseFileWriteTime = File.GetLastWriteTimeUtc(baseFile);

                if (File.GetLastWriteTimeUtc(checksumFile) >= baseFileWriteTime)
                {
                    response.ContentType = "text/plain";
                    response.AppendHeader("Last-Modified", baseFileWriteTime);

                    return new(false);
                }

                static async ValueTask<bool> CreateChecksumFile(string baseFile,
                                                                DateTime baseFileWriteTime,
                                                                string checksumFile,
                                                                HttpResponse response,
                                                                Func<HashAlgorithm> hashProviderFunc,
                                                                CancellationToken cancellationToken)
                {
                    using var hashProvider = hashProviderFunc();
                    using var baseFileStream = File.OpenRead(baseFile);
                    using var checksumFileWriter = new StreamWriter(checksumFile, append: false, encoding: Encoding.UTF8);

#if NET5_0_OR_GREATER
                    var hash = await hashProvider.ComputeHashAsync(baseFileStream, cancellationToken).ConfigureAwait(false);
#else
                    var hash = hashProvider.ComputeHash(baseFileStream);
#endif

                    await checksumFileWriter.WriteAsync(hash.ToHexString().AsMemory(), cancellationToken).ConfigureAwait(false);
                    await checksumFileWriter.WriteAsync(" *".AsMemory(), cancellationToken).ConfigureAwait(false);
                    await checksumFileWriter.WriteLineAsync(Path.GetFileName(baseFile).AsMemory(), cancellationToken).ConfigureAwait(false);

                    response.ContentType = "text/plain";
                    response.AppendHeader("Last-Modified", baseFileWriteTime);

                    return false;
                }

                return CreateChecksumFile(baseFile, baseFileWriteTime, checksumFile, response, hashProviderFunc, cancellationToken);
            }
        }

        return new(false);
    }

    public static IPAddress? GetClientTrueIPAddress(this HttpContext context)
    {
        var address = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (address is not null && !string.IsNullOrWhiteSpace(address))
        {
            var delim = address.IndexOf(',');
            if (delim >= 0)
            {
                address = address.Remove(delim);
            }

            if (IPAddress.TryParse(address, out var ip_address))
            {
                return ip_address;
            }
        }

        return context.Connection.RemoteIpAddress;
    }

    public static bool IsHttpsClient(this HttpRequest request)
    {
        if (request.IsHttps)
        {
            return true;
        }

        var proto = request.Headers["X-Forwarded-Proto"].FirstOrDefault();

        if (proto is not null &&
            !string.IsNullOrWhiteSpace(proto) &&
            proto.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    public static HtmlString GetFormStyleTag(this HttpRequest request, IFileProvider fileProvider, string? prefix) =>
        GetFormStyleTag(request, fileProvider, prefix, inject: false);

    public static HtmlString GetFormStyleTag(this HttpRequest request, IFileProvider fileProvider, string? prefix, bool inject)
    {
        var document = new StringBuilder();

        var subpath = $"/css/{prefix}site.css";
        var cssFile = fileProvider.GetFileInfo(subpath);

        if (cssFile.Exists && cssFile.PhysicalPath is not null)
        {
            if (inject)
            {
                document.AppendLine("<style type=\"text/css\">");
                document.AppendLine(File.ReadAllText(cssFile.PhysicalPath));
                document.AppendLine("</style>");
            }
            else
            {
                document.Append("<link href=\"");
                document.Append(subpath);
                document.AppendLine("\" rel=\"stylesheet\" />");
            }

        }

        if (IsMobileBrowser(request))
        {

            subpath = $"/css/{prefix}mobile.css";
            cssFile = fileProvider.GetFileInfo(subpath);
            if (cssFile.Exists && cssFile.PhysicalPath is not null)
            {
                if (inject)
                {
                    document.AppendLine("<style type=\"text/css\">");
                    document.AppendLine(File.ReadAllText(cssFile.PhysicalPath));
                    document.AppendLine("</style>");
                }
                else
                {
                    document.Append("<link href=\"");
                    document.Append(subpath);
                    document.AppendLine("\" rel=\"stylesheet\" />");
                }
            }

        }

        if (inject)
        {
            document.AppendLine("<meta name=\"viewport\" content=\"width=device-width, minimum-scale=1.0\" />");
        }

        return new HtmlString(document.ToString());

    }

    public static object? GetPartialHtmlForModernBrowser(this HttpRequest request, IFileProvider fileprovider, string codeFile) =>
        GetPartialHtmlForModernBrowser(request, fileprovider, codeFile, default);

    public static object? GetPartialHtmlForModernBrowser(this HttpRequest request, IFileProvider fileprovider, string codeFile, string? altFile)
    {
        if (request is not null && IsPartialScriptSupportBrowser(request))
        {
            if (string.IsNullOrWhiteSpace(altFile))
            {
                return null;
            }
            else
            {
                try
                {
                    var path = fileprovider.GetFileInfo($"/partial_html/{altFile}");
                    
                    if (path.Exists && path.PhysicalPath is not null)
                    {
                        return new HtmlString(File.ReadAllText(path.PhysicalPath));
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return ex.JoinMessages(" -> ");
                }
            }
        }
        else
        {
            try
            {
                var path = fileprovider.GetFileInfo($"/partial_html/{codeFile}");
                
                if (path.Exists && path.PhysicalPath is not null)
                {
                    return new HtmlString(File.ReadAllText(path.PhysicalPath));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.JoinMessages(" -> ");
            }
        }
    }

    public static bool IsPartialScriptSupportBrowser(this HttpRequest request)
    {
#if NET6_0_OR_GREATER
        if (!string.IsNullOrWhiteSpace(request.Headers["X-Wap-Profile"])
            || request.Headers.Keys is not null
            && request.Headers.Keys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase))
                || !string.IsNullOrWhiteSpace(request.Headers.Accept)
            && request.Headers.Accept.Any(t => t is not null
                && t.Contains("wap", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }
#else
        if (!string.IsNullOrWhiteSpace(request.Headers["X-Wap-Profile"])
            || request.Headers.Keys is not null
            && request.Headers.Keys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase))
                || !string.IsNullOrWhiteSpace(request.Headers["Accept"])
            && request.Headers["Accept"].Any(t => t is not null
                && t.Contains("wap", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }
#endif

        return false;
    }

    public static bool IsMobileBrowser(this HttpRequest request)
    {
#if NET6_0_OR_GREATER
        if (request.Headers.UserAgent.Any(t => t is not null && t.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            || !string.IsNullOrWhiteSpace(request.Headers["X-Wap-Profile"])
            || request.Headers.Keys is not null && request.Headers.Keys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase))
            || !string.IsNullOrWhiteSpace(request.Headers.Accept)
            && request.Headers.Accept.Any(t => t is not null
                && t.Contains("wap", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }
#else
        if (request.Headers["User-Agent"].Any(t => t is not null && t.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            || !string.IsNullOrWhiteSpace(request.Headers["X-Wap-Profile"])
            || request.Headers.Keys is not null && request.Headers.Keys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase))
            || !string.IsNullOrWhiteSpace(request.Headers["Accept"])
            && request.Headers["Accept"].Any(t => t is not null
                && t.Contains("wap", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }
#endif

        return false;
    }

    public static MarkupString HtmlStringToMarkupString(object value)
    {
        if (value is null)
        {
            return default;
        }
        else if (value is HtmlString htmlString)
        {
            return new MarkupString(htmlString.Value ?? "");
        }
        else
        {
            return new MarkupString(WebUtility.HtmlEncode(value.ToString() ?? ""));
        }
    }

    private static readonly ConcurrentDictionary<string, string> shortLinkDictionary = new(StringComparer.Ordinal);

#if NET5_0_OR_GREATER
    public static string CreateShortLink(string request)
    {
        Span<byte> id = stackalloc byte[20];

        SHA1.HashData(MemoryMarshal.AsBytes(request.AsSpan()), id);

        var value = id.ToHexString();

        shortLinkDictionary.GetOrAdd(value, request);

        return value;
    }
#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [ThreadStatic]
    private static SHA1? sha1;

    public static string CreateShortLink(string request)
    {
        Span<byte> id = stackalloc byte[20];

        sha1 ??= SHA1.Create();

        sha1.TryComputeHash(MemoryMarshal.AsBytes(request.AsSpan()), id, out _);

        var value = id.ToHexString();

        shortLinkDictionary.GetOrAdd(value, request);

        return value;
    }
#else
    [ThreadStatic]
    private static SHA1? sha1;

    public static string CreateShortLink(string request)
    {
        sha1 ??= SHA1.Create();

        var id = sha1.ComputeHash(Encoding.Unicode.GetBytes(request));

        var value = id.ToHexString()!;

        shortLinkDictionary.GetOrAdd(value, request);

        return value;
    }
#endif

    public static string? GetLinkFromShort(string link)
    {
        if (shortLinkDictionary.TryGetValue(link, out var request))
        {
            return request;
        }
        else
        {
            return null;
        }
    }
}

#endif
