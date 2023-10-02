// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET40_OR_GREATER

using LTRLib.Extensions;
using LTRLib.LTRGeneric;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using static LTRLib.Web.HttpShared;
using static LTRLib.Extensions.NetExtensions;
#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Formatting;
#endif

namespace LTRLib.Web;


[XmlType("WebHttpServerSupport")]
public static class HttpServerSupport
{
    private static readonly string[] _compressionHeaderIndicators = { "gzip", "deflate" };

    public static string GetRequestCompressionEncoding(this HttpRequest Request)
    {

        var acceptEncoding = Request.Headers.Get("Accept-Encoding") ?? Request.Headers.Get("Transfer-Encoding") ?? Request.Headers.Get("TE");

        if (!string.IsNullOrWhiteSpace(acceptEncoding))
        {

            var encodings = acceptEncoding.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(aenc => _compressionHeaderIndicators.Any(ienc => ienc.Equals(aenc, StringComparison.OrdinalIgnoreCase)));

            if (encodings is not null)
            {
                return encodings;
            }

        }

        return "none";

    }

    /// <summary>
    /// Adds automatic gzip and deflate compression support for static content.
    /// </summary>
    public static void ApplicationAddStaticCompression(this HttpContext Context, HttpRequest Request, HttpResponse Response)
    {

#if NET451_OR_GREATER
        if (Response.HeadersWritten)
        {
            return;
        }

#else

        if (AreHeadersWritten(Response))
        {
            return;
        }

#endif

        // ' Default cache options
        Response.Cache.VaryByParams["*"] = true;
        Response.Cache.SetVaryByCustom("encoding");

        var requestExt = Request.CurrentExecutionFilePathExtension;

        var encoding = GetRequestCompressionEncoding(Request);

        Func<Stream, Stream> fcompress;

        switch (encoding)
        {
            case "gzip":
                fcompress = f => new GZipStream(f, CompressionMode.Compress);
                break;

            case "deflate":
                fcompress = f => new DeflateStream(f, CompressionMode.Compress);
                break;

            default:
                return;
        }

        Context.Items["encoding"] = encoding;
        Context.Items["fcompress"] = fcompress;

        // ' Static file compression
        if (!string.IsNullOrWhiteSpace(requestExt) && PublicCacheableExtensions.Any(ext => requestExt.Equals(ext, StringComparison.OrdinalIgnoreCase)))
        {
            var filepath = Request.PhysicalPath;

            if (!File.Exists(filepath))
            {
                return;
            }

            Response.Cache.SetMaxAge(TimeSpan.FromDays(7d));
            Response.Cache.SetCacheability(HttpCacheability.Public);

            if (!EncodableExtensions.Any(ext => requestExt.Equals(ext, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var lastModified = File.GetLastWriteTimeUtc(filepath);
            var etag = Convert.ToBase64String(Cryptography.GetHash<MD5CryptoServiceProvider>(filepath));

            Response.AddFileDependency(filepath);
            Response.ContentType = GetMimeType(requestExt);
            Response.Cache.SetLastModified(lastModified);
            Response.Cache.SetETag(etag);

            var ifModifiedSince = Request.Headers.Get("If-Modified-Since");

            if (!string.IsNullOrWhiteSpace(ifModifiedSince))
            {
                if (ifModifiedSince.IndexOf(';') >= 0)
                {
                    ifModifiedSince = ifModifiedSince.Remove(ifModifiedSince.IndexOf(';'));
                }
            }

            var modifiedSinceHit = !string.IsNullOrWhiteSpace(ifModifiedSince) && ifModifiedSince.Equals(lastModified.ToString("r"));

            var ifNoneMatch = Request.Headers.Get("If-None-Match");

            var eTagHit = !string.IsNullOrWhiteSpace(ifNoneMatch) && ifNoneMatch.Trim('"') == etag;

            if (modifiedSinceHit && (eTagHit || string.IsNullOrWhiteSpace(ifNoneMatch)) || eTagHit && string.IsNullOrWhiteSpace(ifModifiedSince))
            {

                Response.StatusCode = (int)HttpStatusCode.NotModified;
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            var encodedFile = string.Concat(filepath, GetEncodedFileExtension(encoding));

            long encodedLength;

            if (File.GetLastWriteTimeUtc(filepath) > File.GetLastWriteTimeUtc(encodedFile))
            {

                var mstream = new IO.CloseSafeMemoryStream();

                using (var cstream = fcompress(mstream))
                {
                    using var fstream = File.OpenRead(filepath);
                    fstream.CopyTo(cstream);
                }

                using (var fstream = File.Open(encodedFile, FileMode.Create, FileAccess.Write, FileShare.Delete))
                {
                    mstream.WriteTo(fstream);
                }

                encodedLength = mstream.Length;

                mstream.WriteTo(Response.OutputStream);
            }

            else
            {

                using var fstream = File.Open(encodedFile, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.Read);
                encodedLength = fstream.Length;
                fstream.CopyTo(Response.OutputStream);

            }

            Response.AppendHeader("Content-Encoding", encoding);
            Response.AppendHeader("Content-Length", encodedLength.ToString());

            Context.ApplicationInstance.CompleteRequest();
            return;

        }

    }

    private static readonly string[] aspExtensions = { ".aspx", ".asp" };

    /// <summary>
    /// Adds automatic gzip and deflate compression support for dynamic content.
    /// Requires ApplicationAddStaticCompression() to be called first.
    /// </summary>
    public static void ApplicationAddDynamicCompression(this HttpContext Context, HttpRequest Request, HttpResponse Response)
    {

#if NET451_OR_GREATER
        if (Response.HeadersWritten)
        {
            return;
        }

#else

        if (AreHeadersWritten(Response))
        {
            return;
        }

#endif

        var requestExt = Request.CurrentExecutionFilePathExtension;

        // ' Dynamic data compression
        if (string.IsNullOrWhiteSpace(requestExt) || aspExtensions.Any(ext => requestExt.Equals(ext, StringComparison.OrdinalIgnoreCase)))
        {

            var encoding = Context.Items["encoding"] as string;
            var fcompress = Context.Items["fcompress"] as Func<Stream, Stream>;

            if (!string.IsNullOrWhiteSpace(encoding) && fcompress is not null)
            {

                Response.Filter = fcompress(Response.Filter);
                Response.AppendHeader("Content-Encoding", encoding);

            }

        }

    }

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Adds dyndoc features and /reloadapp
    /// feature to a web server request.
    /// </summary>
    /// <returns>True if response is redirected and completed, False if response is allowed to continue</returns>
    public static bool ApplicationAddFeatures(this HttpContext Context, HttpRequest Request, HttpResponse Response, DynDocFeatures Features)
    {

#if NET47_OR_GREATER
        if (!"/robots.txt".Equals(Request.Path, StringComparison.Ordinal) && (Features & DynDocFeatures.DenyBotNets) != 0 && BotNetRanges.Encompasses(Request.UserHostAddress))
        {

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
            return true;
        }
#endif

        var requestExt = Request.CurrentExecutionFilePathExtension;

        if ((Features & DynDocFeatures.DynDoc) != 0 && Request.Path.StartsWith("/dyndoc/", StringComparison.OrdinalIgnoreCase))
        {

            try
            {
                var encodedQuery = Request.Params.Get("query");
                if (string.IsNullOrWhiteSpace(encodedQuery))
                {
                    var args = Request.Path.Substring("/dyndoc/".Length).Split('/');
                    encodedQuery = args[0];
                }

                var docquery = Encoding.UTF8.GetString(Convert.FromBase64String(encodedQuery));
                Context.RewritePath(docquery);
                requestExt = Request.CurrentExecutionFilePathExtension;
            }

            catch
            {

            }

        }

        if ((Features & DynDocFeatures.DynDoc) != 0
            && requestExt.StartsWith(".htm", StringComparison.OrdinalIgnoreCase)
            && !File.Exists(Request.PhysicalPath)
            && File.Exists(Path.ChangeExtension(Request.PhysicalPath, ".aspx")))
        {

            try
            {
                var newUrl = string.Concat(Path.ChangeExtension(Request.FilePath, ".aspx"), "?", Request.QueryString);
                Context.RewritePath(newUrl);
                requestExt = Request.CurrentExecutionFilePathExtension;
            }
            catch
            {
            }

        }

        if ((Features & DynDocFeatures.Redir) != 0
            && Request.Path.StartsWith("/redir/", StringComparison.OrdinalIgnoreCase))
        {

            try
            {
                var encodedQuery = Request.Params.Get("query");
                if (string.IsNullOrWhiteSpace(encodedQuery))
                {
                    var args = Request.Path.Substring("/redir/".Length).Split('/');
                    encodedQuery = args[0];
                }

                var docquery = Encoding.UTF8.GetString(Convert.FromBase64String(encodedQuery));
                Response.Redirect(docquery, endResponse: true);
                return true;
            }
            catch (Exception ex)
            {
                Response.Write(ex.JoinMessages());
                Response.Flush();
                Context.ApplicationInstance.CompleteRequest();
                return true;
            }

        }

        if ((Features & DynDocFeatures.ReloadApp) != 0 && Request.Path.Equals("/reloadapp", StringComparison.OrdinalIgnoreCase))
        {

            Response.ContentType = "text/plain";
            Response.ContentEncoding = Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.Write("Restarting application..." + Environment.NewLine);
            Response.Flush();

            try
            {
                HttpRuntime.UnloadAppDomain();
                Response.Write("Done." + Environment.NewLine);
            }

            catch (Exception ex)
            {
                Response.Write("Error: " + ex.JoinMessages(" -> ") + Environment.NewLine);

            }

            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
            return true;

        }

        if ((Features & DynDocFeatures.Checksum) != 0 && !string.IsNullOrWhiteSpace(requestExt) && CheckSums.ContainsKey(requestExt))
        {

            var checksumFile = Request.PhysicalPath;
            var baseFile = checksumFile.Remove(checksumFile.Length - requestExt.Length);
            var baseFileExtension = Path.GetExtension(baseFile);

            if (File.Exists(baseFile) && PublicCacheableExtensions.Any(ext => baseFileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase)))
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

                Response.AddFileDependency(baseFile);
                Response.ContentType = "text/plain";
                Response.Cache.SetLastModified(baseFileWriteTime);

            }

        }

        return false;

    }

    public static string GetPartialHtmlForModernBrowser(this HttpRequest request, string codeFile) => GetPartialHtmlForModernBrowser(request, codeFile, null);

    public static string GetPartialHtmlForModernBrowser(this HttpRequest request, string codeFile, string? altFile)
    {

        if (IsPartialScriptSupportBrowser(request))
        {
            if (string.IsNullOrWhiteSpace(altFile))
            {
                return "";
            }
            else
            {
                try
                {
                    var path = request.MapPath($"~/partial_html/{altFile}");
                    if (File.Exists(path))
                    {
                        return File.ReadAllText(path);
                    }
                    else
                    {
                        return "";
                    }
                }

                catch (Exception ex)
                {
                    return HttpUtility.HtmlEncode(ex.JoinMessages(" -> "));

                }
            }
        }
        else
        {
            try
            {
                var path = request.MapPath($"~/partial_html/{codeFile}");
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
                else
                {
                    return "";
                }
            }

            catch (Exception ex)
            {
                return HttpUtility.HtmlEncode(ex.JoinMessages(" -> "));

            }
        }

    }
#endif

    public static bool IsWapBrowser(this HttpRequest Request)
    {

        if (!string.IsNullOrWhiteSpace(Request.Headers["X-Wap-Profile"]) || Request.Headers.AllKeys is not null && Request.Headers.AllKeys.Any(t => t.StartsWith("X-OperaMini", StringComparison.OrdinalIgnoreCase)) || Request.AcceptTypes is not null && Request.AcceptTypes.Any(t => t.IndexOf("wap", StringComparison.OrdinalIgnoreCase) >= 0))
        {

            return true;

        }

        return false;

    }

    public static bool IsPartialScriptSupportBrowser(this HttpRequest request)
    {

        if (request.Browser.EcmaScriptVersion is null || request.Browser.EcmaScriptVersion.Major < 1 || request.Browser.Crawler || IsWapBrowser(request))
        {

            return true;

        }

        return false;

    }

    public static bool IsMobileBrowser(this HttpRequest request)
    {

        if (request.Browser.IsMobileDevice || IsWapBrowser(request))
        {

            return true;

        }

        return false;

    }

    public static string GetFormStyleTag(this HttpRequest request, string prefix)
    {

        var document = new StringBuilder();

        var cssFile = new FileInfo(request.MapPath($"~/Styles/{prefix}Forms.css"));
        if (cssFile.Exists)
        {
            document.AppendLine("<style type=\"text/css\">");
            document.AppendLine(File.ReadAllText(cssFile.FullName));
            document.AppendLine("</style>");
        }

        if (request.Browser.IsMobileDevice)
        {

            cssFile = new FileInfo(request.MapPath($"~/Styles/{prefix}FormsMobile.css"));
            if (cssFile.Exists)
            {
                document.AppendLine("<style type=\"text/css\">");
                document.AppendLine(File.ReadAllText(cssFile.FullName));
                document.AppendLine("</style>");
            }

        }

        document.AppendLine("<meta name=\"viewport\" content=\"width=device-width, minimum-scale=1.0\" />");

        return document.ToString();

    }

#if !NET451_OR_GREATER

    private static readonly object headersWrittenFuncLock = new();
    private static Func<HttpResponse, bool>? headersWrittenFunc;

    public static bool AreHeadersWritten(this HttpResponse response)
    {

        if (headersWrittenFunc is null)
        {
            lock (headersWrittenFuncLock)
            {
                if (headersWrittenFunc is null)
                {
                    var method = typeof(HttpResponse).GetProperty("HeadersWritten", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod();
                    var param = Expression.Parameter(typeof(HttpResponse));
                    var callx = Expression.Call(param, method);
                    headersWrittenFunc = Expression.Lambda<Func<HttpResponse, bool>>(callx, param).Compile();
                }
            }
        }

        return headersWrittenFunc.Invoke(response);

    }

#endif

#if NET47_OR_GREATER

    public static IPAddress? GetClientTrueIPAddress(this HttpRequest Request)
    {
        var address = Request.Headers["X-Forwarded-For"];

        IPAddress ip_address;
        if (!string.IsNullOrWhiteSpace(address))
        {
            var delim = address.IndexOf(',');

            if (delim >= 0)
            {
                address = address.Remove(delim);
            }

            if (IPAddress.TryParse(address, out ip_address))
            {
                return ip_address;
            }
        }

        address = Request.UserHostAddress;

        if (IPAddress.TryParse(address, out ip_address))
        {
            return ip_address;
        }

        return null;
    }

    public static bool IsHttpsClient(this HttpRequest request)
    {

        if (request.IsSecureConnection)
        {

            return true;

        }

        var proto = request.Headers["X-Forwarded-Proto"];

        if (!string.IsNullOrWhiteSpace(proto) && proto.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {

            return true;

        }

        return false;

    }

#endif
}

#endif
