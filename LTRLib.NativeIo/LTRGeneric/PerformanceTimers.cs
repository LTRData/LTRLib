/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static System.Math;

namespace LTRLib.LTRGeneric;

using Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

/// <summary>
/// Static methods and properties for accessing high-performance timer values.
/// </summary>
[SupportedOSPlatform("windows")]
public static class PerformanceTimers
{
    /// <summary>
    /// Low accuracy timer.
    /// </summary>
    /// <returns>Returns number of ms since boot. Updates about every 15-20 ms.</returns>
    public static long TickCount => GetTickCount64();

    /// <summary>
    /// Low accuracy timer.
    /// </summary>
    /// <returns>Returns elapsed time since boot. Updates about every 15-20 ms.</returns>
    public static TimeSpan SystemUptime => TimeSpan.FromMilliseconds(GetTickCount64());

    /// <summary>
    /// Frequency of performance counter. This is the number the performance counter will
    /// count in a second. This value depends on hardware and is usually somewhere around
    /// 10-20 MHz.
    /// </summary>
    public static long PerformanceCountsPerSecond { get; }

    /// <summary>
    /// Returns current value of the performance counter. This is a high accuracy timer.
    /// </summary>
    /// <value>Number of performance timer counts since boot.
    /// 
    /// Number of performance timer counts per second can be found in <see cref="PerformanceCountsPerSecond"> property.</see>"/></value>
    public static long PerformanceCounterValue => QueryPerformanceCounter(out var perfcount) ? perfcount : throw new Win32Exception();

    /// <summary>
    /// Converts a number of ticks to performance timer counts.
    /// </summary>
    /// <param name="ticks">Number of ticks.</param>
    /// <returns>Number of performance timer counts corresponding to specified number of ticks, rounded up to nearest integer.</returns>
    public static long ConvertTicksToPerformanceCounts(long ticks)
    {
        var prod = checked(ticks * performance_counts_per_ticks_multiplier);
        var value = DivRem(prod, performance_counts_per_ticks_divisor, out var rem);

        if (rem != 0)
        {
            ++value;
        }

        return value;
    }

    /// <summary>
    /// Converts a number of microseconds to performance timer counts.
    /// </summary>
    /// <param name="microsec">Number of microseconds.</param>
    /// <returns>Number of performance timer counts corresponding to specified number of microseconds, rounded up to nearest integer.</returns>
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static long ConvertMicrosecondsToPerformanceCounts(long microsec) =>
        ConvertTicksToPerformanceCounts(checked(microsec * ticks_per_microsecond));

    /// <summary>
    /// Converts a TimeSpan value to performance timer counts.
    /// </summary>
    /// <param name="timespan">TimeSpan value to convert.</param>
    /// <returns>Number of performance timer counts corresponding to TimeSpan value, rounded up to nearest integer number of performance timer counts.</returns>
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static long ConvertToPerformanceCounts(this TimeSpan timespan) =>
        ConvertTicksToPerformanceCounts(timespan.Ticks);

    /// <summary>
    /// Converts a number of performance timer counts to ticks.
    /// </summary>
    /// <param name="perfcount">Number of performance timer counts.</param>
    /// <returns>Number of ticks corresponding to specified performance timer counts, rounded down to nearest integer.</returns>
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static long ConvertPerformanceCountsToTicks(long perfcount) =>
        checked(perfcount * performance_counts_per_ticks_divisor / performance_counts_per_ticks_multiplier);

    /// <summary>
    /// Converts a number of performance timer counts to a TimeSpan value.
    /// </summary>
    /// <param name="perfcount">Number of performance timer counts.</param>
    /// <returns>TimeSpan value corresponding to specified performance timer counts, rounded down to nearest number of ticks that the TimeSpan structure can hold.</returns>
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static TimeSpan ConvertPerformanceCountsToTimeSpan(long perfcount) =>
        TimeSpan.FromTicks(ConvertPerformanceCountsToTicks(perfcount));

    /// <summary>
    /// Waits specified number of performance timer counts by spinning in a tight loop until at least timer counts have passed. This can usually be used for very exact timing,
    /// since there is no asynchronous object models, Task objects or context switches used in the wait loop.
    /// 
    /// Number of performance timer counts per second can be found in <see cref="PerformanceCountsPerSecond">PerformanceCountsPerSecond</see> property.
    /// 
    /// You can convert microseconds, ticks or TimeSpan values to performance counts using support methods in this class.
    /// </summary>
    /// <param name="perfcount">Number of performance timer counts to wait.</param>
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static void SpinWaitPerformanceCounts(long perfcount)
    {
        perfcount += PerformanceCounterValue;

        while (PerformanceCounterValue < perfcount)
        {
        }
    }

#if NETCOREAPP && !NETCOREAPP2_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivRem(long dividend, long divisor, out long remainder)
    {
        remainder = dividend % divisor;
        return dividend / divisor;
    }
#endif

    #region Internal implementation
#if NETCOREAPP
    private static long GetTickCount64() => Environment.TickCount64;
#else
    [DllImport("kernel32.dll", SetLastError = true)]
    [SupportedOSPlatform("windows")]
    private static extern long GetTickCount64();
#endif

    [DllImport("kernel32.dll", SetLastError = true)]
    [SupportedOSPlatform("windows")]
    private static extern bool QueryPerformanceFrequency(out long frequency);

    [DllImport("kernel32.dll", SetLastError = true)]
    [SupportedOSPlatform("windows")]
    private static extern bool QueryPerformanceCounter(out long count);

    private static readonly long ticks_per_microsecond = TimeSpan.FromSeconds(1).Ticks / 1000000;
    private static readonly long performance_counts_per_ticks_multiplier;
    private static readonly long performance_counts_per_ticks_divisor;

    static PerformanceTimers()
    {
        QueryPerformanceFrequency(out var freq);

        PerformanceCountsPerSecond = freq;

        var factor = NumericExtensions.GreatestCommonDivisor(PerformanceCountsPerSecond, TimeSpan.FromSeconds(1).Ticks);

        performance_counts_per_ticks_multiplier = PerformanceCountsPerSecond / factor;
        performance_counts_per_ticks_divisor = TimeSpan.FromSeconds(1).Ticks / factor;
    }
#endregion
}
