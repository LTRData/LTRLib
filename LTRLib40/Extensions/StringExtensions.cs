// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security;
using System.Text;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif

namespace LTRLib.Extensions;

public static class StringExtensions
{
    public static string Center(this string Msg, int Width, char FillChar)
    {
        if (string.IsNullOrEmpty(Msg))
        {
            return new string(FillChar, Width);
        }
        else if (Msg.Length == Width)
        {
            return Msg;
        }
        else if (Msg.Length > Width)
        {
            return Msg.Substring((Msg.Length - Width) / 2, Width);
        }
        else
        {
            var FillStr = new string(FillChar, (Width - Msg.Length) / 2);
            return $"{FillStr}{Msg}{FillStr}";
        }
    }

    public static string Center(this string Msg, int Width) => Msg.Center(Width, ' ');

    /// <summary>
    /// Left adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="Width"></param>
    /// <param name="FillChar"></param>
    public static string LeftAdjust(this string Msg, int Width, char FillChar) => Msg.Nz().PadRight(Width, FillChar).Substring(0, Width);

    /// <summary>
    /// Left adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="Width"></param>
    public static string LeftAdjust(this string Msg, int Width) => Msg.Nz().PadRight(Width).Substring(0, Width);

    /// <summary>
    /// Right adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="Width"></param>
    /// <param name="FillChar"></param>
    public static string RightAdjust(this string Msg, int Width, char FillChar)
    {
        var withBlock = Msg.Nz().PadLeft(Width, FillChar);
        return withBlock.Substring(withBlock.Length - Width);
    }

    /// <summary>
    /// Right adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="Width"></param>
    public static string RightAdjust(this string Msg, int Width)
    {
        var withBlock = Msg.Nz().PadLeft(Width);
        return withBlock.Substring(withBlock.Length - Width);
    }

    /// <summary>
    /// Adjusts a string to a display with specified width and height by truncating or padding to
    /// fill each line with complete words and spaces. This method ensures that the resulting
    /// string is exactly height * width characters long.
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="Width">Display width</param>
    /// <param name="Height">Display height</param>
    /// <param name="WordDelimiter">Word separator character.</param>
    /// <param name="FillChar">Fill character.</param>
    public static string BoxFormat(this string Msg, int Width, int Height, char WordDelimiter = ' ', char FillChar = ' ')
    {

        var Result = new StringBuilder(Height * Width);
        var Line = new StringBuilder(Width);

        foreach (var word in Msg.Nz().Split(WordDelimiter))
        {
            var Word = word;

            if (Word.Length > Width)
            {
                Word = Word.Remove(Width);
            }

            if (Word.Length + Line.Length > Width)
            {
                Result.Append(Line.ToString().PadRight(Width, FillChar));
                Line.Length = 0;
            }

            Line.Append(Word);
            if (Line.Length >= Width)
            {
                Result.Append(Line.ToString().LeftAdjust(Width, FillChar));
                Line.Length = 0;
            }
            else
            {
                Line.Append(WordDelimiter);
            }
        }

        Result.Append(Line);

        return Result.ToString().LeftAdjust(Height * Width, FillChar);

    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Adjusts a string to a display with specified width by wrapping complete words.
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="LineWidth">Display width. If omitted, defaults to console window width.</param>
    /// <param name="WordDelimiter">Word separator character.</param>
    /// <param name="FillChar">Fill character.</param>
    [SecuritySafeCritical]
    public static string LineFormat(this string Msg, int? LineWidth = default, char WordDelimiter = ' ', char FillChar = ' ')
    {
        int Width;

        if (LineWidth.HasValue)
        {
            Width = LineWidth.Value;
        }

        else if (Console.IsOutputRedirected)
        {
            Width = 79;
        }
        else
        {
            Width = Console.WindowWidth - 1;

        }

        var origLines = Msg.Nz().Replace("\r", "").Split('\n');

        var resultLines = new List<string>(origLines.Length);

        foreach (var origLine in origLines)
        {
            var Result = new StringBuilder();

            var Line = new StringBuilder(Width);

            foreach (var Word in origLine.Split(WordDelimiter))
            {
                if (Word.Length >= Width)
                {
                    Result.AppendLine(Word);
                    continue;
                }

                if (Word.Length + Line.Length >= Width)
                {
                    Result.AppendLine(Line.ToString());
                    Line.Length = 0;
                }

                if (Line.Length > 0)
                {
                    Line.Append(WordDelimiter);
                }

                Line.Append(Word);
            }

            if (Line.Length > 0)
            {
                Result.Append(Line);
            }

            resultLines.Add(Result.ToString());
        }

        return string.Join(Environment.NewLine, resultLines);
    }

#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static string InitialCapital(this string str, CultureInfo CultureInfo, int MinWordLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ');
        for (int i = 0, loopTo = words.GetUpperBound(0); i <= loopTo; i++)
        {
            if (words[i].Any(char.IsLower))
            {
                continue;
            }

            if (words[i].Length >= MinWordLength)
            {
                words[i] = char.ToUpper(words[i][0], CultureInfo) + words[i].Substring(1).ToLower(CultureInfo);
            }
        }

        str = string.Join(" ", words);

        return str;
    }

    public static string InitialCapitalInvariant(this string str, int MinWordLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ');
        for (int i = 0, loopTo = words.GetUpperBound(0); i <= loopTo; i++)
        {
            if (words[i].Any(char.IsLower))
            {
                continue;
            }

            if (words[i].Length >= MinWordLength)
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1).ToLowerInvariant();
            }
        }

        str = string.Join(" ", words);

        return str;
    }

    public static string InitialCapital(this string str, int MinWordLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ');
        for (int i = 0, loopTo = words.GetUpperBound(0); i <= loopTo; i++)
        {
            if (words[i].Any(char.IsLower))
            {
                continue;
            }

            if (words[i].Length >= MinWordLength)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }

        str = string.Join(" ", words);

        return str;
    }

    public static string InitialCapitalInvariantJoin(this IEnumerable<string> words, int MinWordLength)
    {
        if (words is null)
        {
            return "";
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        return (from word in words
                select word.Any(char.IsLower) || word.Length < MinWordLength ? word : char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant()).Join();
    }

    public static string InitialCapitalJoin(this IEnumerable<string> words, int MinWordLength)
    {
        if (words is null)
        {
            return "";
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        return (from word in words
                select word.Any(char.IsLower) || word.Length < MinWordLength ? word : char.ToUpper(word[0]) + word.Substring(1).ToLower()).Join();
    }

#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP

    public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

    public static string Join(this IEnumerable<string> strArray, char separator) => string.Join(separator, strArray);

    public static string Join(this IEnumerable<string> strArray) => string.Join(' ', strArray);

#elif NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

    public static string Join(this IEnumerable<string> strArray, char separator) => string.Join(separator.ToString(), strArray);

    public static string Join(this IEnumerable<string> strArray) => string.Join(" ", strArray);

#elif NET35_OR_GREATER

    public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, strings.ToArray());

    public static string Join(this IEnumerable<string> strArray, char separator) => string.Join(separator.ToString(), strArray.ToArray());

    public static string Join(this IEnumerable<string> strings) => string.Join(" ", strings.ToArray());

#else

    public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, new List<string>(strings).ToArray());

    public static string Join(this IEnumerable<string> strArray, char separator) => string.Join(separator.ToString(), new List<string>(strArray).ToArray());

    public static string Join(this IEnumerable<string> strings) => string.Join(" ", new List<string>(strings).ToArray());

#endif

    public static string Join(this string[] strArray, string separator) => string.Join(separator, strArray);

    public static string Join(this string[] strArray) => string.Join(" ", strArray);

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP

    public static bool Contains(this string str, string value, StringComparison comparisonType) => str.IndexOf(value, comparisonType) >= 0;

    public static string[] Split(this string str, char delimiter, StringSplitOptions options) => str.Split(new[] { delimiter }, options);

    public static string[] Split(this string str, string delimiter, StringSplitOptions options) => str.Split(new[] { delimiter }, options);

    public static string[] Split(this string str, char delimiter) => str.Split(new[] { delimiter });

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Not supported in this version")]
    public static string Replace(this string str, string from, string replaceWith, StringComparison comparisonType) => str.Replace(from, replaceWith);

#endif

#if NETFRAMEWORK || (NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)

    public static byte[] GetBytes(this Encoding encoding, string s, int index, int count) => encoding.GetBytes(s.ToCharArray(index, count));

#endif
}

