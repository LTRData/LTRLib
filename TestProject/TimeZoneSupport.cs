using LTRLib.LTRGeneric;
using System;
using System.Text.Json;
using Xunit;

namespace LTRLib;

public class TimeZoneSupport
{
#if NET48_OR_GREATER || NETSTANDARD || NETCOREAPP
    [Fact]
    public void Json()
    {
        var dateTime = DateTime.Now;

        var cldt = new ConfiguredLocalDateTime(dateTime);

        var json = JsonSerializer.Serialize(cldt);

        var cldt2 = JsonSerializer.Deserialize<ConfiguredLocalDateTime>(json);

        Assert.Equal(cldt, cldt2);

        Assert.Equal(dateTime, cldt2.DateTime);
    }
#endif
}
