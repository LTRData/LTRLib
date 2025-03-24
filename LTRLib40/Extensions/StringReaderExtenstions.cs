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

public static class StringReaderExtenstions
{
    private static readonly Action<StringReader, int> SetStringReaderPosition;

    private static readonly Func<StringReader, int> GetStringReaderPosition;

    private static readonly Func<StringReader, string> GetStringReaderString;

    static StringReaderExtenstions()
    {
        var paramTarget = Expression.Parameter(typeof(StringReader));

        var fieldInfoPos = typeof(StringReader).GetField("_pos", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException("StringReader._pos missing", "_pos");

        var fieldPos = Expression.Field(paramTarget, fieldInfoPos);
        var exprGetPos = Expression.Lambda<Func<StringReader, int>>(fieldPos, paramTarget);
        GetStringReaderPosition = exprGetPos.Compile();

        var fieldInfoString = typeof(StringReader).GetField("_s", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException("StringReader._s missing", "_s");

        var fieldString = Expression.Field(paramTarget, fieldInfoString);
        var exprGetString = Expression.Lambda<Func<StringReader, string>>(fieldString, paramTarget);
        GetStringReaderString = exprGetString.Compile();

        var paramValueSetPos = Expression.Parameter(typeof(int));
        var assignPos = Expression.Assign(fieldPos, paramValueSetPos);
        var exprSetPos = Expression.Lambda<Action<StringReader, int>>(assignPos, paramTarget, paramValueSetPos);
        SetStringReaderPosition = exprSetPos.Compile();
    }

    public static int GetPosition(this StringReader reader) => GetStringReaderPosition(reader);

    public static string GetString(this StringReader reader) => GetStringReaderString(reader);

    public static void SetPosition(this StringReader reader, int position) => SetStringReaderPosition(reader, position);
}

#endif
