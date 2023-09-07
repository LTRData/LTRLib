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
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>

namespace LTRLib.Geodesy.Positions;

[Guid("3a3026e9-7baf-4faa-a19e-e59d24b16248")]
#pragma warning disable CS0618 // Type or member is obsolete
[ClassInterface(ClassInterfaceType.AutoDual)]
#pragma warning restore CS0618 // Type or member is obsolete
public class WGS84Position
    : LatLonPosition, IEquatable<WGS84Position>
{
    /// <summary>
    /// Create a new WGS84 position with empty coordinates
    /// </summary>
    public WGS84Position()
    {
    }

    /// <summary>
    /// Create a new WGS84 position with latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    public WGS84Position(double latitude, double longitude)
        : base(latitude, longitude)
    {
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <summary>
    /// Create a new WGS84 position with latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    public WGS84Position(ReadOnlySpan<char> latitude, ReadOnlySpan<char> longitude)
        : base(latitude, longitude)
    {
    }

    /// <summary>
    /// Create a new WGS84 position from strings parsed based on
    /// supplied format.
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="format"></param>
    public WGS84Position(ReadOnlySpan<char> latitude, ReadOnlySpan<char> longitude, GeoFormat format)
        : base(latitude, longitude, format)
    {
    }

    /// <summary>
    /// Create a new WGS84 position from a string containing both
    /// latitude and longitude. The string is parsed based on the 
    /// supplied format.
    /// </summary>
    /// <param name="positionString"></param>
    /// <param name="format"></param>
    public WGS84Position(ReadOnlySpan<char> positionString, GeoFormat format)
        : base(positionString, format)
    {
    }

    /// <summary>
    /// Create a new WGS84 position from a string containing both
    /// latitude and longitude.
    /// </summary>
    /// <param name="positionString"></param>
    public WGS84Position(ReadOnlySpan<char> positionString)
        : base(positionString)
    {
    }
#endif
    /// <summary>
    /// Create a new WGS84 position with latitude and longitude
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    public WGS84Position(string latitude, string longitude)
        : base(latitude, longitude)
    {
    }

    /// <summary>
    /// Create a new WGS84 position from strings parsed based on
    /// supplied format.
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="format"></param>
    public WGS84Position(string latitude, string longitude, GeoFormat format)
        : base(latitude, longitude, format)
    {
    }

    /// <summary>
    /// Create a new WGS84 position from a string containing both
    /// latitude and longitude. The string is parsed based on the 
    /// supplied format.
    /// </summary>
    /// <param name="positionString"></param>
    /// <param name="format"></param>
    public WGS84Position(string positionString, GeoFormat format)
        : base(positionString, format)
    {
    }

    /// <summary>
    /// Create a new WGS84 position from a string containing both
    /// latitude and longitude.
    /// </summary>
    /// <param name="positionString"></param>
    public WGS84Position(string positionString)
        : base(positionString)
    {
    }

    /// <summary>
    /// Construct from a degrees, minutes and seconds.
    /// </summary>
    /// <param name="lat_direction">W for west, E for east</param>
    /// <param name="lat_degrees">Latitude degrees</param>
    /// <param name="lat_minutes">Latitude minutes</param>
    /// <param name="lat_seconds">Latitude seconds</param>
    /// <param name="lon_direction">N for north, S for south</param>
    /// <param name="lon_degrees">Longitude degrees</param>
    /// <param name="lon_minutes">Longitude minutes</param>
    /// <param name="lon_seconds">Longitude seconds</param>
    public WGS84Position(char lat_direction, byte lat_degrees, byte lat_minutes, double lat_seconds, char lon_direction, byte lon_degrees, byte lon_minutes, double lon_seconds)
        : base(lat_direction, lat_degrees, lat_minutes, lat_seconds, lon_direction, lon_degrees, lon_minutes, lon_seconds)
    {
    }

    /// <summary>
    /// Construct from a degrees and minutes.
    /// </summary>
    /// <param name="lat_direction">W for west, E for east</param>
    /// <param name="lat_degrees">Latitude degrees</param>
    /// <param name="lat_minutes">Latitude minutes</param>
    /// <param name="lon_direction">N for north, S for south</param>
    /// <param name="lon_degrees">Longitude degrees</param>
    /// <param name="lon_minutes">Longitude minutes</param>
    public WGS84Position(char lat_direction, byte lat_degrees, double lat_minutes, char lon_direction, byte lon_degrees, double lon_minutes)
        : base(lat_direction, lat_degrees, lat_minutes, lon_direction, lon_degrees, lon_minutes)
    {
    }

    public override WGS84Position ToWGS84() => this;

    public override void FromWGS84(WGS84Position position)
    {
        Latitude = position.Latitude;
        Longitude = position.Longitude;
    }

    public override WGS84Position AddDistance(double distance, double bearing)
    {
        var R = EarthRadius;
        var d = distance;
        var lat1 = Latitude / AngleFactor;
        var lon1 = Longitude / AngleFactor;
        var brng = bearing / AngleFactor;

        var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(d / R) +
                                    Math.Cos(lat1) * Math.Sin(d / R) * Math.Cos(brng));
        var lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(d / R) * Math.Cos(lat1),
                                        Math.Cos(d / R) - Math.Sin(lat1) * Math.Sin(lat2));

        return new(lat2 * AngleFactor, lon2 * AngleFactor);
    }

    public override double GetDistanceThroughEarth(Position ToPosObj)
    {
        var ToPos = ToPosObj.ToWGS84();

        var degCosLat = DegCos(Latitude);

        var x1 = EarthRadius * degCosLat * DegCos(Longitude);
        var y1 = EarthRadius * DegSin(Latitude);
        var z1 = EarthRadius * degCosLat * DegSin(Longitude);

        var degCosToLat = DegCos(ToPos.Latitude);

        var x2 = EarthRadius * degCosToLat * DegCos(ToPos.Longitude);
        var y2 = EarthRadius * DegSin(ToPos.Latitude);
        var z2 = EarthRadius * degCosToLat * DegSin(ToPos.Longitude);

        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2) + Math.Pow(z2 - z1, 2));
    }

    public override double GetBearing(Position ToPosObj)
    {
        const double piDiv4 = Math.PI / 4;

        var ToPos = ToPosObj.ToWGS84();

        var f = Math.Log(Math.Tan(ToPos.Latitude / AngleFactor / 2 + piDiv4) / Math.Tan(Latitude / AngleFactor / 2 + piDiv4));
        
        var bearing = Math.Atan2(ToPos.Longitude / AngleFactor - Longitude / AngleFactor, f);
        
        if (bearing < 0)
        {
            bearing += 2 * Math.PI;
        }

        return bearing * AngleFactor;
    }

    public override bool Equals(WGS84Position? obj) => obj is not null && (obj.Latitude == Latitude) && (obj.Longitude == Longitude);

    public override bool Equals(object? obj) => obj is WGS84Position other ? Equals(other) : base.Equals(obj);

    /// <summary>
    /// Returns hash code for current position.
    /// </summary>
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
#else
    public override int GetHashCode() => new { Latitude, Longitude }.GetHashCode();
#endif

    public static bool operator ==(WGS84Position p0, WGS84Position p1) => ReferenceEquals(p0, p1) || (p0 is not null && p0.Equals(p1));

    public static bool operator !=(WGS84Position p0, WGS84Position p1) => !(p0 == p1);

    /// <summary>
    /// Calculates four corner positions of a square around this position.
    /// </summary>
    /// <param name="rad">Radius ("side") of square</param>
    /// <returns></returns>
    public WGS84Position[] CreateSquare(double rad)
    {
        // first-cut bounding box (in degrees)
        var maxLat = Latitude + AngleFactor * rad / EarthRadius;
        var minLat = Latitude - AngleFactor * rad / EarthRadius;
        // compensate for degrees longitude getting smaller with increasing latitude
        var maxLon = Longitude + AngleFactor * rad / EarthRadius / Math.Cos(Latitude / AngleFactor);
        var minLon = Longitude - AngleFactor * rad / EarthRadius / Math.Cos(Latitude / AngleFactor);

        return new[] {
                new WGS84Position {
                    Latitude = minLat,
                    Longitude = minLon
                },
                new WGS84Position {
                    Latitude = maxLat,
                    Longitude = minLon
                },
                new WGS84Position {
                    Latitude = maxLat,
                    Longitude = maxLon
                },
                new WGS84Position {
                    Latitude = minLat,
                    Longitude = maxLon
                }
            };
    }

}
