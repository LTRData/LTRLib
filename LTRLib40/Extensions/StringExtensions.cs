/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Linq;

namespace LTRLib.Extensions;

public static class StringExtensions
{
    public static int IndexOfAny(this string sample, IEnumerable<string> values, int startIndex, int count, StringComparison comparisonType)
    {
        var result = values
            .Select(val => new int?(sample.IndexOf(val, startIndex, count, comparisonType)))
            .Where(pos => pos!.Value >= 0)
            .Min();

        return result ?? -1;
    }
}

#endif
