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

using LTRLib.Extensions;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Geodesy.Positions;

/// <summary>
/// Base class for positions based on latitude and longitude.
/// </summary>
[Guid("e6c0a75f-7784-495f-beba-6924de3c5840")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public abstract class LatLonPosition
    : Position
{
    public enum GeoFormat
    {
        Degrees,
        DegreesMinutes,
        DegreesMinutesSeconds
    }

    public enum GridSquarePosition
    {
        SouthWest,
        Center,
        NorthEast
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    protected LatLonPosition(double lat, double lon)
    {
        Latitude = lat;
        Longitude = lon;
    }

    private static readonly char[] _AA00aa00 = { 'A', 'A', '0', '0', 'a', 'a', '0', '0' };
    private static readonly char[] _LL44ll44 = { 'L', 'L', '4', '4', 'l', 'l', '4', '4' };
    private static readonly char[] _XX99xx99 = { 'X', 'X', '9', '9', 'x', 'x', '9', '9' };

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    protected LatLonPosition(ReadOnlySpan<char> lat_str, ReadOnlySpan<char> long_str, GeoFormat format)
    {
        SetLatitudeFromString(lat_str, format);
        SetLongitudeFromString(long_str, format);
    }

    protected LatLonPosition(ReadOnlySpan<char> lat_str, ReadOnlySpan<char> long_str)
    {
        SetLatitudeFromString(lat_str);
        SetLongitudeFromString(long_str);
    }

    protected LatLonPosition(string lat_str, string long_str, GeoFormat format)
    {
        SetLatitudeFromString(lat_str, format);
        SetLongitudeFromString(long_str, format);
    }

    protected LatLonPosition(string lat_str, string long_str)
    {
        SetLatitudeFromString(lat_str);
        SetLongitudeFromString(long_str);
    }

    protected LatLonPosition(ReadOnlySpan<char> lat_lon_str, GeoFormat format)
    {
        var lat_lon = lat_lon_str.Trim().ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (lat_lon.Length != 2)
        {
            throw new ArgumentException("Invalid position string", nameof(lat_lon_str));
        }

        SetLatitudeFromString(lat_lon[0], format);
        SetLongitudeFromString(lat_lon[1], format);
    }

    protected LatLonPosition(string lat_lon_str, GeoFormat format)
        : this(lat_lon_str.AsSpan(), format)
    { }

    protected LatLonPosition(ReadOnlySpan<char> lat_lon_str)
    {
        SetFromString(lat_lon_str);
    }

    protected LatLonPosition(string lat_lon_str)
    {
        SetFromString(lat_lon_str);
    }

    public void SetFromString(ReadOnlySpan<char> lat_lon_str)
    {
        lat_lon_str = lat_lon_str.Trim();

        string[] lat_lon;

        if (lat_lon_str.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
        {
            lat_lon_str = lat_lon_str.Slice("POINT".Length).Trim().TrimStart('(').TrimEnd(')');

            var xy = lat_lon_str.Trim().ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            SetLongitudeFromString(xy[0], GeoFormat.Degrees);
            SetLatitudeFromString(xy[1], GeoFormat.Degrees);
        }
        else if (lat_lon_str.IndexOf(',') >= 0)
        {
            lat_lon = lat_lon_str.Trim().ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            SetLatitudeFromString(lat_lon[0]);
            SetLongitudeFromString(lat_lon[1]);
        }
        else if (lat_lon_str.IndexOf(' ') >= 0)
        {
            lat_lon = lat_lon_str.Trim().ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            SetLatitudeFromString(lat_lon[0]);
            SetLongitudeFromString(lat_lon[1]);
        }
        else
        {
            SetMaidenhead(lat_lon_str, GridSquarePosition.Center);
        }
    }

    public void SetFromString(string lat_lon_str) =>
        SetFromString(lat_lon_str.AsSpan());

    public void SetMaidenhead(ReadOnlySpan<char> maidenhead, GridSquarePosition squarerelativepos)
    {
        switch (maidenhead.Length)
        {
            case 8:
                if (char.IsDigit(maidenhead[6]) &&
                    char.IsDigit(maidenhead[7]))
                {
                    goto case 6;
                }
                else
                {
                    goto default;
                }

            case 6:
                if (char.IsLetter(maidenhead[4]) &&
                    char.IsLetter(maidenhead[5]))
                {
                    goto case 4;
                }
                else
                {
                    goto default;
                }

            case 4:
                if (char.IsDigit(maidenhead[2]) &&
                    char.IsDigit(maidenhead[3]))
                {
                    goto case 2;
                }
                else
                {
                    goto default;
                }

            case 2:
                if (char.IsLetter(maidenhead[0]) &&
                    char.IsLetter(maidenhead[1]))
                {
                    break;
                }
                else
                {
                    goto default;
                }

            default:
                throw new ArgumentException("Invalid position string", nameof(maidenhead));
        }

        var lon = 0d;
        var lat = 0d;

        var sourcechars = squarerelativepos switch
        {
            GridSquarePosition.SouthWest => _AA00aa00,
            GridSquarePosition.Center => _LL44ll44,
            GridSquarePosition.NorthEast => _XX99xx99,
            _ => throw new ArgumentException("Invalid grid square position", nameof(squarerelativepos)),
        };

        Span<char> chrs = stackalloc char[8];
        sourcechars.CopyTo(chrs);
        maidenhead.CopyTo(chrs);

        lon += (char.ToUpperInvariant(chrs[0]) - 'A') * 20;
        lat += (char.ToUpperInvariant(chrs[1]) - 'A') * 10;

        lon += (chrs[2] - '0') * 2;
        lat += chrs[3] - '0';

        lon += (char.ToLowerInvariant(chrs[4]) - 'a') * 5.0 / 60;
        lat += (char.ToLowerInvariant(chrs[5]) - 'a') * 2.5 / 60;

        lon += (chrs[6] - '0') * 0.5 / 60;
        lat += (chrs[7] - '0') * 0.25 / 60;

        switch (squarerelativepos)
        {
            case GridSquarePosition.Center:
                lon += 0.625 / 3600;
                lat += 0.3125 / 3600;
                break;

            case GridSquarePosition.NorthEast:
                lon += 1.25 / 3600;
                lat += 0.625 / 3600;
                break;
        }

        Longitude = lon - 180;
        Latitude = lat - 90;
    }

    public void SetMaidenhead(string maidenhead, GridSquarePosition squarerelativepos) =>
        SetMaidenhead(maidenhead, squarerelativepos);

    /// <summary>
    /// Set the latitude value from a string. The string is
    /// parsed based on given format
    /// </summary>
    /// <param name="value">String represenation of a latitude value</param>
    /// <param name="format">Coordinate format in the string</param>
    public void SetLatitudeFromString(ReadOnlySpan<char> value, GeoFormat format)
    {
        value = value.Trim();

        Latitude = format switch
        {
            GeoFormat.DegreesMinutes => ParseValueFromDmString(value, 'S'),
            GeoFormat.DegreesMinutesSeconds => ParseValueFromDmsString(value, 'S'),
            GeoFormat.Degrees => double.Parse(value, provider: NumberFormatInfo.InvariantInfo),
            _ => throw new ArgumentException("Invalid GeoFormat", nameof(format)),
        };
    }

    public void SetLatitudeFromString(string value, GeoFormat format) =>
        SetLatitudeFromString(value.AsSpan(), format);

    /// <summary>
    /// Set the latitude value from a string.
    /// </summary>
    /// <param name="value">String representation of a latitude value</param>
    public void SetLatitudeFromString(ReadOnlySpan<char> value)
    {
        value = value.Trim();

        if (value.IndexOf('"') >= 0)
        {
            Latitude = ParseValueFromDmsString(value, 'S');
        }
        else if (value.IndexOf('\'') >= 0)
        {
            Latitude = ParseValueFromDmString(value, 'S');
        }
        else
        {
            Latitude = double.Parse(value, provider: NumberFormatInfo.InvariantInfo);
        }
    }

    public void SetLatitudeFromString(string value) =>
        SetLatitudeFromString(value.AsSpan());

    /// <summary>
    /// Set the longitude value from a string. The string is
    /// parsed based on given format
    /// </summary>
    /// <param name="value">String representation of a longitude value</param>
    /// <param name="format">Coordinate format in the string</param>
    public void SetLongitudeFromString(ReadOnlySpan<char> value, GeoFormat format) => Longitude = format switch
    {
        GeoFormat.DegreesMinutes => ParseValueFromDmString(value, 'W'),
        GeoFormat.DegreesMinutesSeconds => ParseValueFromDmsString(value, 'W'),
        GeoFormat.Degrees => double.Parse(value, provider: NumberFormatInfo.InvariantInfo),
        _ => throw new ArgumentException("Invalid GeoFormat", nameof(format)),
    };

    public void SetLongitudeFromString(string value, GeoFormat format) =>
        SetLatitudeFromString(value.AsSpan(), format);

    /// <summary>
    /// Set the longitude value from a string.
    /// </summary>
    /// <param name="value">String representation of a longitude value</param>
    public void SetLongitudeFromString(ReadOnlySpan<char> value)
    {
        value = value.Trim();

        if (value.IndexOf('"') >= 0)
        {
            Longitude = ParseValueFromDmsString(value, 'S');
        }
        else if (value.IndexOf('\'') >= 0)
        {
            Longitude = ParseValueFromDmString(value, 'S');
        }
        else
        {
            Longitude = double.Parse(value, provider: NumberFormatInfo.InvariantInfo);
        }
    }

    public void SetLongitudeFromString(string value) =>
        SetLongitudeFromString(value.AsSpan());

#else
    protected LatLonPosition(string lat_str, string long_str, GeoFormat format)
    {
        SetLatitudeFromString(lat_str, format);
        SetLongitudeFromString(long_str, format);
    }

    protected LatLonPosition(string lat_str, string long_str)
    {
        SetLatitudeFromString(lat_str);
        SetLongitudeFromString(long_str);
    }

    protected LatLonPosition(string lat_lon_str, GeoFormat format)
    {
        var lat_lon = lat_lon_str.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (lat_lon.Length != 2)
        {
            throw new ArgumentException("Invalid position string", nameof(lat_lon_str));
        }

        SetLatitudeFromString(lat_lon[0], format);
        SetLongitudeFromString(lat_lon[1], format);
    }

    protected LatLonPosition(string lat_lon_str)
    {
        SetFromString(lat_lon_str);
    }

    public void SetFromString(string lat_lon_str)
    {
        lat_lon_str = lat_lon_str.Trim();

        string[] lat_lon;

        if (lat_lon_str.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
        {
            lat_lon_str = lat_lon_str.Substring("POINT".Length).Trim().TrimStart('(').TrimEnd(')');

            var xy = lat_lon_str.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            SetLongitudeFromString(xy[0], GeoFormat.Degrees);
            SetLatitudeFromString(xy[1], GeoFormat.Degrees);
        }
        else if (lat_lon_str.IndexOf(',') >= 0)
        {
            lat_lon = lat_lon_str.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
            SetLatitudeFromString(lat_lon[0]);
            SetLongitudeFromString(lat_lon[1]);
        }
        else if (lat_lon_str.IndexOf(' ') >= 0)
        {
            lat_lon = lat_lon_str.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            SetLatitudeFromString(lat_lon[0]);
            SetLongitudeFromString(lat_lon[1]);
        }
        else
        {
            SetMaidenhead(lat_lon_str, GridSquarePosition.Center);
        }
    }

    public void SetMaidenhead(string maidenhead, GridSquarePosition squarerelativepos)
    {
        switch (maidenhead.Length)
        {
            case 8:
                if (char.IsDigit(maidenhead, 6) &&
                    char.IsDigit(maidenhead, 7))
                {
                    goto case 6;
                }
                else
                {
                    goto default;
                }

            case 6:
                if (char.IsLetter(maidenhead, 4) &&
                    char.IsLetter(maidenhead, 5))
                {
                    goto case 4;
                }
                else
                {
                    goto default;
                }

            case 4:
                if (char.IsDigit(maidenhead, 2) &&
                    char.IsDigit(maidenhead, 3))
                {
                    goto case 2;
                }
                else
                {
                    goto default;
                }

            case 2:
                if (char.IsLetter(maidenhead, 0) &&
                    char.IsLetter(maidenhead, 1))
                {
                    break;
                }
                else
                {
                    goto default;
                }

            default:
                throw new ArgumentException("Invalid position string", nameof(maidenhead));
        }

        var lon = 0d;
        var lat = 0d;

        var chrs = squarerelativepos switch
        {
            GridSquarePosition.SouthWest => _AA00aa00,
            GridSquarePosition.Center => _LL44ll44,
            GridSquarePosition.NorthEast => _XX99xx99,
            _ => throw new ArgumentException("Invalid grid square position", nameof(squarerelativepos)),
        };

        maidenhead.CopyTo(0, chrs, 0, maidenhead.Length);

        lon += (char.ToUpperInvariant(chrs[0]) - 'A') * 20;
        lat += (char.ToUpperInvariant(chrs[1]) - 'A') * 10;

        lon += (chrs[2] - '0') * 2;
        lat += chrs[3] - '0';

        lon += (char.ToLowerInvariant(chrs[4]) - 'a') * 5.0 / 60;
        lat += (char.ToLowerInvariant(chrs[5]) - 'a') * 2.5 / 60;

        lon += (chrs[6] - '0') * 0.5 / 60;
        lat += (chrs[7] - '0') * 0.25 / 60;

        switch (squarerelativepos)
        {
            case GridSquarePosition.Center:
                lon += 0.625 / 3600;
                lat += 0.3125 / 3600;
                break;

            case GridSquarePosition.NorthEast:
                lon += 1.25 / 3600;
                lat += 0.625 / 3600;
                break;
        }

        Longitude = lon - 180;
        Latitude = lat - 90;
    }

    /// <summary>
    /// Set the latitude value from a string. The string is
    /// parsed based on given format
    /// </summary>
    /// <param name="value">String represenation of a latitude value</param>
    /// <param name="format">Coordinate format in the string</param>
    public void SetLatitudeFromString(string value, GeoFormat format)
    {
        value = value.Trim();

        Latitude = format switch
        {
            GeoFormat.DegreesMinutes => ParseValueFromDmString(value, 'S'),
            GeoFormat.DegreesMinutesSeconds => ParseValueFromDmsString(value, 'S'),
            GeoFormat.Degrees => double.Parse(value, NumberFormatInfo.InvariantInfo),
            _ => throw new ArgumentException("Invalid GeoFormat", nameof(format)),
        };
    }


    /// <summary>
    /// Set the latitude value from a string.
    /// </summary>
    /// <param name="value">String representation of a latitude value</param>
    public void SetLatitudeFromString(string value)
    {
        value = value.Trim();

        if (value.IndexOf('"') >= 0)
        {
            Latitude = ParseValueFromDmsString(value, 'S');
        }
        else if (value.IndexOf('\'') >= 0)
        {
            Latitude = ParseValueFromDmString(value, 'S');
        }
        else
        {
            Latitude = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }
    }

    /// <summary>
    /// Set the longitude value from a string. The string is
    /// parsed based on given format
    /// </summary>
    /// <param name="value">String representation of a longitude value</param>
    /// <param name="format">Coordinate format in the string</param>
    public void SetLongitudeFromString(string value, GeoFormat format) => Longitude = format switch
    {
        GeoFormat.DegreesMinutes => ParseValueFromDmString(value, 'W'),
        GeoFormat.DegreesMinutesSeconds => ParseValueFromDmsString(value, 'W'),
        GeoFormat.Degrees => double.Parse(value, NumberFormatInfo.InvariantInfo),
        _ => throw new ArgumentException("Invalid GeoFormat", nameof(format)),
    };

    /// <summary>
    /// Set the longitude value from a string.
    /// </summary>
    /// <param name="value">String representation of a longitude value</param>
    public void SetLongitudeFromString(string value)
    {
        value = value.Trim();

        if (value.IndexOf('"') >= 0)
        {
            Longitude = ParseValueFromDmsString(value, 'S');
        }
        else if (value.IndexOf('\'') >= 0)
        {
            Longitude = ParseValueFromDmString(value, 'S');
        }
        else
        {
            Longitude = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }
    }
#endif

    protected LatLonPosition()
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
    public LatLonPosition(char lat_direction, byte lat_degrees, byte lat_minutes, double lat_seconds, char lon_direction, byte lon_degrees, byte lon_minutes, double lon_seconds)
    {
        SetLatitudeFromDms(lat_direction, lat_degrees, lat_minutes, lat_seconds);
        SetLongitudeFromDms(lon_direction, lon_degrees, lon_minutes, lon_seconds);
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
    public LatLonPosition(char lat_direction, byte lat_degrees, double lat_minutes, char lon_direction, byte lon_degrees, double lon_minutes)
    {
        SetLatitudeFromDm(lat_direction, lat_degrees, lat_minutes);
        SetLongitudeFromDm(lon_direction, lon_degrees, lon_minutes);
    }

    /// <summary>
    /// Set the latitude value from a degrees, minutes and seconds.
    /// </summary>
    /// <param name="direction">N for north, S for south</param>
    /// <param name="degrees">Degrees</param>
    /// <param name="minutes">Minutes</param>
    /// <param name="seconds">Seconds</param>
    public void SetLatitudeFromDms(char direction, byte degrees, byte minutes, double seconds) =>
        Latitude = ParseValueFromDms('S', direction, degrees, minutes, seconds);

    /// <summary>
    /// Set the longitude value from a degrees and minutes.
    /// </summary>
    /// <param name="direction">N for north, S for south</param>
    /// <param name="degrees">Degrees</param>
    /// <param name="minutes">Minutes</param>
    public void SetLatitudeFromDm(char direction, byte degrees, double minutes) =>
        Latitude = ParseValueFromDm('S', direction, degrees, minutes);

    /// <summary>
    /// Set the longitude value from a degrees, minutes and seconds.
    /// </summary>
    /// <param name="direction">W for west, E for east</param>
    /// <param name="degrees">Degrees</param>
    /// <param name="minutes">Minutes</param>
    /// <param name="seconds">Seconds</param>
    public void SetLongitudeFromDms(char direction, byte degrees, byte minutes, double seconds) =>
        Longitude = ParseValueFromDms('W', direction, degrees, minutes, seconds);

    /// <summary>
    /// Set the longitude value from a degrees and minutes.
    /// </summary>
    /// <param name="direction">W for west, E for east</param>
    /// <param name="degrees">Degrees</param>
    /// <param name="minutes">Minutes</param>
    public void SetLongitudeFromDm(char direction, byte degrees, double minutes) =>
        Longitude = ParseValueFromDm('W', direction, degrees, minutes);

    /// <summary>
    /// Returns a string representation in the given format
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string LatitudeToString(GeoFormat format) => format switch
    {
        GeoFormat.DegreesMinutes => ConvToDmString(Latitude, 'N', 'S'),
        GeoFormat.DegreesMinutesSeconds => ConvToDmsString(Latitude, 'N', 'S'),
        GeoFormat.Degrees => Latitude.ToString("0.0000000", NumberFormatInfo.InvariantInfo),
        _ => throw new ArgumentException("Invalid GeoFormat", nameof(format)),
    };

    /// <summary>
    /// Returns a string representation in the given format
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public string LongitudeToString(GeoFormat format) => format switch
    {
        GeoFormat.DegreesMinutes => ConvToDmString(Longitude, 'E', 'W'),
        GeoFormat.DegreesMinutesSeconds => ConvToDmsString(Longitude, 'E', 'W'),
        GeoFormat.Degrees => Longitude.ToString("0.0000000", NumberFormatInfo.InvariantInfo),
        _ => throw new ArgumentException("Invalid GeoFormat", nameof(format)),
    };

    private static string ConvToDmString(double value, char positiveValue, char negativeValue)
    {
        if (value == double.MinValue)
        {
            return "";
        }

        var degrees = Math.Floor(Math.Abs(value));
        var minutes = (Math.Abs(value) - degrees) * 60;

        return
            string.Format(CultureInfo.InvariantCulture, "{0} {1:00}º {2:0.00000}'",
            value >= 0 ? positiveValue : negativeValue,
            degrees,
            Math.Floor(minutes * 10000) / 10000);
    }

    private static string ConvToDmsString(double value, char positiveValue, char negativeValue)
    {
        if (value == double.MinValue)
        {
            return "";
        }

        var degrees = Math.Floor(Math.Abs(value));
        var minutes = Math.Floor((Math.Abs(value) - degrees) * 60);
        var seconds = (Math.Abs(value) - degrees - minutes / 60) * 3600;

        return
            string.Format(CultureInfo.InvariantCulture, "{0} {1:00}º {2:00}' {3:0.00000}\"",
            value >= 0 ? positiveValue : negativeValue,
            degrees,
            minutes,
            seconds);
    }


    public override string ToString() => ToString(GeoFormat.Degrees);

    public virtual string ToString(GeoFormat format)
    {
        return string.Concat(
            LatitudeToString(format),
            ",",
            LongitudeToString(format));
    }

    public string ToMaidenhead()
    {
        var lon = Longitude + 180;
        var lat = Latitude + 90;

        var field_lon = Math.Floor(lon / 20);
        lon -= field_lon * 20;
        var square_lon = Math.Floor(lon / 2);
        lon -= square_lon * 2;
        var subsquare_lon = Math.Floor(lon * 60 / 5);
        lon -= subsquare_lon * 5 / 60;
        var extsquare_lon = Math.Floor(lon * 60 / 0.5);
        //lon -= extsquare_lon * 0.5 / 60;

        var field_lat = Math.Floor(lat / 10);
        lat -= field_lat * 10;
        var square_lat = Math.Floor(lat);
        lat -= square_lat;
        var subsquare_lat = Math.Floor(lat * 60 / 2.5);
        lat -= subsquare_lat * 2.5 / 60;
        var extsquare_lat = Math.Floor(lat * 60 / 0.25);
        //lat -= extsquare_lat * 0.25 / 60;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        return string.Create<object>(8, null!, (span, _) =>
        {
            span[0] = (char)('A' + field_lon);
            span[1] = (char)('A' + field_lat);
            span[2] = (char)('0' + square_lon);
            span[3] = (char)('0' + square_lat);
            span[4] = (char)('A' + subsquare_lon);
            span[5] = (char)('A' + subsquare_lat);
            span[6] = (char)('0' + extsquare_lon);
            span[7] = (char)('0' + extsquare_lat);
        });
#else
        var span = new char[8];
        span[0] = (char)('A' + field_lon);
        span[1] = (char)('A' + field_lat);
        span[2] = (char)('0' + square_lon);
        span[3] = (char)('0' + square_lat);
        span[4] = (char)('A' + subsquare_lon);
        span[5] = (char)('A' + subsquare_lat);
        span[6] = (char)('0' + extsquare_lon);
        span[7] = (char)('0' + extsquare_lat);

        return new string(span);
#endif
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override void FromWKT(ReadOnlySpan<char> str) => SetFromString(str);
#endif
    public override void FromWKT(string str) => SetFromString(str);


    public override string ToWKT() => $"POINT ({LongitudeToString(GeoFormat.Degrees)} {LatitudeToString(GeoFormat.Degrees)})";
}
