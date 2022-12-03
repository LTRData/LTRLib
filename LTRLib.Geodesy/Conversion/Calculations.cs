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

    public static bool IsSunUpNow(WGS84Position position)
    {
        var dt = DateTime.UtcNow;

        var sunTime = new SunTime(dt, position);

        return dt.TimeOfDay > sunTime.Sunrise && dt.TimeOfDay < sunTime.Sunset;
    }
}
