using LTRLib.MathExpression;
using System;
using System.Globalization;
using Xunit;

namespace LTRLib;

public class MathExpressions
{
    [Fact]
    public void Test1()
    {
        var math = new MathExpressionParser(CultureInfo.InvariantCulture);

        var expr1 = math.ParseExpression<Func<double>>("atan2(312,2)");
        var value1 = expr1();
        Assert.Equal(Math.Atan2(312, 2), value1);

        var expr2 = math.ParseExpression<Func<double>>("sin(0.4) * 2");
        var value2 = expr2();
        Assert.Equal(Math.Sin(0.4) * 2, value2);

        var expr3 = math.ParseExpression<Func<double>>("e ** 2");
        var value3 = expr3();
        Assert.Equal(Math.Pow(Math.E, 2), value3);

        var expr4 = math.ParseExpression<Func<double>>("169 - 5 - 3 - 1");
        var value4 = expr4();
        Assert.Equal(169 - 5 - 3 - 1, value4);

        var expr5 = math.ParseExpression<Func<double>>("169 - (5 - 3 - 1)");
        var value5 = expr5();
        Assert.Equal(169 - (5 - 3 - 1), value5);

        var expr6 = math.ParseExpression<Func<double>>("169 (5 - 3 - 1)");
        var value6 = expr6();
        Assert.Equal(169 * (5 - 3 - 1), value6);

        var expr7 = math.ParseExpression<Func<double>>("169 - 5 * 3 - 1");
        var value7 = expr7();
        Assert.Equal(169 - 5 * 3 - 1, value7);

        var expr8 = math.ParseExpression<Func<double>>("169 - 5 - 3 * 1");
        var value8 = expr8();
        Assert.Equal(169 - 5 - 3 * 1, value8);

        var expr9 = math.ParseExpression<Func<double>>("169 * 5 - 3 - 1");
        var value9 = expr9();
        Assert.Equal(169 * 5 - 3 - 1, value9);

        var expr10 = math.ParseExpression<Func<double>>("2.5 + 400 / (.1 - .01) * 2");
        var value10 = expr10();
        Assert.Equal(2.5 + 400 / (.1 - .01) * 2, value10);
    }
}
