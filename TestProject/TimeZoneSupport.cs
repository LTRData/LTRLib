using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

public class TimeZoneSupport
{
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
}
