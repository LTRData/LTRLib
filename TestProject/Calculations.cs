using LTRLib.LTRGeneric;
using Xunit;

namespace LTRLib;

public class Calculations
{
    [Fact]
    public void Luhn()
    {
        var personnr = "780517561";
        var luhn = StringSupport.AppendLuhn(personnr, PG: false);
        Assert.Equal("7805175614", luhn);

        var result = StringSupport.ValidSwedishPersonalIdNumber(luhn);
        Assert.Equal(luhn, result);
    }

}
