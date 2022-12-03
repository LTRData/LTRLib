using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0056 // Use index operator
#pragma warning disable IDE0057 // Use range operator

namespace LTRLib.LTRGeneric;

public static class StringSupport
{
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static bool IsOSWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
    public static bool IsOSWindows { get; } = true;
#endif

    private static readonly string[] lineBreaks = new[] { "\r\n", "\r", "\n" };
    private static readonly Type[] typesString = new[] { typeof(string) };
    private static readonly Type[] typesStringAndFormatProvider = new[] { typeof(string), typeof(IFormatProvider) };

    /// <summary>
    /// Parses a string and returns value as specified type
    /// </summary>
    /// <param name="data"></param>
    /// <param name="t"></param>
    public static object? ParseValueType(string data, Type t)
    {
        if (ReferenceEquals(t, typeof(string)))
        {
            return data;
        }
        else if (ReferenceEquals(t, typeof(char)))
        {
            if (!string.IsNullOrEmpty(data))
            {
                return data[0];
            }
            else
            {
                return default;
            }
        }
        else if (t.IsArray)
        {
            if (ReferenceEquals(t.GetElementType(), typeof(string)))
            {
                return data.Split(lineBreaks, StringSplitOptions.None);
            }

            var StringArray = data.Split(lineBreaks, StringSplitOptions.RemoveEmptyEntries);
            var NewArray = Array.CreateInstance(t.GetElementType()!, StringArray.Length);
            for (int i = 0, loopTo = StringArray.Length - 1; i <= loopTo; i++)
            {
                NewArray.SetValue(ParseValueType(StringArray[i], t.GetElementType()!), i);
            }

            return NewArray;
        }
        else if (string.IsNullOrEmpty(data))
        {
            return null;
        }
        else if (ReferenceEquals(t, typeof(bool)) || ReferenceEquals(t, typeof(TimeSpan)))
        {
            try
            {
                var Method = t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typesString, null);
                return Method?.Invoke(null, new[] { data });
            }
            catch (Exception ex) when (ex.InnerException is not null)
            {
                throw ex.InnerException;
            }
        }
        else if (ReferenceEquals(t, typeof(DateTime)))
        {
            try
            {
                var Method = t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typesStringAndFormatProvider, null);
                return Method?.Invoke(null, new object[] { data, DateTimeFormatInfo.CurrentInfo });
            }
            catch (Exception ex) when (ex.InnerException is not null)
            {
                throw ex.InnerException;
            }
        }
        else
        {
            try
            {
                var Method = t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typesStringAndFormatProvider, null);
                return Method?.Invoke(null, new object[] { data, NumberFormatInfo.CurrentInfo });
            }
            catch (Exception ex) when (ex.InnerException is not null)
            {
                throw ex.InnerException;
            }
        }
    }

    /// <summary>
    /// Parses a string and returns value as specified type
    /// </summary>
    /// <param name="data"></param>
    /// <param name="t"></param>
    /// <param name="format"></param>
    public static object? ParseValueType(string data, Type t, IFormatProvider format)
    {
        if (ReferenceEquals(t, typeof(string)))
        {
            return data;
        }
        else if (ReferenceEquals(t, typeof(char)))
        {
            if (!string.IsNullOrEmpty(data))
            {
                return data[0];
            }
            else
            {
                return new char();
            }
        }
        else if (t.IsArray)
        {
            if (ReferenceEquals(t.GetElementType(), typeof(string)))
            {
                return data.Split(lineBreaks, StringSplitOptions.None);
            }
            
            var StringArray = data.Split(lineBreaks, StringSplitOptions.RemoveEmptyEntries);
            var NewArray = Array.CreateInstance(t.GetElementType()!, StringArray.Length);
            
            for (int i = 0, loopTo = StringArray.Length - 1; i <= loopTo; i++)
            {
                NewArray.SetValue(ParseValueType(StringArray[i], t.GetElementType()!, format), i);
            }

            return NewArray;
        }
        else if (string.IsNullOrEmpty(data))
        {
            return null;
        }
        else if (ReferenceEquals(t, typeof(bool)) || ReferenceEquals(t, typeof(TimeSpan)))
        {
            try
            {
                return t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typesString, null)?.Invoke(null, new[] { data });
            }
            catch (Exception ex) when (ex.InnerException is not null)
            {
                throw ex.InnerException;
            }
        }
        else
        {
            try
            {
                return t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typesStringAndFormatProvider, null)?.Invoke(null, new object[] { data, format });
            }
            catch (Exception ex) when (ex.InnerException is not null)
            {
                throw ex.InnerException;
            }
        }
    }

    public static long? ParseSuffixedSize(string Str)
    {
        long? ParseSuffixedSizeRet;

        if (string.IsNullOrEmpty(Str))
        {
            return default;
        }

        if (Str.StartsWith("0x", StringComparison.Ordinal) || Str.StartsWith("&H", StringComparison.Ordinal))
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            return long.Parse(Str.AsSpan(2), NumberStyles.AllowHexSpecifier);
#else
            return long.Parse(Str.Substring(2), NumberStyles.AllowHexSpecifier);
#endif
        }

        var Suffix = Str[Str.Length - 1];
        if (char.IsLetter(Suffix))
        {
            switch (char.ToUpper(Suffix))
            {
                case 'T':
                        ParseSuffixedSizeRet = 1024L << 30;
                        break;

                case 'G':
                        ParseSuffixedSizeRet = 1024L << 20;
                        break;

                case 'M':
                        ParseSuffixedSizeRet = 1024L << 10;
                        break;

                case 'K':
                        ParseSuffixedSizeRet = 1024L;
                        break;

                default:
                        throw new FormatException($"Bad suffix: {Suffix}");
            }

            Str = Str.Remove(Str.Length - 1);
        }
        else
        {
            ParseSuffixedSizeRet = 1L;
        }

        ParseSuffixedSizeRet *= long.Parse(Str, NumberStyles.Any);
        return ParseSuffixedSizeRet;
    }

    public static byte[] ParseHexString(string str)
    {
        var bytes = new byte[(str.Length >> 1)];

        for (int i = 0, loopTo = bytes.Length - 1; i <= loopTo; i++)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            bytes[i] = byte.Parse(str.AsSpan(i << 1, 2), NumberStyles.HexNumber);
#else
            bytes[i] = byte.Parse(str.Substring(i << 1, 2), NumberStyles.HexNumber);
#endif
        }

        return bytes;
    }

    public static IEnumerable<byte> ParseHexString(IEnumerable<char> str)
    {
        var buffer = new char[2];

        foreach (var c in str)
        {
            if (buffer[0] == default(char))
            {
                buffer[0] = c;
            }
            else
            {
                buffer[1] = c;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
                yield return byte.Parse(buffer, NumberStyles.HexNumber);
#else
                yield return byte.Parse(new string(buffer), NumberStyles.HexNumber);
#endif

                Array.Clear(buffer, 0, 2);
            }
        }
    }

    public static byte[] ParseHexString(string str, int offset, int count)
    {
        var bytes = new byte[(count >> 1)];

        for (int i = 0, loopTo = count - 1; i <= loopTo; i++)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            bytes[i] = byte.Parse(str.AsSpan(i + offset << 1, 2), NumberStyles.HexNumber);
#else
            bytes[i] = byte.Parse(str.Substring(i + offset << 1, 2), NumberStyles.HexNumber);
#endif
        }

        return bytes;
    }

    /// <summary>
    /// Translates character spans to a string containing all characters in spans. The string AFPR will
    /// for example return ABCDEFPQR.
    /// </summary>
    /// <param name="CharSpan">String of character pairs describing first and last characters of each
    /// span.</param>
    public static string TranslateCharSpanToCharList(string CharSpan)
    {
        var CharList = new StringBuilder();
        for (int i = 0, loopTo = CharSpan.Length - 2; i <= loopTo; i += 2)
        {
            for (short j = (short)CharSpan[i], loopTo1 = (short)CharSpan[i + 1]; j <= loopTo1; j++)
            {
                CharList.Append(Convert.ToChar(j));
            }
        }

        return CharList.ToString();
    }

    /// <summary>
    /// Returns characters up to excluding first found NULL character.
    /// </summary>
    /// <param name="Str">NULL terminated string</param>
    public static string TrimNullString(string Str)
    {
        var ToPos = Str.IndexOf(new char());
        if (ToPos == -1)
        {
            return Str;
        }
        else
        {
            return Str.Remove(ToPos);
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Calculate Verhoeff checksum character for numeric string. Non-numeric characters are ignored.
    /// </summary>
    /// <param name="Str">String containing numeric characters.</param>
    /// <returns>Numeric characters in input string with calculated checksum character appended.</returns>

    public static string AppendVerhoeff(IEnumerable<char> Str)
    {
        var Digits = Str.Where(char.IsDigit).ToArray();
        return $"{Digits.AsMemory()}{Verhoeff.GenerateVerhoeff(Digits)}";
    }

    /// <summary>
    /// Calculate Luhn checksum character for numeric string. Non-numeric characters are ignored.
    /// </summary>
    /// <param name="Str">String containing numeric characters.</param>
    /// <param name="PG">Generate PlusGirot character counted OCR code with Luhn checksum if True, 
    /// append Luhn checksum character only if False.</param>
    /// <returns>Numeric characters in input string with calculated checksum character appended.</returns>

    public static string AppendLuhn(IEnumerable<char> Str, bool PG)
    {
        var Digits = Str.Where(char.IsDigit).ToArray();

        if (PG)
        {
            Digits = Digits.Append((char)(((Digits.Length + 2) % 10) + '0')).ToArray();
        }

        return $"{Digits.AsMemory()}{CalculateLuhn(Digits)}";
    }

    /// <summary>
    /// Calculate and returns Luhn checksum character for numeric string.
    /// </summary>
    /// <param name="Str">String containing numeric characters. An exception will occur if string contains non-numeric characters.</param>
    /// <returns>Calculated Luhn checksum character.</returns>

    public static char CalculateLuhn(ReadOnlySpan<char> Str)
    {
        var Checksum = default(int);

        for (var i = Str.Length - 1; i >= 0; i -= 1)
        {
            var value = (Str[i] - '0') * (1 + (Str.Length - i & 1));

            if (value >= 10)
            {
                Checksum++;
            }

            Checksum += value;
        }

        Checksum = (10 - Checksum % 10) % 10;

        return (char)(Checksum + '0');
    }

    /// <summary>
    /// Verifies last character as Luhn checksum character for numeric string.
    /// </summary>
    /// <param name="Str">String containing numeric characters. An exception will occur if string contains non-numeric characters.</param>
    /// <returns>Boolean value indicating whether or not last character in input string is a valid Luhn checksum character.</returns>

    public static bool ValidateLuhn(ReadOnlySpan<char> Str) => CalculateLuhn(Str.Slice(0, Str.Length - 1)).Equals(Str[Str.Length - 1]);

    /// <summary>
    /// Calculates longitudinal redundancy check number for string.
    /// </summary>

    public static int CalculateLRC(string Str)
    {
        var CalculateLRCRet = 0;
        for (int i = 0, loopTo = Str.Length - 1; i <= loopTo; i++)
        {
            CalculateLRCRet ^= Str[i];
        }

        return CalculateLRCRet;
    }

    /// <summary>
    /// Ensures that a character sequence contains a valid Swedish Id number.
    /// </summary>
    /// <returns>Returns valid id number, or Nothing if input could not be successfully parsed.</returns>

    public static string? ValidSwedishIdNumber(IEnumerable<char> Id)
    {
        if (Id is null)
        {
            return null;
        }

        var Digits = Id.Where(char.IsDigit).ToArray();

        if (Digits.Length is not 10 and not 12)
        {
            return null;
        }

        if (ValidateLuhn(Digits.AsSpan(Digits.Length - 10, 10)))
        {
            return new(Digits);
        }
        else
        {
            return null;
        }
    }

    private static readonly string[] _ValidSwedishPersonalIdNumber_Formats = { "yyyyMMdd", "yyMMdd" };

    /// <summary>
    /// Ensures that a character sequence contains a valid personal Swedish Id number.
    /// </summary>
    /// <returns>Returns valid personal id number, or Nothing if input could not be successfully parsed.</returns>

    public static string? ValidSwedishPersonalIdNumber(IEnumerable<char> Id)
    {

        var Result = ValidSwedishIdNumber(Id);
        if (Result is null)
        {
            return null;
        }

        if (DateTime.TryParseExact(Result.Remove(Result.Length - 4), _ValidSwedishPersonalIdNumber_Formats, null, DateTimeStyles.None, out _))
        {
            return Result;
        }
        else
        {
            return null;
        }

    }
#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static Dictionary<string, string[]> ParseCommandLine(IEnumerable<string> args, StringComparer comparer)
    {
        var dict = ParseCommandLineParameter(args).GroupBy(item => item.Key, item => item.Value, comparer).ToDictionary(item => item.Key, item => item.SelectMany(i => i).ToArray(), comparer);

        return dict;
    }

    public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ParseCommandLineParameter(IEnumerable<string> args)
    {
        var switches_finished = false;

        foreach (var arg in args)
        {
            if (switches_finished)
            {
            }
            else if (arg.Length == 0 || arg.Equals("-", StringComparison.Ordinal))
            {
                switches_finished = true;
            }
            else if (arg.Equals("--", StringComparison.Ordinal))
            {
                switches_finished = true;
                continue;
            }
            else if (arg.StartsWith("--", StringComparison.Ordinal) || IsOSWindows && arg.StartsWith("/", StringComparison.Ordinal))
            {
                var namestart = 1;
                if (arg[0] == '-')
                {
                    namestart = 2;
                }

                var valuepos = arg.IndexOf('=');
                if (valuepos < 0)
                {
                    valuepos = arg.IndexOf(':');
                }

                string name;
                IEnumerable<string> value;

                if (valuepos >= 0)
                {
                    name = arg.Substring(namestart, valuepos - namestart);
                    value = Enumerable.Repeat(arg.Substring(valuepos + 1), 1);
                }
                else
                {
                    name = arg.Substring(namestart);
                    value = Enumerable.Empty<string>();
                }

                yield return new KeyValuePair<string, IEnumerable<string>>(name, value);
            }
            else if (arg.StartsWith("-", StringComparison.Ordinal))
            {
                for (int i = 1, loopTo = arg.Length - 1; i <= loopTo; i++)
                {
                    var name = arg.Substring(i, 1);
                    IEnumerable<string> value;

                    if (i + 1 < arg.Length && (arg[i + 1] == '=' || arg[i + 1] == ':'))
                    {
                        value = Enumerable.Repeat(arg.Substring(i + 2), 1);
                        yield return new KeyValuePair<string, IEnumerable<string>>(name, value);
                        break;
                    }

                    value = Enumerable.Empty<string>();
                    yield return new KeyValuePair<string, IEnumerable<string>>(name, value);
                }
            }
            else
            {
                switches_finished = true;
            }

            if (switches_finished)
            {
                yield return new KeyValuePair<string, IEnumerable<string>>(string.Empty, Enumerable.Repeat(arg, 1));
            }
        }
    }

#endif

}
