using LTRLib.Geodesy.Positions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

public class Positions
{
    [Fact]
    public void ToMaidenhead()
    {
        var pos = new WGS84Position(57.71259, 12.93351);
        var mh = pos.ToMaidenhead();

        Assert.Equal("JO67LR21", mh);
    }

    [Fact]
    public void FromMaidenhead()
    {
        var pos = new WGS84Position("JO67LR21");
        Assert.Equal(57.71259, pos.Latitude, 3);
        Assert.Equal(12.93351, pos.Longitude, 3);
    }

    [Fact]
    public void AddDistance()
    {
        var pos1 = new WGS84Position("JO67LR21");
        var pos2 = pos1.AddDistance(72630, 86.33);

        Assert.Equal(72630, pos1.GetSurfaceDistance(pos2), 0);
        Assert.Equal(86.84, pos1.GetBearing(pos2), 0);

        Assert.Equal(72630, pos2.GetSurfaceDistance(pos1), 0);
        Assert.Equal(266.84, pos2.GetBearing(pos1), 0);

        var mh = pos2.ToMaidenhead();
        Assert.Equal("JO77BR89", mh);
    }
}
