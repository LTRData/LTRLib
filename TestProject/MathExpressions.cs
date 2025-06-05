using LTRLib.MathExpression;
using System;
using System.Globalization;
using Xunit;

namespace LTRLib;

public class MathExpressions
{
    [Fact]
    public void Test2()
    {
        var math = new MathExpressionParser(CultureInfo.InvariantCulture);

        Assert.Equal(Math.Sin(0.4) * 2, math.ParseExpression<Func<double>>("sin(0.4) * 2")());
    }

    [Fact]
    public void Test1()
    {
        var math = new MathExpressionParser(CultureInfo.InvariantCulture);

        Assert.Equal(Math.Atan2(312, 2), math.ParseExpression<Func<double>>("atan2(312,2)")());

        Assert.Equal(Math.Pow(Math.E, 2), math.ParseExpression<Func<double>>("e ** 2")());

        Assert.Equal(169 - 5 - 3 - 1, math.ParseExpression<Func<double>>("169 - 5 - 3 - 1")());

        Assert.Equal(169 - (5 - 3 - 1), math.ParseExpression<Func<double>>("169 - (5 - 3 - 1)")());

        Assert.Equal(169 * (5 - 3 - 1), math.ParseExpression<Func<double>>("169 (5 - 3 - 1)")());

        Assert.Equal(169 - 5 * 3 - 1, math.ParseExpression<Func<double>>("169 - 5 * 3 - 1")());

        Assert.Equal(169 - 5 - 3 * 1, math.ParseExpression<Func<double>>("169 - 5 - 3 * 1")());

        Assert.Equal(169 * 5 - 3 - 1, math.ParseExpression<Func<double>>("169 * 5 - 3 - 1")());

        Assert.Equal(2.5 + 400 / (.1 - .01) * 2, math.ParseExpression<Func<double>>("2.5 + 400 / (.1 - .01) * 2")());
    }

    [Fact]
    public void Test3()
    {
        var math = new MathExpressionParser(CultureInfo.InvariantCulture);

        Assert.Equal(1 << 10, math.ParseExpression<Func<double>>("1 << 10")());
    }

    [Fact]
    public void Test4()
    {
        var math = new MathExpressionParser(CultureInfo.InvariantCulture);

        Assert.Equal(-35 - (-35), math.ParseExpression<Func<double>>("-35-(-35)")());

        Assert.Equal(-(35 - (-35)), math.ParseExpression<Func<double>>("-(35-(-35))")());

        Assert.Equal(-35 - (-35), math.ParseExpression<Func<double>>("-(35)-(-35)")());

        Assert.Equal(- -35, math.ParseExpression<Func<double>>("--35")());

        Assert.Equal(+35, math.ParseExpression<Func<double>>("+35")());

        Assert.Equal(+-35, math.ParseExpression<Func<double>>("+-35")());

        Assert.Equal(-+35, math.ParseExpression<Func<double>>("-+35")());

        Assert.Equal(+-+35, math.ParseExpression<Func<double>>("+-+35")());
    }
}
