using System;
using System.Globalization;

namespace LTRLib.LTRGeneric;

public static class DateTimeSupport
{
    public static DateTime UnixTimeBase { get; } = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public const long TicksPerSecond = 10000000L;

    public const long TicksPerMillisecond = 10000L;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static DateTime CurrentConfiguredTimeZoneLocalTime => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ConfiguredTimeZone);

    public static TimeZoneInfo ConfiguredTimeZone { get; set; } = GetConfiguredTimeZone();

    private static TimeZoneInfo GetConfiguredTimeZone()
    {
        var tz = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];

        if (string.IsNullOrEmpty(tz))
        {
            return TimeZoneInfo.Local;
        }
        else
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tz);
        }
    }

#endif

    public static DateTime DateFromUnixTimestamp(long unixTime) => UnixTimeBase.AddTicks(unixTime * TicksPerSecond);

    public static DateTime DateFromUnixTimestamp(decimal unixTime) => UnixTimeBase.AddTicks((long)Math.Round(unixTime * TicksPerSecond));

    public static DateTime DateFromUnixTimestamp(double unixTime) => UnixTimeBase.AddSeconds(unixTime);

    public static double DateToUnixTimestamp(DateTime date) => (date - UnixTimeBase).TotalSeconds;

    public static DateTime DateFromJavaTimestamp(long javaTime) => UnixTimeBase.AddTicks(javaTime * TicksPerMillisecond);

    public static DateTime DateFromJavaTimestamp(decimal javaTime) => UnixTimeBase.AddTicks((long)Math.Round(javaTime * TicksPerMillisecond));

    public static DateTime DateFromJavaTimestamp(double javaTime) => UnixTimeBase.AddMilliseconds(javaTime);

    public static double DateToJavaTimestamp(DateTime date) => (date - UnixTimeBase).TotalMilliseconds;

    /// <summary>
    /// Gets DateTime value for first day in specified week using current culture.
    /// </summary>
    public static DateTime DateFromWeekOfYear(int Year, int WeekOfYear) => DateFromWeekOfYear(Year, WeekOfYear, DateTimeFormatInfo.CurrentInfo);

    /// <summary>
    /// Gets DateTime value for first day in specified week.
    /// </summary>
    /// <param name="Year"></param>
    /// <param name="WeekOfYear"></param>
    /// <param name="format">DateTimeFormatInfo to use for locale aware conversion.</param>
    public static DateTime DateFromWeekOfYear(int Year, int WeekOfYear, DateTimeFormatInfo format)
    {
        var Jan1 = new DateTime(Year, 1, 1);
        var Jan1Week = format.Calendar.GetWeekOfYear(Jan1, format.CalendarWeekRule, format.FirstDayOfWeek);
        DateTime Week1Day;
        if (Jan1Week == 1)
        {
            Week1Day = Jan1;
        }
        else
        {
            Week1Day = format.Calendar.AddWeeks(Jan1, 1);
        }

        var Week1DayOfWeek = (int)format.Calendar.GetDayOfWeek(Week1Day);
        if (Week1DayOfWeek < (int)format.FirstDayOfWeek)
        {
            Week1DayOfWeek += 7;
        }

        var Week1FirstDay = Week1Day.AddDays((int)format.FirstDayOfWeek - Week1DayOfWeek);
        return format.Calendar.AddWeeks(Week1FirstDay, WeekOfYear - 1);
    }

    /// <summary>
    /// Converts a year-week-dayofweek string representation of a date to a DateTime value using current culture.
    /// </summary>
    /// <param name="s">String representation for a date in one of following formats: yywwd, yyww-d, ywwd or yww-d</param>
    public static DateTime DateFromYYWWDD(string s) => DateFromYYWWDD(s, DateTimeFormatInfo.CurrentInfo);

    /// <summary>
    /// Converts a year-week-dayofweek string representation of a date to a DateTime value.
    /// </summary>
    /// <param name="s">String representation for a date in one of following formats: yywwd, yyww-d, ywwd or yww-d</param>
    /// <param name="format">DateTimeFormatInfo to use for locale aware conversion.</param>
    public static DateTime DateFromYYWWDD(string s, DateTimeFormatInfo format)
    {

        if (string.IsNullOrEmpty(s)
            || s.Length < 3
            || s.Length > 6
            || ((s.Length == 5 || s.Length == 6) && s[s.Length - 2] != '-'))
        {
            return default;
        }

        if (s.Length == 3 || s.Length == 5)
        {
            var CurrentYear = format.Calendar.GetYear(DateTime.Today);

            var Prefix = CurrentYear / 10;

            if ((CurrentYear % 10) - (s[0] - '0') >= 5)
            {
                Prefix += 1;
            }
            else if ((CurrentYear % 10) - (s[0] - '0') <= 5)
            {
                Prefix -= 1;
            }

            Prefix %= 10;

            s = Prefix + s;
        }

        var Year = format.Calendar.ToFourDigitYear(int.Parse(s.Substring(0, 2)));

        var WeekOfYear = int.Parse(s.Substring(2, 2));

        var WeekFirstDay = DateFromWeekOfYear(Year, WeekOfYear, format);

        if (s.Length == 6)
        {
            var DayOfWeek = s[5] - '0';

            if (DayOfWeek >= 1 && DayOfWeek <= 7)
            {
                return WeekFirstDay.AddDays(DayOfWeek - 1);
            }
            else
            {
                return WeekFirstDay;
            }
        }
        else
        {
            return WeekFirstDay;
        }
    }

    public static bool IsValidFileTime(long filetime)
        => filetime >= DateTimeConstants.MinFileTime && filetime <= DateTimeConstants.MaxFileTime;

    public static DateTime? TryParseFileTime(long filetime)
    {
        if (filetime >= DateTimeConstants.MinFileTime && filetime <= DateTimeConstants.MaxFileTime)
        {
            return DateTime.FromFileTime(filetime);
        }
        else
        {
            return default;
        }
    }

    public static DateTime? TryParseFileTimeUtc(long filetime)
    {
        if (filetime >= DateTimeConstants.MinFileTime && filetime <= DateTimeConstants.MaxFileTime)
        {
            return DateTime.FromFileTimeUtc(filetime);
        }
        else
        {
            return default;
        }
    }

}

