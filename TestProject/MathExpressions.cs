using LTRLib.MathExpression;
using System;
using System.Diagnostics;
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
    }
}
