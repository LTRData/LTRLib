using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endif

namespace LTRLib.IO;

public static class NetSupport
{
    public static string? GetHttpResultCode(Uri uri)
    {
        Stream stream;
        
        if (uri.Scheme == "https")
        {
            stream = IOSupport.OpenSslStream(uri, (_, _, _, _) => true);
        }
        else
        {
            stream = IOSupport.OpenTcpIpStream(uri.DnsSafeHost, uri.Port);
        }

        using (stream)
        {
            var rd = new StreamReader(stream, Encoding.UTF8);
            var wr = new StreamWriter(stream, Encoding.ASCII);

            wr.WriteLine($"HEAD {uri.PathAndQuery} HTTP/1.0");
            wr.WriteLine($"Host: {uri.DnsSafeHost}");
            wr.WriteLine("Connection: Close");
            wr.WriteLine();
            wr.Flush();

            return rd.ReadLine();
        }
    }

    public static WebHeaderCollection GetHttpHeaders(Uri uri)
    {

        Stream stream;
        if (uri.Scheme == "https")
        {
            stream = IOSupport.OpenSslStream(uri, (_, _, _, _) => true);
        }
        else
        {
            stream = IOSupport.OpenTcpIpStream(uri.DnsSafeHost, uri.Port);
        }

        using (stream)
        {

            var rd = new StreamReader(stream, Encoding.UTF8);
            var wr = new StreamWriter(stream, Encoding.ASCII);

            wr.WriteLine($"HEAD {uri.PathAndQuery} HTTP/1.0");
            wr.WriteLine($"Host: {uri.DnsSafeHost}");
            wr.WriteLine("Connection: close");
            wr.WriteLine();
            wr.Flush();

            var response = rd.ReadLine();

            var headers = new WebHeaderCollection();

            do
            {
                var line = rd.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                headers.Add(line);
            }

            while (true);

            return headers;

        }

    }

    private static readonly Dictionary<string, byte[]> _DownloadAndCacheData_cache = [];

    [Obsolete("Use DownloadAndCacheDataAsync instead.")]
    public static byte[]? DownloadAndCacheData(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        try
        {
            byte[]? data = null;
            lock (_DownloadAndCacheData_cache)
            {
                if (!_DownloadAndCacheData_cache.TryGetValue(str, out data))
                {
                    using (var WebClient = new WebClient())
                    {
                        data = WebClient.DownloadData(str);
                    }

                    _DownloadAndCacheData_cache.Add(str, data);
                }
            }
            
            return data;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to download image {str}: {ex}");
            return null;
        }
    }

}