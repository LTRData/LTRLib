using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Buffers;
#endif
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

    private static readonly string[] lineBreaks = ["\r\n", "\r", "\n"];
    private static readonly Type[] typesString = [typeof(string)];
    private static readonly Type[] typesStringAndFormatProvider = [typeof(string), typeof(IFormatProvider)];

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
                return Method?.Invoke(null, [data, DateTimeFormatInfo.CurrentInfo]);
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
                return Method?.Invoke(null, [data, NumberFormatInfo.CurrentInfo]);
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
                return t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, typesStringAndFormatProvider, null)?.Invoke(null, [data, format]);
            }
            catch (Exception ex) when (ex.InnerException is not null)
            {
                throw ex.InnerException;
            }
        }
    }

    /// <summary>
    /// Translates character spans to a string containing all characters in spans. The string AFPR will
    /// for example return ABCDEFPQR.
    /// </summary>
    /// <param name="charSpan">String of character pairs describing first and last characters of each
    /// span.</param>
    public static string TranslateCharSpanToCharList(string charSpan)
    {
        var CharList = new StringBuilder();
        for (int i = 0, loopTo = charSpan.Length - 2; i <= loopTo; i += 2)
        {
            for (short j = (short)charSpan[i], loopTo1 = (short)charSpan[i + 1]; j <= loopTo1; j++)
            {
                CharList.Append(Convert.ToChar(j));
            }
        }

        return CharList.ToString();
    }

    /// <summary>
    /// Returns characters up to excluding first found NULL character.
    /// </summary>
    /// <param name="str">NULL terminated string</param>
    public static string TrimNullString(string str)
    {
        var toPos = str.IndexOf(new char());
        if (toPos == -1)
        {
            return str;
        }
        else
        {
            return str.Remove(toPos);
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

    private static readonly string[] _ValidSwedishPersonalIdNumber_Formats = ["yyyyMMdd", "yyMMdd"];

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

}
