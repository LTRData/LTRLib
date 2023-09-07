﻿/*
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


using LTRLib.Extensions;
using LTRLib.Geodesy.Conversion;
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Geodesy.Positions;

[Guid("c7f1b8d8-eb34-40b8-b412-34cdaca55f05")]
[ClassInterface(ClassInterfaceType.AutoDual)]

public class RT90Position
    : Position, IEquatable<RT90Position>
{
    public double PointX { get; set; }
    public double PointY { get; set; }

    public enum RT90Projection
    {
        rt90_7_5_gon_v = 0,
        rt90_5_0_gon_v = 1,
        rt90_2_5_gon_v = 2,
        rt90_0_0_gon_v = 3,
        rt90_2_5_gon_o = 4,
        rt90_5_0_gon_o = 5
    }

    /// <summary>
    /// Create a new empty position using default projection (2.5 gon v)
    /// </summary>
    public RT90Position()
    {
        Projection = RT90Projection.rt90_2_5_gon_v;
    }

    /// <summary>
    /// Create a new position using default projection (2.5 gon v)
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
    public RT90Position(double x, double y)
    {
        PointX = x;
        PointY = y;
        Projection = RT90Projection.rt90_2_5_gon_v;
    }

    /// <summary>
    /// Create a new position using default projection (2.5 gon v)
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
    public RT90Position(float x, float y)
    {
        PointX = x;
        PointY = y;
        Projection = RT90Projection.rt90_2_5_gon_v;
    }

    /// <summary>
    /// Create a new position using default projection (2.5 gon v)
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
    public RT90Position(int x, int y)
    {
        PointX = x;
        PointY = y;
        Projection = RT90Projection.rt90_2_5_gon_v;
    }

    /// <summary>
    /// Create a new position using default projection (2.5 gon v)
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public RT90Position(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        PointX = double.Parse(x, provider: NumberFormatInfo.InvariantInfo);
        PointY = double.Parse(y, provider: NumberFormatInfo.InvariantInfo);
        Projection = RT90Projection.rt90_2_5_gon_v;
    }
#endif
    public RT90Position(string x, string y)
    {
        PointX = double.Parse(x, NumberFormatInfo.InvariantInfo);
        PointY = double.Parse(y, NumberFormatInfo.InvariantInfo);
        Projection = RT90Projection.rt90_2_5_gon_v;
    }

    /// <summary>
    /// Create a new position
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
    /// <param name="projection">RT90 projection</param>
    public RT90Position(double x, double y, RT90Projection projection)
    {
        PointX = x;
        PointY = y;
        Projection = projection;
    }

    /// <summary>
    /// Create a new position
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
    /// <param name="projection">RT90 projection</param>
    public RT90Position(float x, float y, RT90Projection projection)
    {
        PointX = x;
        PointY = y;
        Projection = projection;
    }

    /// <summary>
    /// Create a new position
    /// </summary>
    /// <param name="x">X value</param>
    /// <param name="y">Y value</param>
    /// <param name="projection">RT90 projection</param>
    public RT90Position(string x, string y, RT90Projection projection)
    {
        PointX = double.Parse(x, NumberFormatInfo.InvariantInfo);
        PointY = double.Parse(y, NumberFormatInfo.InvariantInfo);
        Projection = projection;
    }

    /// <summary>
    /// Create a RT90 position by converting a WGS84 position
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    /// <param name="rt90projection">Projection to convert to</param>
    public RT90Position(WGS84Position position, RT90Projection rt90projection)
    {
        FromWGS84(position, rt90projection);
    }

    /// <summary>
    /// Create a RT90 position by converting a WGS84 position in "RT90 2.5 gon v" projection
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    public RT90Position(WGS84Position position)
    {
        FromWGS84(position, RT90Projection.rt90_2_5_gon_v);
    }

    /// <summary>
    /// Create a RT90 position by converting a WGS84 position
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    /// <param name="rt90projection">Projection to convert to</param>
    public void FromWGS84(WGS84Position position, RT90Projection rt90projection)
    {
        var gkProjection = new GaussKreuger();
        gkProjection.swedish_params(GetProjectionString(rt90projection));
        var lat_lon = gkProjection.geodetic_to_grid(position.Latitude, position.Longitude);
        PointX = lat_lon[0];
        PointY = lat_lon[1];
        Projection = rt90projection;
    }

    /// <summary>
    /// Create a RT90 position by converting a WGS84 position in "RT90 2.5 gon v" projection
    /// </summary>
    /// <param name="position">WGS84 position to convert</param>
    public override void FromWGS84(WGS84Position position)
    {
        var gkProjection = new GaussKreuger();
        gkProjection.swedish_params(GetProjectionString(Projection));
        var lat_lon = gkProjection.geodetic_to_grid(position.Latitude, position.Longitude);
        PointX = lat_lon[0];
        PointY = lat_lon[1];
    }

    /// <summary>
    /// Convert the position to WGS84 format
    /// </summary>
    /// <returns></returns>
    public override WGS84Position ToWGS84()
    {
        var gkProjection = new GaussKreuger();
        gkProjection.swedish_params(ProjectionString);
        var lat_lon = gkProjection.grid_to_geodetic(PointX, PointY);

        var newPos = new WGS84Position()
        {
            Latitude = lat_lon[0],
            Longitude = lat_lon[1],
        };

        return newPos;
    }

    private static string GetProjectionString(RT90Projection projection) => projection switch
    {
        RT90Projection.rt90_7_5_gon_v => "rt90_7.5_gon_v",
        RT90Projection.rt90_5_0_gon_v => "rt90_5.0_gon_v",
        RT90Projection.rt90_2_5_gon_v => "rt90_2.5_gon_v",
        RT90Projection.rt90_0_0_gon_v => "rt90_0.0_gon_v",
        RT90Projection.rt90_2_5_gon_o => "rt90_2.5_gon_o",
        RT90Projection.rt90_5_0_gon_o => "rt90_5.0_gon_o",
        _ => "rt90_2.5_gon_v",
    };

    public RT90Projection Projection { get; set; }
    public string ProjectionString => GetProjectionString(Projection);

    public string X
    {
        get => PointX.ToString("0.0", NumberFormatInfo.InvariantInfo);
        set => PointX = double.Parse(value, NumberFormatInfo.InvariantInfo);
    }

    public string Y
    {
        get => PointY.ToString("0.0", NumberFormatInfo.InvariantInfo);
        set => PointY = double.Parse(value, NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator RT90Position(PointF point) => new(point.X, point.Y);

    public static implicit operator RT90Position(Point point) => new(point.X, point.Y);

    public static implicit operator PointF(RT90Position point) => new((float)point.PointX, (float)point.PointY);

    public static implicit operator Point(RT90Position point) => new((int)point.PointX, (int)point.PointY);

    public override string ToString() => $"X={X},Y={Y}";

    public bool Equals(RT90Position? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.Projection != Projection)
        {
            return base.Equals(obj);
        }

        return obj.PointX == PointX && obj.PointY == PointY;
    }

    public static bool operator ==(RT90Position p0, RT90Position p1) => p0.Equals(p1);

    public static bool operator !=(RT90Position p0, RT90Position p1) => !p0.Equals(p1);

    public override bool Equals(Position? obj)
    {
        if (obj is RT90Position cmpobj)
        {
            return Equals(cmpobj);
        }

        return base.Equals(obj);
    }

    public override bool Equals(object? obj)
    {
        if (obj is RT90Position cmpobj)
        {
            return Equals(cmpobj);
        }

        return base.Equals(obj);
    }

#if NETCOREAPP || NETSTANDARD || NET461_OR_GREATER
    public override int GetHashCode() => HashCode.Combine(PointX, PointY);
#else
    public override int GetHashCode() => (int)(PointX * 10) ^ (int)(PointY * 10);
#endif

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public override void FromWKT(ReadOnlySpan<char> str)
    {
        if (str.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
        {
            str = str.Slice("POINT".Length).Trim().TrimStart('(').TrimEnd(')');
        }

        var xy = str.Trim().ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        X = xy[0];
        Y = xy[1];
    }
#else
    public override void FromWKT(string str)
    {
        if (str.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
        {
            str = str.Substring("POINT".Length).Trim().TrimStart('(').TrimEnd(')');
        }

        var xy = str.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        X = xy[0];
        Y = xy[1];
    }
#endif

    public override string ToWKT() => $"POINT ({X} {Y})";
}
