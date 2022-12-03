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

public readonly struct SunTime
{
    public SunTime(DateTime datetime, WGS84Position position)
    {
        datetime = datetime.ToUniversalTime();

        var lat = DegreesToRadians(position.Latitude);
        var lon = DegreesToRadians(position.Longitude);

        var year_first_noon = new DateTime(datetime.Year, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var year_length = new DateTime(datetime.Year + 1, 1, 1, 12, 0, 0, DateTimeKind.Utc) - year_first_noon;
        var year_offset = datetime - year_first_noon;
        var year_angle = 2 * PI * year_offset.TotalMilliseconds / year_length.TotalMilliseconds;

        EqTime = TimeSpan.FromMilliseconds(229.18 * (4.5 + 112.08 * Cos(year_angle) - 1924.62 * Sin(year_angle) - 876.9 * Cos(2 * year_angle) - 2450.94 * Sin(2 * year_angle)));

        var TimeOffset = EqTime + TimeSpan.FromMilliseconds(MillisecondsPerRadian * lon);

        Declination = 0.006918 - 0.399912 * Cos(year_angle) + 0.070257 * Sin(year_angle) - 0.006758 * Cos(2 * year_angle) + 0.000907 * Sin(2 * year_angle) - 0.002697 * Cos(3 * year_angle) + 0.00148 * Sin(3 * year_angle);

        TrueSolarTime = datetime.TimeOfDay + TimeOffset;

        SolarHourAngle = TrueSolarTime.TotalMilliseconds / MillisecondsPerRadian - PI;

        var sinlat = Sin(lat);
        var coslat = Cos(lat);
        var sindecl = Sin(Declination);
        var cosdecl = Cos(Declination);
        var cossolarhour = Cos(SolarHourAngle);

        var sinf = sinlat * sindecl + coslat * cosdecl * cossolarhour;

        SunElevation = Asin(sinf);

        var cosf = Cos(SunElevation);

        HourAngle = Acos(CosZenith / coslat / cosdecl - Tan(lat) * Tan(Declination));

        Azimuth = Acos((sindecl * coslat - cosdecl * cossolarhour * sinlat) / cosf);

        if (SolarHourAngle > 0)
        {
            Azimuth = 2 * PI - Azimuth;
        }

        SunNoon = TimeSpan.FromMilliseconds(Milliseconds12Hours - MillisecondsPerRadian * lon) - EqTime;

        Sunrise = TimeSpan.FromMilliseconds(Milliseconds12Hours - MillisecondsPerRadian * (lon + HourAngle)) - EqTime;

        Sunset = TimeSpan.FromMilliseconds(Milliseconds12Hours - MillisecondsPerRadian * (lon - HourAngle)) - EqTime;
    }

    public TimeSpan EqTime { get; }
    public double Declination { get; }
    public TimeSpan TrueSolarTime { get; }
    public double SolarHourAngle { get; }
    public double SunElevation { get; }
    public double Azimuth { get; }
    public double HourAngle { get; }
    public TimeSpan Sunrise { get; }
    public TimeSpan SunNoon { get; }
    public TimeSpan Sunset { get; }
}
