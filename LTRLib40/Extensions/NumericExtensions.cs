/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System.Collections.Generic;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif

namespace LTRLib.Extensions;

/// <summary>
/// Static methods for factorization of integer values.
/// </summary>
public static class NumericExtensions
{
    /// <summary>
    /// Finds greatest common divisor for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Greatest common divisor for values a and b.</returns>
    public static int GreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    /// <summary>
    /// Finds greatest common divisor for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Greatest common divisor for values a and b.</returns>
    public static long GreatestCommonDivisor(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Finds greatest common divisor for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Greatest common divisor for values.</returns>
    public static int GreatestCommonDivisor(this IEnumerable<int> values) =>
        values.Aggregate(GreatestCommonDivisor);

    /// <summary>
    /// Finds greatest common divisor for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Greatest common divisor for values.</returns>
    public static long GreatestCommonDivisor(this IEnumerable<long> values) =>
        values.Aggregate(GreatestCommonDivisor);
#endif

    /// <summary>
    /// Finds least common multiple for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Least common multiple for values a and b.</returns>
    public static int LeastCommonMultiple(int a, int b)
        => a / GreatestCommonDivisor(a, b) * b;

    /// <summary>
    /// Finds least common multiple for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Least common multiple for values a and b.</returns>
    public static long LeastCommonMultiple(long a, long b)
        => a / GreatestCommonDivisor(a, b) * b;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Finds least common multiple for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Least common multiple for values.</returns>
    public static int LeastCommonMultiple(this IEnumerable<int> values) =>
        values.Aggregate(LeastCommonMultiple);

    /// <summary>
    /// Finds least common multiple for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Least common multiple for values.</returns>
    public static long LeastCommonMultiple(this IEnumerable<long> values) =>
        values.Aggregate(LeastCommonMultiple);
#endif

    /// <summary>
    /// Returns a sequence of prime factors for a value.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Sequence of prime factors</returns>
    public static IEnumerable<int> PrimeFactors(this int value)
    {
        var z = 2;

        while (checked(z * z) <= value)
        {
            if (value % z == 0)
            {
                yield return z;
                value /= z;
            }
            else
            {
                z++;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }

    /// <summary>
    /// Returns a sequence of prime factors for a value.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Sequence of prime factors</returns>
    public static IEnumerable<long> PrimeFactors(this long value)
    {
        var z = 2L;

        while (checked(z * z) <= value)
        {
            if (value % z == 0)
            {
                yield return z;
                value /= z;
            }
            else
            {
                z++;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    private static readonly List<long> _primetable = [2, 3];

    /// <summary>
    /// Calculates a sequence of prime numbers as long as the sequence is
    /// iterated through. If an overflow occurs, an exception is thrown.
    /// </summary>
    /// <returns>A sequence of prime numbers.</returns>
    public static IEnumerable<long> EnumeratePrimeNumbers()
    {
        lock (_primetable)
        {
            var value = 0L;

            for (var i = 0; i < _primetable.Count; i++)
            {
                value = _primetable[i];

                yield return value;
            }

            for (; ; )
            {
                value = checked(value + 2);

                if (!_primetable
                    .Skip(1)
                    .TakeWhile(prime => prime * prime <= value)
                    .Any(prime => value % prime == 0))
                {
                    _primetable.Add(value);

                    yield return value;
                }
            }
        }
    }

    public static int Multiply(this IEnumerable<int> factors) =>
        factors.Aggregate((prod, factor) => checked(prod * factor));

    public static long Multiply(this IEnumerable<long> factors) =>
        factors.Aggregate((prod, factor) => checked(prod * factor));
#endif
}

