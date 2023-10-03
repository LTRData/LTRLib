using LTRData.Extensions.Formatting;
using LTRLib.LTRGeneric;
using System;
using Xunit;

namespace LTRLib;

public class SizeCalc
{
    [Fact]
    public void Test1()
    {
        var size256KB = SizeFormatting.ParseSuffixedSize("256K") ?? throw new FormatException();
        Assert.Equal(256 << 10, size256KB);

        var size512b = SizeFormatting.ParseSuffixedSize("512") ?? throw new FormatException();
        Assert.Equal(512, size512b);

        var str256KB = SizeFormatting.FormatBytes(size256KB);
        Assert.Equal($"{256:0.0} KB", str256KB);
    }
}
