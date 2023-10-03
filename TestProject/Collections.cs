using LTRData.Extensions.Buffers;
using LTRData.Extensions.Formatting;
using LTRLib.LTRGeneric;
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

}
