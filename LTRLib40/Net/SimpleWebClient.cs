// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LTRLib.Net;

[ComVisible(true)]
[Obsolete("WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.")]
public class SimpleWebClient
{
    public event EventHandler? BeginDownload;

    public event EventHandler? Finished;

    public event EventHandler? BeginUpload;

    //public event EventHandler? UploadFinished;

    public ICredentials? Credentials { get; set; }

    public bool? UseDefaultCredentials { get; set; }

    public IWebProxy? Proxy { get; set; }

    public string? Method { get; set; }

    public WebHeaderCollection? RequestHeaders { get; set; }

    public WebHeaderCollection? ResponseHeaders { get; set; }

    public string? ContentType { get; set; }

    protected virtual WebRequest GetWebRequest(Uri uri)
    {
        var request = WebRequest.Create(uri);

        if (UseDefaultCredentials is not null)
        {
            request.UseDefaultCredentials = UseDefaultCredentials.Value;
        }
        if (Credentials is not null)
        {
            request.Credentials = Credentials;
        }
        if (Proxy is not null)
        {
            request.Proxy = Proxy;
        }
        if (Method is not null)
        {
            request.Method = Method;
        }
        if (RequestHeaders is not null)
        {
            request.Headers = RequestHeaders;
        }
        if (ContentType is not null)
        {
            request.ContentType = ContentType;
        }

        return request;
    }

    protected virtual async Task<WebResponse> GetWebResponseAsync(WebRequest request)
    {
        var response = await request.GetResponseAsync().ConfigureAwait(continueOnCapturedContext: false);

        var httpresp = response as HttpWebResponse;

        if (httpresp is not null && httpresp.StatusCode != HttpStatusCode.OK)
        {
            throw new WebException(httpresp.StatusDescription, null, WebExceptionStatus.ProtocolError, response);
        }

        return response;
    }

    public async Task DownloadFileAsync(Uri uri, string target)
    {
        using var stream = File.Open(target, FileMode.Create, FileAccess.Write, FileShare.Read | FileShare.Delete);

        try
        {
            await DownloadFileAsync(uri, stream).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex)
        {
            File.Delete(target);
            throw new Exception("File download failed", ex);
        }
    }

    public async Task<byte[]> UploadFileAsync(Uri uri, string source, bool deleteLocalOnSuccess)
    {
        byte[] result;

        using (var stream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
        {

            result = await UploadFileAsync(uri, stream).ConfigureAwait(continueOnCapturedContext: false);

            if (deleteLocalOnSuccess)
            {
                File.Delete(source);
            }
        }

        return result;
    }

    public async Task DownloadFileAsync(Uri uri, Stream target)
    {
        var request = GetWebRequest(uri);

        OnBeginDownload(EventArgs.Empty);

        var response = await GetWebResponseAsync(request).ConfigureAwait(continueOnCapturedContext: false);

        if (response.SupportsHeaders)
        {
            ResponseHeaders = response.Headers;
        }

        using (var source = response.GetResponseStream())
        {
            await source.CopyToAsync(target).ConfigureAwait(continueOnCapturedContext: false);
        }

        OnFinished(EventArgs.Empty);
    }

    public async Task<byte[]> UploadFileAsync(Uri uri, Stream source)
    {
        var request = GetWebRequest(uri);

        OnBeginUpload(EventArgs.Empty);

        using (var target = await request.GetRequestStreamAsync().ConfigureAwait(continueOnCapturedContext: false))
        {
            await source.CopyToAsync(target).ConfigureAwait(continueOnCapturedContext: false);
        }

        return await DownloadResponseData(request).ConfigureAwait(continueOnCapturedContext: false);
    }

    public async Task<byte[]> DownloadDataAsync(Uri uri)
    {
        var request = GetWebRequest(uri);

        return await DownloadResponseData(request).ConfigureAwait(continueOnCapturedContext: false);
    }

    public async Task<string> DownloadStringAsync(Uri uri, Encoding encoding)
    {
        return encoding.GetString(await DownloadDataAsync(uri).ConfigureAwait(continueOnCapturedContext: false));
    }

    public async Task<string> UploadStringAsync(Uri uri, string str, Encoding encoding)
    {
        return encoding.GetString(await UploadFileAsync(uri, new MemoryStream(encoding.GetBytes(str))).ConfigureAwait(continueOnCapturedContext: false));
    }

    protected virtual async Task<byte[]> DownloadResponseData(WebRequest request)
    {
        var response = await GetWebResponseAsync(request).ConfigureAwait(continueOnCapturedContext: false);

        MemoryStream target;

        if (response.SupportsHeaders)
        {
            ResponseHeaders = response.Headers;
        }
        else
        {
            ResponseHeaders = null;
        }

        if (ResponseHeaders is not null
            && int.TryParse(ResponseHeaders["Content-Length"], 0, NumberFormatInfo.InvariantInfo, out var contentLength))
        {
            target = new MemoryStream(contentLength);
        }
        else
        {
            target = new MemoryStream();
        }

        OnBeginDownload(EventArgs.Empty);

        using (var source = response.GetResponseStream())
        {
            await source.CopyToAsync(target).ConfigureAwait(continueOnCapturedContext: false);
        }

        OnFinished(EventArgs.Empty);

        return target.GetBuffer();
    }

    protected virtual void OnFinished(EventArgs e)
    {
        Finished?.Invoke(this, e);
    }

    protected virtual void OnBeginDownload(EventArgs e)
    {
        BeginDownload?.Invoke(this, e);
    }

    protected virtual void OnBeginUpload(EventArgs e)
    {
        BeginUpload?.Invoke(this, e);
    }
}

#endif
