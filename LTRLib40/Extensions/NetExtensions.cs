using LTRLib.Net;
using System.Net;
using System.Net.Sockets;

namespace LTRLib.Extensions;

public static class NetExtensions
{
#if NET47_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static IPAddressRanges PrivateIPRanges { get; }
        = new(AddressFamily.InterNetwork)
        {
            { "192.168.0.0", "192.168.255.255" },
            { "10.0.0.0", "10.255.255.255" },
            { "172.16.0.0", "172.31.255.255" },
            { "169.254.0.0", "169.254.255.255" },
            { "192.0.2.0", "192.0.2.255" },
            { "127.0.0.0", "127.255.255.255" },
            { "0.0.0.0", "2.255.255.255" }
        };

    public static IPAddressRanges BotNetRanges { get; }
        = new(AddressFamily.InterNetwork)
        {
            { "5.75.128.0", "5.75.255.255" },
            { "5.9.0.0", "5.9.255.255" },
            { "148.251.0.0", "148.251.255.255" },
            { "144.76.0.0", "144.76.126.203" },
            { "88.198.64.0", "88.198.255.255" },
            { "78.46.150.0", "78.47.7.255" },
            { "144.76.0.0", "144.76.126.203" },
            { "178.63.9.156", "178.63.105.17" },
            { "176.9.0.0", "176.9.75.42" },
            { "178.63.9.156", "178.63.105.17" },
            { "46.4.0.0", "46.4.255.255" },
            { "136.243.0.0", "136.243.255.255" },
            { "86.186.144.0", "86.186.144.255" },
            { "192.99.0.0", "192.99.29.255" },
            { "162.210.196.0", "162.210.199.255" },
            { "104.156.252.0", "104.156.253.255" },
            { "217.79.176.0", "217.79.191.255" },
            { "198.100.145.0", "198.100.151.255" },
            { "54.36.0.0", "54.36.255.255" },
            { "216.244.64.0", "216.244.95.255" },
            { "185.191.171.0", "185.191.171.255" },
            { "114.119.128.0", "114.119.191.255" },
            { "157.90.0.0", "157.90.255.255" },
            { "95.217.0.0", "95.217.255.255" }
        };

    public static bool IsLoopbackOrPrivate(this IPAddress remoteIpAddress)
    {
        if (remoteIpAddress.IsIPv4MappedToIPv6)
        {
            remoteIpAddress = remoteIpAddress.MapToIPv4();
        }

        return IPAddress.IsLoopback(remoteIpAddress)
            || remoteIpAddress.IsIPv6LinkLocal
            || remoteIpAddress.IsIPv6SiteLocal
            || PrivateIPRanges.Encompasses(remoteIpAddress);
    }

    public static bool IsLoopback(this IPAddress remoteIpAddress)
    {
        if (remoteIpAddress.IsIPv4MappedToIPv6)
        {
            remoteIpAddress = remoteIpAddress.MapToIPv4();
        }

        return IPAddress.IsLoopback(remoteIpAddress);
    }
#endif
}