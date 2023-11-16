// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;

namespace LTRLib.LTRGeneric;

/// <summary>
/// For more information cf. http://en.wikipedia.org/wiki/Verhoeff_algorithm
/// Dihedral Group: http://mathworld.wolfram.com/DihedralGroup.html
/// </summary>
/// <remarks></remarks>
public static class Verhoeff
{
    // The multiplication table
    private static readonly int[,] d =
    {
        { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 0, 6, 7, 8, 9, 5 },
        { 2, 3, 4, 0, 1, 7, 8, 9, 5, 6 },
        { 3, 4, 0, 1, 2, 8, 9, 5, 6, 7 },
        { 4, 0, 1, 2, 3, 9, 5, 6, 7, 8 },
        { 5, 9, 8, 7, 6, 0, 4, 3, 2, 1 },
        { 6, 5, 9, 8, 7, 1, 0, 4, 3, 2 },
        { 7, 6, 5, 9, 8, 2, 1, 0, 4, 3 },
        { 8, 7, 6, 5, 9, 3, 2, 1, 0, 4 },
        { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
    };

    // The permutation table
    private static readonly int[,] p =
    {
        { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 5, 7, 6, 2, 8, 3, 0, 9, 4 },
        { 5, 8, 0, 3, 7, 9, 6, 1, 4, 2 },
        { 8, 9, 1, 6, 0, 4, 3, 5, 2, 7 },
        { 9, 4, 5, 3, 1, 2, 6, 8, 7, 0 },
        { 4, 2, 8, 6, 5, 7, 3, 9, 0, 1 },
        { 2, 7, 9, 3, 8, 0, 6, 4, 1, 5 },
        { 7, 0, 4, 6, 9, 1, 3, 2, 5, 8 }
    };

    // The inverse table
    private static readonly int[] inv = [0, 4, 3, 2, 1, 5, 6, 7, 8, 9];

    /// <summary>
    /// Validates that an entered number is Verhoeff compliant.
    /// </summary>
    /// <param name="num"></param>
    /// <returns>True if Verhoeff compliant, otherwise false</returns>
    /// <remarks>Make sure the check digit is the last one!</remarks>
    public static bool ValidateVerhoeff(ReadOnlySpan<char> num)
    {
        var c = 0;
        var myArray = StringToReversedIntArray(num);

        for (int i = 0, loopTo = myArray.Length - 1; i <= loopTo; i++)
        {
            c = d[c, p[i % 8, myArray[i]]];
        }

        return c == 0;
    }

    /// <summary>
    /// For a given number generates a Verhoeff digit
    /// </summary>
    /// <param name="num"></param>
    /// <returns>Verhoeff check digit as char</returns>
    /// <remarks>Append this check digit to num</remarks>
    public static char GenerateVerhoeff(ReadOnlySpan<char> num)
    {
        var c = 0;
        var myArray = StringToReversedIntArray(num);

        for (int i = 0, loopTo = myArray.Length - 1; i <= loopTo; i++)
        {
            c = d[c, p[(i + 1) % 8, myArray[i]]];
        }

        return (char)(inv[c] + '0');
    }

    /// <summary>
    /// Converts a string to a reversed integer array.
    /// </summary>
    /// <param name="str"></param>
    /// <returns>Reversed integer array</returns>
    /// <remarks></remarks>
    private static int[] StringToReversedIntArray(ReadOnlySpan<char> str)
    {
        var myArray = new int[str.Length];

        for (int i = 0, loopTo = str.Length - 1; i <= loopTo; i++)
        {
            myArray[i] = str[str.Length - i - 1] - '0';
        }

        return myArray;
    }
}

#endif
