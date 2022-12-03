// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

namespace LTRLib.Extensions;

public static class MathExtensions
{
    public static int GetNumberOfDecimalsSafe(this decimal dec) => (decimal.GetBits(dec)[3] & 0xFF0000) >> 16;

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static int GetNumberOfDecimals(this decimal dec)
    {
        Span<byte> bytes = stackalloc byte[sizeof(decimal)];

        MemoryMarshal.Write(bytes, ref dec);

        var flags = MemoryMarshal.Read<int>(bytes);

        return (flags & 0xFF0000) >> 16;
    }
#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static double? Median(this IEnumerable<double?> sequence)
    {
        var a = (from n in sequence
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal? Median(this IEnumerable<decimal?> sequence)
    {

        var a = (from n in sequence
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static double Median(this IEnumerable<double> sequence)
    {

        var a = (from v in sequence
                 orderby v
                 select v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal Median(this IEnumerable<decimal> sequence)
    {

        var a = (from v in sequence
                 orderby v
                 select v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static double? Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, double?> selector)
    {

        var a = (from n in sequence.Select(selector)
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal? Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, decimal?> selector)
    {

        var a = (from n in sequence.Select(selector)
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static double Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, double> selector)
    {

        var a = sequence.Select(selector).OrderBy(v => v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, decimal> selector)
    {

        var a = sequence.Select(selector).OrderBy(v => v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

#endif

    // 
    // Summary:
    // Rounds a decimal value to a specified number of fractional digits.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // decimals:
    // The number of decimal places in the return value.
    // 
    // Returns:
    // The number nearest to d that contains a number of fractional digits equal to
    // decimals.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // decimals is less than 0 or greater than 28.
    // 
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d, int decimals) => Math.Round(d, decimals);
    // 
    // Summary:
    // Rounds a decimal value to the nearest integral value.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // Returns:
    // The integer nearest parameter d. If the fractional component of d is halfway
    // between two integers, one of which is even and the other odd, the even number
    // is returned. Note that this method returns a System.Decimal instead of an integral
    // type.
    // 
    // Exceptions:
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d) => Math.Round(d);

    // 
    // Summary:
    // Rounds a decimal value to a specified number of fractional digits. A parameter
    // specifies how to round the value if it is midway between two numbers.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // decimals:
    // The number of decimal places in the return value.
    // 
    // mode:
    // Specification for how to round d if it is midway between two other numbers.
    // 
    // Returns:
    // The number nearest to d that contains a number of fractional digits equal to
    // decimals. If d has fewer fractional digits than decimals, d is returned unchanged.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // decimals is less than 0 or greater than 28.
    // 
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    // 
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d, int decimals, MidpointRounding mode) => Math.Round(d, decimals, mode);
    // 
    // Summary:
    // Rounds a decimal value to the nearest integer. A parameter specifies how to round
    // the value if it is midway between two numbers.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // mode:
    // Specification for how to round d if it is midway between two other numbers.
    // 
    // Returns:
    // The integer nearest d. If d is halfway between two numbers, one of which is even
    // and the other odd, then mode determines which of the two is returned.
    // 
    // Exceptions:
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    // 
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d, MidpointRounding mode) => Math.Round(d, mode);
    // 
    // Summary:
    // Rounds a double-precision floating-point value to the nearest integral value.
    // 
    // Parameters:
    // a:
    // A double-precision floating-point number to be rounded.
    // 
    // Returns:
    // The integer nearest a. If the fractional component of a is halfway between two
    // integers, one of which is even and the other odd, then the even number is returned.
    // Note that this method returns a System.Double instead of an integral type.
    public static double RoundValue(this double a) => Math.Round(a);
    // 
    // Summary:
    // Rounds a double-precision floating-point value to a specified number of fractional
    // digits.
    // 
    // Parameters:
    // value:
    // A double-precision floating-point number to be rounded.
    // 
    // digits:
    // The number of fractional digits in the return value.
    // 
    // Returns:
    // The number nearest to value that contains a number of fractional digits equal
    // to digits.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // digits is less than 0 or greater than 15.
    public static double RoundValue(this double value, int digits) => Math.Round(value, digits);

    // 
    // Summary:
    // Rounds a double-precision floating-point value to the nearest integer. A parameter
    // specifies how to round the value if it is midway between two numbers.
    // 
    // Parameters:
    // value:
    // A double-precision floating-point number to be rounded.
    // 
    // mode:
    // Specification for how to round value if it is midway between two other numbers.
    // 
    // Returns:
    // The integer nearest value. If value is halfway between two integers, one of which
    // is even and the other odd, then mode determines which of the two is returned.
    // 
    // Exceptions:
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    public static double RoundValue(this double value, MidpointRounding mode) => Math.Round(value, mode);
    // 
    // Summary:
    // Rounds a double-precision floating-point value to a specified number of fractional
    // digits. A parameter specifies how to round the value if it is midway between
    // two numbers.
    // 
    // Parameters:
    // value:
    // A double-precision floating-point number to be rounded.
    // 
    // digits:
    // The number of fractional digits in the return value.
    // 
    // mode:
    // Specification for how to round value if it is midway between two other numbers.
    // 
    // Returns:
    // The number nearest to value that has a number of fractional digits equal to digits.
    // If value has fewer fractional digits than digits, value is returned unchanged.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // digits is less than 0 or greater than 15.
    // 
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    public static double RoundValue(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);

}

