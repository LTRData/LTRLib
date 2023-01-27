/*
 * Geodesy - Björn Sållarp 2009
 * 
 * RT90, SWEREF99 and WGS84 coordinate transformation library
 * 
 * 
 * Read my blog @ http://blog.sallarp.com
 * 
 * License: http://creativecommons.org/licenses/by-nc-sa/3.0/
 * 
 * Modified and extended by Olof Lagerkvist 2011. http://www.ltr-data.se
 */

using System;
using System.Globalization;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Geodesy.Positions;

using Conversion;

[Guid("d9775dac-574b-4978-95c2-6b7fa00b63e5")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class SWEREF99Position
    : LatLonPosition
{
    public enum SWEREFProjection
    {
        sweref_99_tm = 0,
        sweref_99_12_00 = 1,
        sweref_99_13_30 = 2,
        sweref_99_15_00 = 3,
        sweref_99_16_30 = 4,
        sweref_99_18_00 = 5,
        sweref_99_14_15 = 6,
        sweref_99_15_45 = 7,
        sweref_99_17_15 = 8,
        sweref_99_18_45 = 9,
        sweref_99_20_15 = 10,
        sweref_99_21_45 = 11,
        sweref_99_23_15 = 12
    }

    /// <summary>
    /// Create an empty Sweref99 position with 
    /// Sweref 99 TM as default projection.
    /// </summary>
    public SWEREF99Position()
        : base()
    {
        Projection = SWEREFProjection.sweref_99_tm;
    }

    /// <summary>
    /// Create a Sweref99 position from double values with 
    /// Sweref 99 TM as default projection.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="e"></param>
    public SWEREF99Position(double n, double e)
        : base(n, e)
    {
        Projection = SWEREFProjection.sweref_99_tm;
    }

    /// <summary>
    /// Create a Sweref99 position from double values with 
    /// Sweref 99 TM as default projection.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="e"></param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public SWEREF99Position(ReadOnlySpan<char> n, ReadOnlySpan<char> e)
        : base(n, e)
    {
        Projection = SWEREFProjection.sweref_99_tm;
    }

    /// <summary>
    /// Create a Sweref99 position from double values. Supply the projection
    /// for values other than Sweref 99 TM
    /// </summary>
    /// <param name="n"></param>
    /// <param name="e"></param>
    /// <param name="projection"></param>
    public SWEREF99Position(ReadOnlySpan<char> n, ReadOnlySpan<char> e, SWEREFProjection projection)
        : base(n, e)
    {
        Projection = projection;
    }

    /// <summary>
    /// Create a new Sweref99 position from a string containing both
    /// latitude and longitude. The string is parsed based on the 
    /// supplied format.
    /// </summary>
    /// <param name="positionString"></param>
    /// <param name="format"></param>
    public SWEREF99Position(ReadOnlySpan<char> positionString, GeoFormat format)
        : base(positionString, format)
    {
    }

    /// <summary>
    /// Create a new Sweref99 position from a string containing both
    /// latitude and longitude.
    /// </summary>
    /// <param name="positionString"></param>
    public SWEREF99Position(ReadOnlySpan<char> positionString)
        : base(positionString)
    {
    }
#endif
    public SWEREF99Position(string n, string e)
        : base(n, e)
    {
        Projection = SWEREFProjection.sweref_99_tm;
    }

    /// <summary>
    /// Create a Sweref99 position from double values. Supply the projection
    /// for values other than Sweref 99 TM
    /// </summary>
    /// <param name="n"></param>
    /// <param name="e"></param>
    /// <param name="projection"></param>
    public SWEREF99Position(string n, string e, SWEREFProjection projection)
        : base(n, e)
    {
        Projection = projection;
    }

    /// <summary>
    /// Create a new Sweref99 position from a string containing both
    /// latitude and longitude. The string is parsed based on the 
    /// supplied format.
    /// </summary>
    /// <param name="positionString"></param>
    /// <param name="format"></param>
    public SWEREF99Position(string positionString, GeoFormat format)
        : base(positionString, format)
    {
    }

    /// <summary>
    /// Create a new Sweref99 position from a string containing both
    /// latitude and longitude.
    /// </summary>
    /// <param name="positionString"></param>
    public SWEREF99Position(string positionString)
        : base(positionString)
    {
    }

    /// <summary>
    /// Create a Sweref99 position from double values. Supply the projection
    /// for values other than Sweref 99 TM
    /// </summary>
    /// <param name="n"></param>
    /// <param name="e"></param>
    /// <param name="projection"></param>
    public SWEREF99Position(double n, double e, SWEREFProjection projection)
        : base(n, e)
    {
        Projection = projection;
    }

    /// <summary>
    /// Create a Sweref99 position by converting a WGS84 position
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    /// <param name="projection"></param>
    public SWEREF99Position(WGS84Position position, SWEREFProjection projection)
    {
        FromWGS84(position, projection);
    }

    /// <summary>
    /// Create a Sweref99 position by converting a WGS84 position
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    public SWEREF99Position(WGS84Position position)
    {
        FromWGS84(position, SWEREFProjection.sweref_99_tm);
    }

    /// <summary>
    /// Create a Sweref99 position by converting a WGS84 position
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    /// <param name="projection"></param>
    public void FromWGS84(WGS84Position position, SWEREFProjection projection)
    {
        var gkProjection = new GaussKreuger();
        gkProjection.swedish_params(GetProjectionString(projection));
        var lat_lon = gkProjection.geodetic_to_grid(position.Latitude, position.Longitude);
        Latitude = lat_lon[0];
        Longitude = lat_lon[1];
        Projection = projection;
    }

    /// <summary>
    /// Create a RT90 position by converting a WGS84 position
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    public override void FromWGS84(WGS84Position position)
    {
        var gkProjection = new GaussKreuger();
        gkProjection.swedish_params(GetProjectionString(Projection));
        var lat_lon = gkProjection.geodetic_to_grid(position.Latitude, position.Longitude);
        Latitude = lat_lon[0];
        Longitude = lat_lon[1];
    }

    /// <summary>
    /// Convert the position to WGS84 format
    /// </summary>
    /// <returns></returns>
    public override WGS84Position ToWGS84()
    {
        var gkProjection = new GaussKreuger();
        gkProjection.swedish_params(ProjectionString);
        var lat_lon = gkProjection.grid_to_geodetic(Latitude, Longitude);

        var newPos = new WGS84Position()
        {
            Latitude = lat_lon[0],
            Longitude = lat_lon[1],
        };

        return newPos;
    }

    private static string GetProjectionString(SWEREFProjection projection) => projection switch
    {
        SWEREFProjection.sweref_99_tm => "sweref_99_tm",
        SWEREFProjection.sweref_99_12_00 => "sweref_99_1200",
        SWEREFProjection.sweref_99_13_30 => "sweref_99_1330",
        SWEREFProjection.sweref_99_14_15 => "sweref_99_1415",
        SWEREFProjection.sweref_99_15_00 => "sweref_99_1500",
        SWEREFProjection.sweref_99_15_45 => "sweref_99_1545",
        SWEREFProjection.sweref_99_16_30 => "sweref_99_1630",
        SWEREFProjection.sweref_99_17_15 => "sweref_99_1715",
        SWEREFProjection.sweref_99_18_00 => "sweref_99_1800",
        SWEREFProjection.sweref_99_18_45 => "sweref_99_1845",
        SWEREFProjection.sweref_99_20_15 => "sweref_99_2015",
        SWEREFProjection.sweref_99_21_45 => "sweref_99_2145",
        SWEREFProjection.sweref_99_23_15 => "sweref_99_2315",
        _ => projection.ToString(),
    };

    public SWEREFProjection Projection { get; set; }

    public string ProjectionString => GetProjectionString(Projection);

    public override string ToString() => $"N: {Latitude.ToString("00.000", CultureInfo.InvariantCulture)} E: {Longitude.ToString("00.000", CultureInfo.InvariantCulture)} Projection: {ProjectionString}";
}
