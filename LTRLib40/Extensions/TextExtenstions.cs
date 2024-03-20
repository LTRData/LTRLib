/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LTRLib.Extensions;

public static class TextExtensions
{
    private static readonly Action<StringReader, int> SetStringReaderPosition;

    private static readonly Action<StringReader, int> SetStringReaderLength;

    private static readonly Func<StringReader, int> GetStringReaderPosition;

    private static readonly Func<StringReader, string> GetStringReaderString;

    private static readonly Func<StringReader, int> GetStringReaderLength;

    static TextExtensions()
    {
        var paramTarget = Expression.Parameter(typeof(StringReader));

        var fieldInfoPos = typeof(StringReader).GetField("_pos", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException("StringReader._pos missing", "_pos");

        var fieldPos = Expression.Field(paramTarget, fieldInfoPos);
        var exprGetPos = Expression.Lambda<Func<StringReader, int>>(fieldPos, paramTarget);
        GetStringReaderPosition = exprGetPos.Compile();

        var fieldInfoLength = typeof(StringReader).GetField("_length", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException("StringReader._length missing", "_length");
        
        var fieldLength = Expression.Field(paramTarget, fieldInfoLength);
        var exprGetLength = Expression.Lambda<Func<StringReader, int>>(fieldLength, paramTarget);
        GetStringReaderLength = exprGetLength.Compile();

        var fieldInfoString = typeof(StringReader).GetField("_s", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException("StringReader._s missing", "_s");

        var fieldString = Expression.Field(paramTarget, fieldInfoString);
        var exprGetString = Expression.Lambda<Func<StringReader, string>>(fieldString, paramTarget);
        GetStringReaderString = exprGetString.Compile();

        var paramValueSetPos = Expression.Parameter(typeof(int));
        var assignPos = Expression.Assign(fieldPos, paramValueSetPos);
        var exprSetPos = Expression.Lambda<Action<StringReader, int>>(assignPos, paramTarget, paramValueSetPos);
        SetStringReaderPosition = exprSetPos.Compile();

        var paramValueSetLen = Expression.Parameter(typeof(int));
        var assignLen = Expression.Assign(fieldPos, paramValueSetLen);
        var exprSetLen = Expression.Lambda<Action<StringReader, int>>(assignLen, paramTarget, paramValueSetLen);
        SetStringReaderLength = exprSetLen.Compile();
    }

    public static int GetPosition(this StringReader reader) => GetStringReaderPosition(reader);

    public static int GetLength(this StringReader reader) => GetStringReaderLength(reader);

    public static string GetString(this StringReader reader) => GetStringReaderString(reader);

    public static void SetPosition(this StringReader reader, int position) => SetStringReaderPosition(reader, position);

    public static void SetLength(this StringReader reader, int stringLength) => SetStringReaderLength(reader, stringLength);

    public static int IndexOfAny(this string sample, IEnumerable<string> values, int startIndex, int count, StringComparison comparisonType)
    {
        var result = values
            .Select(val => sample.IndexOf(val, startIndex, count, comparisonType))
            .Where(pos => pos >= 0)
            .ToArray();

        if (result.Length == 0)
        {
            return -1;
        }

        return result.Min();
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [return: NotNullIfNotNull(nameof(obj))]
#endif
    public static string? ToMembersString(this object? obj)
    {
        if (obj is null)
        {
            return "{null}";
        }
        else
        {
            return (typeof(Reflection.MembersStringParser<>)
                .MakeGenericType(obj.GetType())
                .GetMethod("ToString", BindingFlags.Public | BindingFlags.Static)!
                .Invoke(null, [obj]) as string)
                ?? obj.ToString()
                ?? obj.GetType().FullName!;
        }
    }

    public static string ToMembersString<T>(this T o) where T : struct => Reflection.MembersStringParser<T>.ToString(o);
}

#endif
