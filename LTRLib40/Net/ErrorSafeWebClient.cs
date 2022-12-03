// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Net;

namespace LTRLib.Net;

/// <summary>
/// A WebClient that does not throw an exception on protocol errors. It raises an
/// event on protocol response, which using code can handle to determine how to
/// handle the WebResponse and Exception objects.
/// </summary>
[Obsolete("WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.")]
public class ErrorSafeWebClient : WebClient
{
    public ErrorSafeWebClient() : base()
    {
    }

    public event EventHandler<WebClientGotResponseEventArgs>? GotResponse;

    /// <summary>
    /// Request method verb to use, such as GET, POST etc. Set to Nothing/null to use default.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Sets If-Modified-Since header value for http requests.
    /// </summary>
    public DateTime? IfModifiedSince { get; set; }

    /// <summary>
    /// Length of time in milliseconds before timeout.
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// Length of time in milliseconds before read/write timeout for HTTP requests.
    /// </summary>
    public int? ReadWriteTimeout { get; set; }

    protected virtual void OnGotResponse(WebClientGotResponseEventArgs e) => GotResponse?.Invoke(this, e);

    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = base.GetWebRequest(address);
        if (IfModifiedSince.HasValue && request is HttpWebRequest httpRequest)
        {
            httpRequest.IfModifiedSince = IfModifiedSince.Value;
        }
        if (!string.IsNullOrEmpty(Method))
        {
            request.Method = Method;
        }
        if (Timeout.HasValue)
        {
            request.Timeout = Timeout.Value;

            if (ReadWriteTimeout.HasValue && request is HttpWebRequest httpWebRequest)
            {
                httpWebRequest.ReadWriteTimeout = Timeout.Value;
            }
        }
        return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        var e = new WebClientGotResponseEventArgs();

        try
        {
            e.Response = base.GetWebResponse(request);
        }
        catch (WebException ex)
        when (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is not null)
        {
            e.Response = ex.Response;
            e.Exception = ex;
        }

        OnGotResponse(e);
        return e.Response;
    }
}

public class WebClientGotResponseEventArgs : EventArgs
{
    public WebResponse Response { get; set; } = null!;
    public WebException Exception { get; set; } = null!;
}
