﻿using LTRData.Extensions.Buffers;
using LTRData.Extensions.Formatting;
using LTRLib.LTRGeneric;
using LTRLib.Net;
#if NET471_OR_GREATER || NETCOREAPP
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Linq;
using Xunit;

namespace LTRLib;

public class Collections
{
    [Fact]
    public void SingValueTest()
    {
        var enumerable = SingleValueEnumerable.Get(2).ToArray();

        Assert.Single(enumerable);
        Assert.Equal(2, enumerable[0]);
    }

    [Fact]
    public void ToHexStringTest()
    {
        var span = "\r\n"u8;
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
        var spanHex = span.ToHexString();
        Assert.Equal("0d0a", spanHex);
#endif

        var array = span.ToArray();
        var arrayHex = array.ToHexString();
        Assert.Equal("0d0a", arrayHex);

        var collectionAsEnumerable = array.AsEnumerable();
        var collectionAsEnumerableHex = collectionAsEnumerable.ToHexString();
        Assert.Equal("0d0a", collectionAsEnumerableHex);

        var enumerable = array.Take(array.Length);
        var enumerableHex = enumerable.ToHexString();
        Assert.Equal("0d0a", enumerableHex);
    }

    [Fact]
    public void ToHexStringWithDelimiterTest()
    {
        var span = "\r\n"u8;
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
        var spanHex = span.ToHexString(":".AsSpan());
        Assert.Equal("0d:0a", spanHex);
#endif

        var array = span.ToArray();
        var arrayHex = array.ToHexString(":");
        Assert.Equal("0d:0a", arrayHex);

        var collectionAsEnumerable = array.AsEnumerable();
        var collectionAsEnumerableHex = collectionAsEnumerable.ToHexString(":");
        Assert.Equal("0d:0a", collectionAsEnumerableHex);

        var enumerable = array.Take(array.Length);
        var enumerableHex = enumerable.ToHexString(":");
        Assert.Equal("0d:0a", enumerableHex);
    }

    [Fact]
    public void BitmapTests()
    {
        var buffer = new byte[512];

        buffer.AsSpan().Fill(0xff);

        Assert.True(buffer.GetBit(9));
        Assert.True(buffer.GetBit(10));

        buffer.AsSpan().Fill(0x00);

        Assert.False(buffer.GetBit(9));
        Assert.False(buffer.GetBit(10));

        buffer.SetBit(9);

        Assert.True(buffer.GetBit(9));
        Assert.False(buffer.GetBit(10));

        Assert.Equal(2, buffer[1]);
    }

#if NET471_OR_GREATER || NETCOREAPP
    [Fact]
    public void QueryStrings()
    {
        var q = QueryString.Create("a", "b");

        Assert.Equal("?a=b", q.Value);

        q = q.Add("c", "d");

        Assert.Equal("?a=b&c=d", q.Value);

        Assert.Equal("b", q.Get("a"));
        Assert.Equal("d", q.Get("c"));

        q = q.Remove("a");

        Assert.Equal("?c=d", q.Value);

        Assert.Null(q.Get("a"));
        Assert.Equal("d", q.Get("c"));
    }
#endif
}
