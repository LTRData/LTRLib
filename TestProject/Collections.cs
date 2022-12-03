using LTRLib.Extensions;
using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        var spanHex = span.ToHexString();
        Assert.Equal("0d0a", spanHex);
#endif

        var array = span.ToArray();
        var arrayHex = array.ToHexString();
        Assert.Equal("0d0a", arrayHex);

        var enumerable = array.AsEnumerable();
        var enumerableHex = enumerable.ToHexString();
        Assert.Equal("0d0a", enumerableHex);
    }

}
