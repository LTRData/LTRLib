using LTRLib.LTRGeneric;
using System;
using Xunit;

namespace LTRLib;

public class SizeCalc
{
    [Fact]
    public void Test1()
    {
        var size256KB = StringSupport.ParseSuffixedSize("256K") ?? throw new FormatException();
        Assert.Equal(256 << 10, size256KB);

        var size512b = StringSupport.ParseSuffixedSize("512") ?? throw new FormatException();
        Assert.Equal(512, size512b);

        var str256KB = StringSupport.FormatBytes(size256KB);
        Assert.Equal($"{256:0.0} KB", str256KB);
    }
}
