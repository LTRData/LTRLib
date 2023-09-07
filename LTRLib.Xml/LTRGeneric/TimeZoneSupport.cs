using System;

namespace LTRLib.LTRGeneric;

public static class TimeZoneSupport
{
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

}
