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

public readonly struct SolarDayEvents
{
    public enum TwilightType
    {
        /// <summary>Standard sunrise/sunset (solar center at −0.833° altitude ≈ 90.833° zenith).</summary>
        SunriseSunset,

        /// <summary>Civil twilight (Sun at −6° altitude ⇒ 96° zenith).</summary>
        Civil,

        /// <summary>Nautical twilight (Sun at −12° altitude ⇒ 102° zenith).</summary>
        Nautical,

        /// <summary>Astronomical twilight (Sun at −18° altitude ⇒ 108° zenith).</summary>
        Astronomical
    }

    public SolarDayEvents(
        DateTime datetime,
        WGS84Position position,
        TwilightType twilight = TwilightType.SunriseSunset)
    {
        var dateUtc = datetime.ToUniversalTime().Date; // midnight UTC

        // Map enum -> zenith (degrees)
        var solarZenithDeg = twilight switch
        {
            TwilightType.SunriseSunset => 90.833, // −0.833° altitude (standard)
            TwilightType.Civil => 96.0,   // −6°
            TwilightType.Nautical => 102.0,  // −12°
            TwilightType.Astronomical => 108.0,  // −18°
            _ => 90.833
        };

        var Jdate = JulianDay(dateUtc);
        var Lw = -DegreesToRadians(position.Longitude);
        var phi = DegreesToRadians(position.Latitude);

        // Approximate day index n (days since J2000) for local longitude
        double n = Math.Round((Jdate - J2000 - Lw / (2.0 * PI)));

        // Solar noon (Julian Day)
        double Jtransit = SolarTransitJulian(n, Lw);

        // Declination at transit
        (_, double delta) = GetSolarEclipticAndDeclination(Jtransit);

        // Compute cos(H0) for the selected zenith
        // Note: alt = 90° - zenith ⇒ sin(alt) = cos(zenith)
        var altDeg = 90.0 - solarZenithDeg; // negative for twilight thresholds
        var cosH0 = (Sin(DegreesToRadians(altDeg)) - Sin(phi) * Sin(delta))
                       / (Cos(phi) * Cos(delta));

        SolarNoonUtc = FromJulianDayUtc(Jtransit);
        
        if (cosH0 >= 1.0)
        {
            // Sun never reaches this altitude: no "rise" or "set" for this threshold
            SunriseUtc = null;
            SunsetUtc = null;
        }
        else if (cosH0 <= -1.0)
        {
            // Sun stays above this altitude all day: no "set" for this threshold
            SunriseUtc = null;
            SunsetUtc = null;
        }
        else
        {
            var H0 = Acos(cosH0); // radians
            var Jrise = Jtransit - H0 / (2.0 * PI);
            var Jset = Jtransit + H0 / (2.0 * PI);

            SunriseUtc = FromJulianDayUtc(Jrise);
            SunsetUtc = FromJulianDayUtc(Jset);
        }
    }

    /// <summary>
    /// null if no sunrise that date (at given zenith)
    /// </summary>
    public DateTime? SunriseUtc { get; }

    /// <summary>
    /// Solar transit
    /// </summary>
    public DateTime? SolarNoonUtc { get; }

    /// <summary>
    /// null if no sunset that date (at given zenith)
    /// </summary>
    public DateTime? SunsetUtc { get; }
}
