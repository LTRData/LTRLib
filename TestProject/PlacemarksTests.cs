#if NET48_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Placemarks;
using NetTopologySuite.Geometries;
using Xunit;

namespace LTRLib;

public class PlacemarksTests
{
    [Fact]
    public void GeometryFromWKTPoint()
    {
        const string wktPoint = "POINT (2 1)";

        var geometry = PlacemarkSupport.GeometryFromText(wktPoint);

        Assert.IsType<Point>(geometry);

        Assert.Equal(2, geometry.Coordinate.X);

        Assert.Equal(1, geometry.Coordinate.Y);
    }

    [Fact]
    public void GeometryFromWKTLine()
    {
        const string wktPoint = "LINESTRING (2 2, 3 4, 4 6)";

        var geometry = PlacemarkSupport.GeometryFromText(wktPoint);

        Assert.IsType<LineString>(geometry);
    }
}

#endif
