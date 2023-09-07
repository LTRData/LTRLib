/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Geodesy.Positions;

/// <summary>
/// Base class for position in any positioning system.
/// </summary>
[Guid("18869e1b-ae95-4774-a34c-16d91e18ca48")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public abstract class Position
    : IEquatable<Position>, IEquatable<WGS84Position>, IXmlSerializable, ICloneable
{
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    protected static double ParseValueFromDmString(ReadOnlySpan<char> value, char negativechar)
    {
        if (value.IsWhiteSpace())
        {
            return double.MinValue;
        }

        var direction = value[0];
        value = value.Slice(1).Trim();

        var degrees = value.Slice(0, value.IndexOfAny('º', '°'));
        value = value.Slice(value.IndexOfAny('º', '°') + 1).Trim();

        var minutes = value.Slice(0, value.IndexOf('\''));

        return ParseValueFromDmString(negativechar, direction, degrees, minutes);
    }

    protected static double ParseValueFromDmsString(ReadOnlySpan<char> value, char negativechar)
    {
        if (value.IsWhiteSpace())
        {
            return double.MinValue;
        }

        var direction = value[0];
        value = value.Slice(1).Trim();

        var degrees = value.Slice(0, value.IndexOfAny('º', '°'));
        value = value.Slice(value.IndexOfAny('º', '°') + 1).Trim();

        var minutes = value.Slice(0, value.IndexOf("'"));
        value = value.Slice(value.IndexOf("'") + 1).Trim();

        var seconds = value.Slice(0, value.IndexOf("\""));

        return ParseValueFromDmsString(negativechar, direction, degrees, minutes, seconds);
    }
#else
    protected static double ParseValueFromDmString(string value, char negativechar)
    {
        if (string.IsNullOrEmpty(value))
        {
            return double.MinValue;
        }

        var direction = value[0];
        value = value.Substring(1).Trim();

        var degrees = value.Substring(0, value.IndexOfAny(DegreeChars));
        value = value.Substring(value.IndexOfAny(DegreeChars) + 1).Trim();

        var minutes = value.Substring(0, value.IndexOf("'"));

        return ParseValueFromDmString(negativechar, direction, degrees, minutes);
    }

    private static readonly char[] DegreeChars = { 'º', '°' };

    protected static double ParseValueFromDmsString(string value, char negativechar)
    {
        if (string.IsNullOrEmpty(value))
        {
            return double.MinValue;
        }

        var direction = value[0];
        value = value.Substring(1).Trim();

        var degrees = value.Substring(0, value.IndexOfAny(DegreeChars));
        value = value.Substring(value.IndexOfAny(DegreeChars) + 1).Trim();

        var minutes = value.Substring(0, value.IndexOf("'"));
        value = value.Substring(value.IndexOf("'") + 1).Trim();

        var seconds = value.Substring(0, value.IndexOf("\""));

        return ParseValueFromDmsString(negativechar, direction, degrees, minutes, seconds);
    }
#endif

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    protected static double ParseValueFromDmString(char negativechar, char direction, ReadOnlySpan<char> degrees, ReadOnlySpan<char> minutes) =>
        ParseValueFromDm(negativechar, direction, byte.Parse(degrees), double.Parse(minutes, provider: NumberFormatInfo.InvariantInfo));

    protected static double ParseValueFromDmsString(char negativechar, char direction, ReadOnlySpan<char> degrees, ReadOnlySpan<char> minutes, ReadOnlySpan<char> seconds) =>
        ParseValueFromDms(negativechar, direction, byte.Parse(degrees), byte.Parse(minutes), double.Parse(seconds, provider: NumberFormatInfo.InvariantInfo));
#else
    protected static double ParseValueFromDmString(char negativechar, char direction, string degrees, string minutes) =>
        ParseValueFromDm(negativechar, direction, byte.Parse(degrees), double.Parse(minutes, NumberFormatInfo.InvariantInfo));

    protected static double ParseValueFromDmsString(char negativechar, char direction, string degrees, string minutes, string seconds) =>
        ParseValueFromDms(negativechar, direction, byte.Parse(degrees), byte.Parse(minutes), double.Parse(seconds, NumberFormatInfo.InvariantInfo));
#endif

    protected static double ParseValueFromDm(char negativechar, char direction, byte degrees, double minutes)
    {
        double retVal = degrees;
        retVal += minutes / 60d;

        if (retVal > 90)
        {
            return double.MinValue;
        }

        if (direction == negativechar || direction == '-')
        {
            retVal *= -1;
        }

        return retVal;
    }

    protected static double ParseValueFromDms(char negativechar, char direction, byte degrees, byte minutes, double seconds)
    {
        double retVal = degrees;
        retVal += minutes / 60d;
        retVal += seconds / 3600d;

        if (retVal > 90)
        {
            return double.MinValue;
        }

        if (direction == negativechar || direction == '-')
        {
            retVal *= -1;
        }

        return retVal;
    }

    /// <summary>
    /// Conversion factor between degrees and radians.
    /// </summary>
    public const double AngleFactor = 180 / Math.PI;

    /// <summary>
    /// Earth radius.
    /// </summary>
    public double EarthRadius = 6371008.7714; // 6394121.6; // 6367442.5;

    /// <summary>
    /// Sine function with angle measured in degrees.
    /// </summary>
    protected static double DegSin(double degrees) => Math.Sin(degrees / AngleFactor);

    /// <summary>
    /// Cosine function with angle measured in degrees.
    /// </summary>
    protected static double DegCos(double degrees) => Math.Cos(degrees / AngleFactor);

    /// <summary>
    /// Converts shortest distance between to positions on earth to distance at surface level.
    /// </summary>
    public static double GetSurfaceDistance(double DistanceThroughEarth, double EarthRadius) => EarthRadius * 2 * Math.Asin(DistanceThroughEarth / 2 / EarthRadius);

    /// <summary>
    /// Specifies whether position objects either reference same object or if of same type whether they contain
    /// same positioning data.
    /// </summary>
    /// <param name="obj">Object to compare to.</param>

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, this))
        {
            return true;
        }

        if (obj is null || !ReferenceEquals(obj.GetType(), GetType()))
        {
            return false;
        }

        if (obj is Position cmpobj)
        {
            return Equals(cmpobj);
        }

        return false;
    }

    /// <summary>
    /// Specifies whether position objects point to same position.
    /// </summary>
    /// <param name="obj">Object to compare to.</param>

    public virtual bool Equals(Position? obj) => obj is not null && Equals(obj.ToWGS84());

    /// <summary>
    /// Specifies whether position objects point to same position.
    /// </summary>
    /// <param name="obj">Object to compare to.</param>

    public virtual bool Equals(WGS84Position? obj) => ToWGS84().Equals(obj);

    /// <summary>
    /// Builds a string representation of current position.
    /// </summary>

    public static bool operator ==(Position p0, Position p1) => p0.Equals(p1);

    public static bool operator !=(Position p0, Position p1) => !p0.Equals(p1);

    public static bool operator ==(Position p0, WGS84Position p1) => p0.Equals(p1);

    public static bool operator !=(Position p0, WGS84Position p1) => !p0.Equals(p1);

    public abstract override string ToString();

    /// <summary>
    /// Returns hash code for current position.
    /// </summary>

    public override int GetHashCode() => ToWGS84().GetHashCode();

    /// <summary>
    /// Converts this position to a latitude and longitude based position in the WGS84 positioning system.
    /// </summary>
    public abstract WGS84Position ToWGS84();

    /// <summary>
    /// Converts a WGS84 position to positioning system represented by class of this instance.
    /// </summary>
    public abstract void FromWGS84(WGS84Position position);

    /// <summary>
    /// Finds WGS84 position at a certain distance and bearing away from this instance.
    /// </summary>
    /// <param name="distance">Distance in meters</param>
    /// <param name="bearing">Bearing in degrees</param>
    /// <returns></returns>
    public virtual WGS84Position AddDistance(double distance, double bearing) => ToWGS84().AddDistance(distance, bearing);

    /// <summary>
    /// Calculates direct distance in meters between this and another point on earth's surface.
    /// </summary>
    /// <param name="ToPos">The other position to calculate distance to from this position.</param>
    /// <returns>Distance in meters.</returns>
    public virtual double GetDistanceThroughEarth(Position ToPos) => ToWGS84().GetDistanceThroughEarth(ToPos);

    /// <summary>
    /// Calculates distance in meters along surface of earth between this and another point on earth's surface.
    /// </summary>
    /// <param name="ToPos">The other position to calculate distance to from this position.</param>
    /// <returns>Distance in meters.</returns>
    public virtual double GetSurfaceDistance(Position ToPos) => GetSurfaceDistance(GetDistanceThroughEarth(ToPos), EarthRadius);

    /// <summary>
    /// Calculates distance points according to WWL REG 1 rules (111.2 km/degree, 1 point per started km).
    /// </summary>
    /// <param name="ToPos">The other position to calculate distance to from this position.</param>
    /// <returns>Distance in points.</returns>
    public virtual int GetWWLDistancePoints(Position ToPos)
    {
        var pos1 = ToWGS84();
        var pos2 = ToPos.ToWGS84();

        var Bm = pos1.Latitude / AngleFactor;
        var Lm = pos1.Longitude / AngleFactor;
        
        var Bn = pos2.Latitude / AngleFactor;
        var Ln = pos2.Longitude / AngleFactor;

        var distance = 111.2 * AngleFactor * Math.Acos(Math.Sin(Bm) * Math.Sin(Bn) + Math.Cos(Bm) * Math.Cos(Bn) * Math.Cos(Lm - Ln));

        var points = (int)Math.Truncate(distance + 1);

        return points;
    }

    /// <summary>
    /// Calculates bearing in degrees from this position to another position on earth's surface.
    /// </summary>
    /// <param name="ToPos">The other position to calculate bearing to from this position.</param>
    /// <returns>Bearing in degrees.</returns>
    public virtual double GetBearing(Position ToPos) => ToWGS84().GetBearing(ToPos);


    object ICloneable.Clone() => Clone();

    public virtual Position Clone() => (Position)MemberwiseClone();

    public virtual XmlSchema? GetSchema() => null;

    public virtual void ReadXml(XmlReader reader) => FromWKT(reader.ReadElementContentAsString());

    public virtual void WriteXml(XmlWriter writer) => writer.WriteRaw(ToWKT());

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public abstract void FromWKT(ReadOnlySpan<char> wkt);

    public virtual void FromWKT(string wkt) => FromWKT(wkt.AsSpan());
#else
    public abstract void FromWKT(string wkt);
#endif

    public abstract string ToWKT();
}
