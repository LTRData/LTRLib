/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using static System.Math;

namespace LTRLib.Geodesy.Conversion;

using Positions;

public static class Calculations
{
    public static double RadiansToDegrees(double v) => v * 180 / PI;

    public static double DegreesToRadians(double v) => v * PI / 180;

    public static readonly double CosZenith = Cos(90.833 / 180 * PI);
    public static readonly double Milliseconds12Hours = TimeSpan.FromHours(12).TotalMilliseconds;
    public static readonly double MillisecondsPerRadian = Milliseconds12Hours / PI;

    public static double NormalizeDeg(double d)
    {
        d = Mod(d, 360.0);
        return d < 0 ? d + 360.0 : d;
    }

    public static double Mod(double a, double n) => a - n * Floor(a / n);

    /// <summary> Astronomical Julian Day for a given UTC time. </summary>
    public static double JulianDay(DateTime utc)
    {
        var Y = utc.Year;
        var M = utc.Month;
        var D = utc.Day + utc.TimeOfDay.TotalSeconds / 86400.0;

        if (M <= 2) { Y -= 1; M += 12; }

        var A = Y / 100;
        var B = 2 - A + A / 4;

        var jd = Floor(365.25 * (Y + 4716))
                  + Floor(30.6001 * (M + 1))
                  + D + B - 1524.5;

        return jd;
    }

    public static DateTime FromJulianDayUtc(double jd)
    {
        // Inverse of Julian day (UTC). Returns DateTimeKind.Utc.
        var Z = Floor(jd + 0.5);
        var F = (jd + 0.5) - Z;
        var A = Z;

        var B = A + 1524;
        var C = Floor((B - 122.1) / 365.25);
        var D = Floor(365.25 * C);
        var E = Floor((B - D) / 30.6001);

        var day = B - D - Floor(30.6001 * E) + F;
        var month = (int)((E < 14) ? (E - 1) : (E - 13));
        var year = (int)((month > 2) ? (C - 4716) : (C - 4715));

        var dayInt = (int)Floor(day);
        var frac = day - dayInt;
        var totalMs = (int)Round(frac * 86400000.0);

        var dt = new DateTime(year, month, dayInt, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(totalMs);
        return dt;
    }

#if  !NETCOREAPP && !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Clamps a value between the specified minimum and maximum inclusive bounds.
    /// Works as a replacement for Math.Clamp on older .NET versions.
    /// </summary>
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
        {
            return min;
        }

        if (value.CompareTo(max) > 0)
        {
            return max;
        }

        return value;
    }
#endif

    public const double J2000 = 2451545.0; // JD at 2000-01-01 12:00:00 UTC

    /// <summary>Solar transit (solar noon) JD for day index n and longitude Lw (radians, west-positive).</summary>
    public static double SolarTransitJulian(double n, double Lw)
    {
        // Mean anomaly and ecliptic longitude at approximate transit
        var M = DegreesToRadians(357.5291 + 0.98560028 * (n)); // n ~ days since J2000 (approximate)
        var C = DegreesToRadians(1.9148) * Sin(M)
                 + DegreesToRadians(0.0200) * Sin(2 * M)
                 + DegreesToRadians(0.0003) * Sin(3 * M);
        var lambda = DegreesToRadians(102.9372) + M + C + PI; // perihelion + M + C + 180°

        // Equation: Jtransit = J2000 + n + 0.0009 + Lw/2π + 0.0053*sin(M) - 0.0069*sin(2λ)
        var Japprox = J2000 + n + 0.0009 + Lw / (2.0 * PI);
        return Japprox + 0.0053 * Sin(M) - 0.0069 * Sin(2.0 * lambda);
    }

    /// <summary>Returns (ecliptic longitude λ, declination δ) at JD (radians).</summary>
    public static SolarEclipticAndDeclination GetSolarEclipticAndDeclination(double JD)
    {
        var n = JD - J2000;

        var M = DegreesToRadians(357.5291 + 0.98560028 * n);
        var C = DegreesToRadians(1.9148) * Sin(M)
                 + DegreesToRadians(0.0200) * Sin(2 * M)
                 + DegreesToRadians(0.0003) * Sin(3 * M);
        var lambda = DegreesToRadians(102.9372) + M + C + PI; // radians
        var eps = DegreesToRadians(23.4397); // obliquity approximation

        var delta = Asin(Sin(eps) * Sin(lambda)); // declination
        return new SolarEclipticAndDeclination(lambda, delta);
    }

    /// <summary>
    /// Returns true if the Sun is above the horizon right now
    /// (using current UTC time and standard sunrise/sunset threshold).
    /// </summary>
    /// <param name="position">Position.</param>
    public static bool IsSunUp(WGS84Position position)
    {
        // Get the current UTC time and date
        var nowUtc = DateTime.UtcNow;

        // Compute today's sunrise/sunset events
        var events = new SolarDayEvents(nowUtc, position, SolarDayEvents.TwilightType.SunriseSunset);

        // If sunrise or sunset are missing (polar regions), fall back to solar altitude
        if (events.SunriseUtc == null || events.SunsetUtc == null)
        {
            var solar = new SolarTime(nowUtc, position);
            return solar.SunElevation > 0.0;
        }

        // Compare time to sunrise and sunset
        return nowUtc >= events.SunriseUtc && nowUtc < events.SunsetUtc;
    }
}

public readonly record struct SolarEclipticAndDeclination(double Lambda, double Delta);
