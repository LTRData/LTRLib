using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

public class TextExtensions
{
    [Fact]
    public void TextExtTest()
    {
        var a = new object();
        a.ToMembersString();
    }

    [Fact]
    public void StringBuilderExtTest()
    {
        const string str = "TESTSTRING";
        var a = new StringReader(str);
        Assert.Equal(str, a.ReadToEnd());
        a.SetPosition(0);
        Assert.Equal(str, a.ReadToEnd());
    }

    [Fact]
    public void StringExtTest()
    {
        const string str = "TESTSTRING";
        var a = str.IndexOfAny(["EST", "ING"], 0, str.Length, StringComparison.InvariantCulture);
        Assert.Equal(1, a);
    }
}
