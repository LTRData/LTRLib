using System;

namespace LTRLib.Geodesy.Conversion;

public class MoonPhaseCalculator
{
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
}