using LTRLib.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

#if NET47_OR_GREATER || NETCOREAPP

public class NetsupportTests
{
    [Fact]
    public void RangeEncompasses()
    {
        var ranges = new IPAddressRanges(AddressFamily.InterNetwork)
        {
            {"172.16.0.0", "172.31.255.255" }
        };

        var inside = IPAddress.Parse("172.16.1.1");
        
        Assert.True(ranges.Encompasses(inside));

        var outside = IPAddress.Parse("192.168.0.1");

        Assert.False(ranges.Encompasses(outside));

        var (Network, Mask, Broadcast, BitCount) = ranges.CalculateNetwork(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.31.255.255"));

        Assert.Equal(IPAddress.Parse("172.16.0.0"), Network);
        Assert.Equal(IPAddress.Parse("172.31.255.255"), Broadcast);
        Assert.Equal(IPAddress.Parse("255.240.0.0"), Mask);
        Assert.Equal(12, BitCount);

        Assert.Throws<ArgumentException>(() => ranges.CalculateNetwork(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.32.255.255")));
    }
}

#endif
