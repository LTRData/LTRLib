
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
using static LTRLib.Extensions.NetExtensions;
using Microsoft.AspNetCore.Http.Extensions;

using HttpUtility = System.Web.HttpUtility;

namespace LTRLib.WebCore;


[ComVisible(false)]
[XmlType("WebCoreHttpServerSupport")]
public static class HttpServerSupport
{
    private static readonly string[] _compressionHeaderIndicators = { "gzip", "deflate" };

    public static string GetRequestCompressionEncoding(this HttpRequest Request)
    {
        var acceptEncoding = Request.Headers.Get("Accept-Encoding") ?? Request.Headers.Get("Transfer-Encoding") ?? Request.Headers.Get("TE");

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

    /// <summary>
    /// Adds dyndoc features and /reloadapp
    /// feature to a web server request.
    /// </summary>
    /// <returns>True if response is redirected and completed, False if response is allowed to continue</returns>
    public static async Task<bool> ApplicationAddFeatures(this HttpContext Context,
                                                          HttpRequest Request,
                                                          HttpResponse Response,
                                                          IFileProvider FileProvider,
                                                          DynDocFeatures Features)
    {
        if (Context.Connection.RemoteIpAddress is not null
            && !Request.Path.StartsWithSegments("/robots.txt", StringComparison.Ordinal)
            && (Features & DynDocFeatures.DenyBotNets) != 0
            && BotNetRanges.Encompasses(Context.Connection.RemoteIpAddress))
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return true;
        }

        var requestExt = Path.GetExtension(Request.Path);

        if ((Features & DynDocFeatures.DynDoc) != 0 &&
            Request.Path.StartsWithSegments("/dyndoc", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var encodedPath = Request.Query["path"].FirstOrDefault();
                var encodedQuery = Request.Query["query"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(encodedQuery)
                    || string.IsNullOrWhiteSpace(encodedPath))
                {
                    var requestPath = Request.Path.Value;
                    var docSeparator = requestPath.LastIndexOf('/');
                    var querySeparator = requestPath.LastIndexOf('/', docSeparator - 1);

                    encodedPath = requestPath.Substring("/dyndoc".Length, querySeparator - "/dyndoc".Length);
                    encodedQuery = requestPath.Substring(querySeparator + 1, docSeparator - querySeparator - 1).Replace('_', '/');
                }
                
                if (!string.IsNullOrWhiteSpace(encodedQuery)
                    && !string.IsNullOrWhiteSpace(encodedPath))
                {
                    var query = Encoding.UTF8.GetString(Convert.FromBase64String(encodedQuery));

                    Request.Path = PathString.FromUriComponent(encodedPath);
                    Request.QueryString = QueryString.FromUriComponent(query);

                    requestExt = Path.GetExtension(Request.Path);
                }
            }
            catch
            {
            }
        }

        if (requestExt.Equals(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            Request.Path = Request.Path.Value.Remove(Request.Path.Value.Length - requestExt.Length);
        }
        else if (requestExt.Equals(".asp", StringComparison.OrdinalIgnoreCase))
        {
            Request.Path = Request.Path.Value.Remove(Request.Path.Value.Length - requestExt.Length);
        }
        else if ((requestExt.Equals(".html", StringComparison.OrdinalIgnoreCase) ||
            requestExt.Equals(".htm", StringComparison.OrdinalIgnoreCase)) &&
            !FileProvider.GetFileInfo(Request.Path).Exists &&
            !FileProvider.GetDirectoryContents(Request.Path).Exists)
        {
            Request.Path = Request.Path.Value.Remove(Request.Path.Value.Length - requestExt.Length);
        }

        if ((Features & DynDocFeatures.Redir) != 0 &&
            Request.Path.StartsWithSegments("/redir", StringComparison.OrdinalIgnoreCase))
        {

            string msg;
            
            try
            {
                var encodedQuery = Request.Query["query"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(encodedQuery))
                {
                    encodedQuery = Request.Path.Value
                        .AsSpan("/redir/".Length)
                        .Split('/')
                        .ElementAtOrDefault(0)
                        .ToString();
                }
                
                var docquery = Encoding.UTF8.GetString(Convert.FromBase64String(encodedQuery));
                
                Response.Redirect(docquery, permanent: true);
                
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.JoinMessages();
            }

            await Response.WriteAsync(msg, Context.RequestAborted).ConfigureAwait(continueOnCapturedContext: false);
            return true;
        }

        if ((Features & DynDocFeatures.Checksum) != 0 && !string.IsNullOrWhiteSpace(requestExt) && CheckSums.ContainsKey(requestExt))
        {
            var checksumFile = FileProvider.GetFileInfo(Request.Path).PhysicalPath;
            var baseFile = checksumFile.Remove(checksumFile.Length - requestExt.Length);
            var baseFileExtension = Path.GetExtension(baseFile);

            if (File.Exists(baseFile) &&
                PublicCacheableExtensions.Any(ext => ext.Equals(baseFileExtension, StringComparison.OrdinalIgnoreCase)))
            {

                var baseFileWriteTime = File.GetLastWriteTimeUtc(baseFile);

                if (File.GetLastWriteTimeUtc(checksumFile) < baseFileWriteTime)
                {

                    using var hashProvider = CheckSums[requestExt]();
                    using var baseFileStream = File.OpenRead(baseFile);
                    using var checksumFileWriter = new StreamWriter(checksumFile, append: false, encoding: Encoding.UTF8);

                    var hash = hashProvider.ComputeHash(baseFileStream);

                    Array.ForEach(hash, b => checksumFileWriter.Write(b.ToString("x2")));

                    checksumFileWriter.Write(" *");
                    checksumFileWriter.WriteLine(Path.GetFileName(baseFile));

                }

                Response.ContentType = "text/plain";
                Response.AppendHeader("Last-Modified", baseFileWriteTime);
            }
        }

        return false;
    }

    public static IPAddress GetClientTrueIPAddress(this HttpContext context)
    {
        var address = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(address))
        {

            var delim = address.IndexOf(",", StringComparison.Ordinal);
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

        if (!string.IsNullOrWhiteSpace(proto) && proto.EndsWith("s", StringComparison.OrdinalIgnoreCase))
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

        if (cssFile.Exists)
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
            if (cssFile.Exists)
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
                    if (path.Exists)
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
                if (path.Exists)
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

    public static bool IsPartialScriptSupportBrowser(this HttpRequest Request)
    {

        if (!string.IsNullOrWhiteSpace(Request.Headers["X-Wap-Profile"]) ||
            Request.Headers.Keys is not null && Request.Headers.Keys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase)) ||
            !string.IsNullOrWhiteSpace(Request.Headers["Accept"]) && Request.Headers["Accept"].Any(t => t.Contains("wap", StringComparison.OrdinalIgnoreCase)))
        {

            return true;

        }

        return false;

    }

    public static bool IsMobileBrowser(this HttpRequest Request)
    {

        if (Request.Headers["User-Agent"].Any(t => t.Contains("Mobile", StringComparison.OrdinalIgnoreCase)) ||
            !string.IsNullOrWhiteSpace(Request.Headers["X-Wap-Profile"]) ||
            Request.Headers.Keys is not null && Request.Headers.Keys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase)) ||
            !string.IsNullOrWhiteSpace(Request.Headers["Accept"]) && Request.Headers["Accept"].Any(t => t.Contains("wap", StringComparison.OrdinalIgnoreCase)))
        {

            return true;

        }

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
            return new MarkupString(htmlString.Value);
        }
        else
        {
            return new MarkupString(WebUtility.HtmlEncode(value.ToString()));
        }
    }

    public static string? Get(this in QueryString query, string key)
    {
        var buffer = HttpUtility.ParseQueryString(query.Value);

        return buffer[key];
    }

    public static QueryString Remove(this in QueryString query, string key)
    {
        var buffer = HttpUtility.ParseQueryString(query.Value);

        if (buffer[key] is null)
        {
            return query;
        }

        buffer.Remove(key);

        return QueryString.FromUriComponent($"?{query}");
    }
}

#endif
