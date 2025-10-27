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
using static Calculations;

public readonly struct SolarTime
{
    /// <summary>
    /// Calculates apparent solar time and mean solar time,
    /// as well as solar position for a given (lat, lon) and time.
    /// </summary>
    /// <param name="datetime">Time.</param>
    /// <param name="position">Position.</param>
    public SolarTime(DateTime datetime, WGS84Position position)
    {
        datetime = datetime.ToUniversalTime();

        var latitudeDeg = position.Latitude;
        var longitudeDeg = position.Longitude;

        // -------- Astronomy section (simplified NOAA/SPA model) ----------
        var jd = JulianDay(datetime);
        var T = (jd - 2451545.0) / 36525.0;

        var L0 = NormalizeDeg(280.46646 + T * (36000.76983 + T * 0.0003032));
        var M = 357.52911 + T * (35999.05029 - 0.0001537 * T);
        var e = 0.016708634 - T * (0.000042037 + 0.0000001267 * T);

        var C = Sin(DegreesToRadians(M)) * (1.914602 - T * (0.004817 + 0.000014 * T))
                 + Sin(DegreesToRadians(2 * M)) * (0.019993 - 0.000101 * T)
                 + Sin(DegreesToRadians(3 * M)) * 0.000289;

        var trueLong = L0 + C;
        var omega = 125.04 - 1934.136 * T;
        var lambda = trueLong - 0.00569 - 0.00478 * Sin(DegreesToRadians(omega));

        var eps0 = 23 + (26 + (21.448 - T * (46.815 + T * (0.00059 - 0.001813 * T))) / 60.0) / 60.0;
        var eps = eps0 + 0.00256 * Cos(DegreesToRadians(omega));

        Declination = RadiansToDegrees(Asin(Sin(DegreesToRadians(eps)) * Sin(DegreesToRadians(lambda)))); // solar declination

        // Equation of time (EoT) in minutes
        var y = Tan(DegreesToRadians(eps / 2.0));
        y *= y;
        EquationOfTimeMinutes = 4.0 * RadiansToDegrees(
            y * Sin(DegreesToRadians(2 * L0))
          - 2.0 * e * Sin(DegreesToRadians(M))
          + 4.0 * e * y * Sin(DegreesToRadians(M)) * Cos(DegreesToRadians(2 * L0))
          - 0.5 * y * y * Sin(DegreesToRadians(4 * L0))
          - 1.25 * e * e * Sin(DegreesToRadians(2 * M))
        ); // minutes

        // --------- Time values (no time zone applied) ----------
        var minutesUTC = datetime.TimeOfDay.TotalMinutes;

        // Mean Solar Time (LMST) – depends only on longitude
        var lmstMin = minutesUTC + 4.0 * longitudeDeg;           // 4 min/degree
        lmstMin = Mod(lmstMin, 1440.0);
        if (lmstMin < 0)
        {
            lmstMin += 1440.0;
        }

        MeanSolarTime = TimeSpan.FromMinutes(lmstMin);

        // Apparent Solar Time – longitude + Equation of Time
        var tstMin = minutesUTC + 4.0 * longitudeDeg + EquationOfTimeMinutes;
        tstMin = Mod(tstMin, 1440.0);
        if (tstMin < 0)
        {
            tstMin += 1440.0;
        }

        ApparentSolarTime = TimeSpan.FromMinutes(tstMin);

        // Hour angle (from apparent solar time), -180..+180°
        SolarHourAngle = (tstMin / 4.0) - 180.0;

        // Solar altitude & azimuth
        var lat = DegreesToRadians(latitudeDeg);
        var dec = DegreesToRadians(Declination);
        var H = DegreesToRadians(SolarHourAngle);

        var sinLat = Sin(lat);
        var sinDec = Sin(dec);
        var cosLat = Cos(lat);
        var cosDec = Cos(dec);
        var cosH = Cos(H);

        var cosZenith = sinLat * sinDec + cosLat * cosDec * cosH;
        cosZenith = Clamp(cosZenith, -1.0, 1.0);
        var zenith = Acos(cosZenith);
        SunElevation = 90.0 - RadiansToDegrees(zenith);

        // Azimuth: 0° = north, east = 90°
        var sinZenith = Sin(zenith);
        var sinAz = -Sin(H) * cosDec / sinZenith;
        var cosAz = (sinDec - sinLat * cosZenith) / (cosLat * sinZenith);
        Azimuth = Mod(RadiansToDegrees(Atan2(sinAz, cosAz)), 360.0);
    }

    public double EquationOfTimeMinutes { get; }
    public double Declination { get; }
    public TimeSpan ApparentSolarTime { get; }
    public TimeSpan MeanSolarTime { get; }
    public double SolarHourAngle { get; }
    public double SunElevation { get; }
    public double Azimuth { get; }
    public double HourAngle { get; }
}
