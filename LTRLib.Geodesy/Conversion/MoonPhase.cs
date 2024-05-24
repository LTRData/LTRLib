using System;

namespace LTRLib.Geodesy.Conversion;

public class MoonPhaseCalculator
{
    /// <summary>
    /// Calculates moon phase (0 - 1) for a specified day
    /// </summary>
    /// <param name="date">Day</param>
    /// <returns>Moon phase between 0 and 1</returns>
    public static double GetMoonPhase(DateTime date)
    {
        // Calculate Julian Date
        var julianDate = GetJulianDate(date);

        // Number of days since the epoch 2000-01-01
        var daysSinceNew = julianDate - 2451550.1;
        var newMoons = daysSinceNew / 29.53058867;
        var fraction = newMoons - Math.Floor(newMoons);

        return fraction;
    }

    /// <summary>
    /// Returns a string containing a Unicode moon symbol for a specified moon phase between 0 and 1.
    /// </summary>
    /// <param name="phase">Moon phase as returned by <see cref="GetMoonPhase(DateTime)"/></param>
    /// <returns>String containing a Unicode moon symbol</returns>
    public static string GetMoonPhaseSymbol(double phase) => phase switch
    {
        < 0.02 => "🌑",
        < 0.25 => "🌒",
        < 0.27 => "🌓",
        < 0.50 => "🌔",
        < 0.52 => "🌕",
        < 0.75 => "🌖",
        < 0.77 => "🌗",
        _ => "🌘"
    };

    /// <summary>
    /// Returns name of specified moon phase between 0 and 1.
    /// </summary>
    /// <param name="phase">Moon phase as returned by <see cref="GetMoonPhase(DateTime)"/></param>
    /// <returns>Name of moon phase</returns>
    public static string GetMoonPhaseName(double phase) => phase switch
    {
        < 0.02 => "New Moon",
        < 0.25 => "Waxing Crescent",
        < 0.27 => "First Quarter",
        < 0.50 => "Waxing Gibbous",
        < 0.52 => "Full Moon",
        < 0.75 => "Waning Gibbous",
        < 0.77 => "Last Quarter",
        _ => "Waning Crescent"
    };

    /// <summary>
    /// Calculates Julian date for a specified UTC <see cref="DateTime"/>
    /// </summary>
    /// <param name="date">UTC date and time</param>
    /// <returns>Julian date</returns>
    public static double GetJulianDate(DateTime date)
    {
        var year = date.Year;
        var month = date.Month;
        if (month <= 2)
        {
            year--;
            month += 12;
        }
        var day = date.Day;

        var A = year / 100;
        var B = 2 - A + (A / 4);

        var jd = Math.Floor(365.25 * (year + 4716)) +
                   Math.Floor(30.6001 * (month + 1)) +
                   day + B - 1524.5;

        jd += (date.Hour + date.Minute / 60.0 + date.Second / 3600.0) / 24.0;

        return jd;
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Returns moon position in the sky for a specified time and location on earth.
    /// </summary>
    /// <param name="date">UTC date and time</param>
    /// <param name="latitude">Latitude in degrees</param>
    /// <param name="longitude">Longitude in degrees</param>
    /// <returns>Azimuth and elevation, in radians, where moon is found in the sky</returns>
    public static (double Azimuth, double Elevation) GetMoonPosition(DateTime date, double latitude, double longitude)
    {
        var jd = GetJulianDate(date);

        // Calculate moon's ecliptic coordinates
        (var eclLon, var eclLat) = GetMoonEclipticCoordinates(jd);

        // Convert to equatorial coordinates
        (var ra, var dec) = EclipticToEquatorial(eclLon, eclLat, jd);

        // Calculate local sidereal time
        var lst = LocalSiderealTime(jd, longitude);

        // Convert to horizontal coordinates
        (var azimuth, var elevation) = EquatorialToHorizontal(ra, dec, lst, latitude);

        return (azimuth, elevation);
    }

    /// <summary>
    /// Returns moon ecliptic coordinates for a specified time.
    /// </summary>
    /// <param name="jd">Julian date</param>
    /// <returns>Coordinates in degrees</returns>
    public static (double eclLon, double eclLat) GetMoonEclipticCoordinates(double jd)
    {
        // Simplified calculation for moon's ecliptic longitude and latitude
        // For higher accuracy, use precise lunar ephemeris data
        //var T = (jd - 2451545.0) / 36525.0;
        var L0 = 218.316 + 13.176396 * (jd - 2451545.0);
        var M = 134.963 + 13.064993 * (jd - 2451545.0);

        L0 %= 360.0;
        M %= 360.0;

        var L = L0 + 6.289 * Math.Sin(Calculations.DegreesToRadians(M));
        var B = 5.128 * Math.Sin(Calculations.DegreesToRadians(M));

        return (L, B);
    }

    /// <summary>
    /// Returns moon equatorial coordinates for a specified time.
    /// </summary>
    /// <param name="eclLon">Ecliptic coordinates in degrees</param>
    /// <param name="eclLat">Ecliptic coordinates in degrees</param>
    /// <param name="jd">Julian date</param>
    /// <returns>Coordinates in radians</returns>
    public static (double ra, double dec) EclipticToEquatorial(double eclLon, double eclLat, double jd)
    {
        // Obliquity of the ecliptic
        var T = (jd - 2451545.0) / 36525.0;
        var epsilon = 23.439292 - 0.0130042 * T;

        var ra = Math.Atan2(Math.Cos(Calculations.DegreesToRadians(epsilon)) * Math.Sin(Calculations.DegreesToRadians(eclLon)),
                               Math.Cos(Calculations.DegreesToRadians(eclLon)));
        var dec = Math.Asin(Math.Sin(Calculations.DegreesToRadians(eclLat)) * Math.Cos(Calculations.DegreesToRadians(epsilon)) +
                               Math.Cos(Calculations.DegreesToRadians(eclLat)) * Math.Sin(Calculations.DegreesToRadians(epsilon)) * Math.Sin(Calculations.DegreesToRadians(eclLon)));

        return (ra, dec);
    }

    /// <summary>
    /// </summary>
    /// <param name="jd">Julian date</param>
    /// <param name="longitude">Longitude in degrees</param>
    /// <returns></returns>
    public static double LocalSiderealTime(double jd, double longitude)
    {
        var T = (jd - 2451545.0) / 36525.0;
        var GMST = 280.46061837 + 360.98564736629 * (jd - 2451545.0) +
                      T * T * (0.000387933 - T / 38710000.0);
        GMST %= 360.0;

        if (GMST < 0)
        {
            GMST += 360.0;
        }

        return GMST + longitude;
    }

    /// <summary>
    /// Returns moon horizontal coordinates.
    /// </summary>
    /// <param name="ra">Equatorial coordinates in radians</param>
    /// <param name="dec">Equatorial coordinates in radians</param>
    /// <param name="lst">Local siderial time, in degrees</param>
    /// <param name="latitude">Latitude, in degrees</param>
    /// <returns>Coordinates in radians</returns>
    public static (double Azimuth, double Elevation) EquatorialToHorizontal(double ra, double dec, double lst, double latitude)
    {
        var ha = Calculations.DegreesToRadians(lst) - ra;

        if (ha < 0)
        {
            ha += 2 * Math.PI;
        }

        latitude = Calculations.DegreesToRadians(latitude);

        var elevation = Math.Asin(Math.Sin(dec) * Math.Sin(latitude) +
                                    Math.Cos(dec) * Math.Cos(latitude) * Math.Cos(ha));

        var azimuth = Math.Acos((Math.Sin(dec) - Math.Sin(elevation) * Math.Sin(latitude)) /
                                   (Math.Cos(elevation) * Math.Cos(latitude)));

        if (Math.Sin(ha) > 0)
        {
            azimuth = 2 * Math.PI - azimuth;
        }

        return (azimuth, elevation);
    }
#endif
}
