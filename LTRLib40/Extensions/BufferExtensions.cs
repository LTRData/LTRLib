/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using System.Globalization;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

namespace LTRLib.Extensions;

public static class BufferExtensions
{
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static T[] RentArray<T>(int size) => System.Buffers.ArrayPool<T>.Shared.Rent(size);

    public static void ReturnArray<T>(T[] array) => System.Buffers.ArrayPool<T>.Shared.Return(array);
#else
    public static T[] RentArray<T>(int size) => new T[size];

    public static void ReturnArray<T>(T[] _) { }
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string? ToHexString(this ICollection<byte> bytes)
    {
        if (bytes is null)
        {
            return null;
        }

        var valuestr = string.Create(bytes.Count << 1,
            bytes,
            (span, bytes) =>
            {
                foreach (var b in bytes)
                {
                    if (b.TryFormat(span, out var value, "x2"))
                    {
                        span = span[value..];
                    }
                }
            });

        return valuestr;
    }

#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP

    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string? ToHexString(this ICollection<byte> bytes)
    {
        if (bytes is null)
        {
            return null;
        }

        var valuestr = new string('\0', bytes.Count << 1);
        var span = MemoryMarshal.AsMemory(valuestr.AsMemory()).Span;
        foreach (var b in bytes)
        {
            if (b.TryFormat(span, out var value, "x2"))
            {
                span = span[value..];
            }
        }

        return valuestr;
    }

#else

    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string? ToHexString(this ICollection<byte> bytes)
    {
        if (bytes is null)
        {
            return null;
        }

        var valuestr = new StringBuilder(bytes.Count << 1);
        foreach (var b in bytes)
        {
            valuestr.Append(b.ToString("x2"));
        }

        return valuestr.ToString();
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP

    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string ToHexString(this ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty)
        {
            return string.Empty;
        }

        var valuestr = new string('\0', bytes.Length << 1);
        var span = MemoryMarshal.AsMemory(valuestr.AsMemory()).Span;
        
        foreach (var b in bytes)
        {
            if (b.TryFormat(span, out var value, "x2"))
            {
                span = span[value..];
            }
        }

        return valuestr;
    }

#endif

    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string? ToHexString(this IEnumerable<byte> bytes)
    {
        if (bytes is null)
        {
            return null;
        }

        var valuestr = new StringBuilder();
        foreach (var b in bytes)
        {
            valuestr.Append(b.ToString("x2"));
        }

        return valuestr.ToString();
    }

    public static void WriteHex(this TextWriter writer, IEnumerable<byte> bytes)
    {
        var i = 0;
        foreach (var line in bytes.FormatHexLines())
        {
            writer.Write(((ushort)(i >> 16)).ToString("X4"));
            writer.Write(' ');
            writer.Write(((ushort)i).ToString("X4"));
            writer.Write("  ");
            writer.WriteLine(line);
            i += 0x10;
        }
    }

    public static IEnumerable<string> FormatHexLines(this IEnumerable<byte> bytes)
    {
        var sb = new StringBuilder(67);
        byte pos = 0;
        foreach (var b in bytes)
        {
            if (pos == 0)
            {
                sb.Append($"                        -                                          ");
            }

            var bstr = b.ToString("X2");
            if ((pos & 8) == 0)
            {
                sb[pos * 3] = bstr[0];
                sb[pos * 3 + 1] = bstr[1];
            }
            else
            {
                sb[2 + pos * 3] = bstr[0];
                sb[2 + pos * 3 + 1] = bstr[1];
            }

            sb[51 + pos] = char.IsControl((char)b) ? '.' : (char)b;

            pos++;
            pos &= 0xf;
            if (pos == 0)
            {
                yield return sb.ToString();
                sb.Length = 0;
            }
        }

        if (sb.Length > 0)
        {
            yield return sb.ToString();
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static ReadOnlySpan<char> AsChars(this byte[] bytes) => MemoryMarshal.Cast<byte, char>(bytes);

    public static ReadOnlySpan<char> AsChars(this Memory<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes.Span);

    public static ReadOnlySpan<char> AsChars(this ReadOnlyMemory<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes.Span);

    public static ReadOnlySpan<char> AsChars(this Span<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes);

    public static ReadOnlySpan<char> AsChars(this ReadOnlySpan<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes);

    public ref struct StringSplitByCharEnumerator
    {
        private readonly char delimiter;
        private readonly StringSplitOptions options;
        private readonly bool reverse;

        private ReadOnlySpan<char> chars;
        private ReadOnlySpan<char> current;

        public ReadOnlySpan<char> Current => current;

        public bool MoveNext() => reverse ? MoveNextReverse() : MoveNextForward();

        public bool MoveNextForward()
        {
            while (!chars.IsEmpty)
            {

                var i = chars.IndexOf(delimiter);
                if (i < 0)
                {
                    i = chars.Length;
                }

                current = chars.Slice(0, i);

                if (i < chars.Length)
                {
                    chars = chars.Slice(i + 1);
                }
                else
                {
                    chars = default;
                }

#if NET5_0_OR_GREATER
                if (options.HasFlag(StringSplitOptions.TrimEntries))
                {
                    current = current.Trim();
                }
#endif

                if (!current.IsEmpty ||
                    !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MoveNextReverse()
        {
            while (!chars.IsEmpty)
            {
                var i = chars.LastIndexOf(delimiter);

                current = i >= 0 ? chars.Slice(i + 1) : chars;

                if (i < 0)
                {
                    chars = default;
                }
                else
                {
                    chars = chars.Slice(0, i);
                }

#if NET5_0_OR_GREATER
                if (options.HasFlag(StringSplitOptions.TrimEntries))
                {
                    current = current.Trim();
                }
#endif

                if (!current.IsEmpty ||
                    !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                {
                    return true;
                }
            }

            return false;
        }

        public ReadOnlySpan<char> First() => MoveNext() ? current : throw new InvalidOperationException();

        public ReadOnlySpan<char> FirstOrDefault() => MoveNext() ? current : default;

        public ReadOnlySpan<char> Last()
        {
            var found = false;
            ReadOnlySpan<char> result = default;

            while (MoveNext())
            {
                found = true;
                result = current;
            }

            if (found)
            {
                return result;
            }

            throw new InvalidOperationException();
        }

        public ReadOnlySpan<char> LastOrDefault()
        {
            var found = false;
            ReadOnlySpan<char> result = default;

            while (MoveNext())
            {
                found = true;
                result = current;
            }

            if (found)
            {
                return result;
            }

            return default;
        }

        public ReadOnlySpan<char> ElementAt(int pos)
        {
            for (var i = 0; i <= pos; i++)
            {
                if (!MoveNext())
                {
                    throw new ArgumentOutOfRangeException(nameof(pos));
                }
            }

            return current;
        }

        public ReadOnlySpan<char> ElementAtOrDefault(int pos)
        {
            for (var i = 0; i <= pos; i++)
            {
                if (!MoveNext())
                {
                    return default;
                }
            }

            return current;
        }

        public StringSplitByCharEnumerator GetEnumerator() => this;

        public StringSplitByCharEnumerator(ReadOnlySpan<char> chars, char delimiter, StringSplitOptions options, bool reverse)
        {
            current = default;
            this.chars = chars;
            this.delimiter = delimiter;
            this.options = options;
            this.reverse = reverse;
        }
    }

    public ref struct StringSplitByStringEnumerator
    {
        private readonly ReadOnlySpan<char> delimiter;
        private readonly StringSplitOptions options;
        private readonly bool reverse;

        private ReadOnlySpan<char> chars;
        private ReadOnlySpan<char> current;

        public ReadOnlySpan<char> Current => current;

        public bool MoveNext() => reverse ? MoveNextReverse() : MoveNextForward();

        public bool MoveNextForward()
        {
            while (!chars.IsEmpty)
            {

                var i = chars.IndexOf(delimiter);
                if (i < 0)
                {
                    i = chars.Length;
                }

                current = chars.Slice(0, i);

                if (i + delimiter.Length <= chars.Length)
                {
                    chars = chars.Slice(i + delimiter.Length);
                }
                else
                {
                    chars = default;
                }

#if NET5_0_OR_GREATER
                if (options.HasFlag(StringSplitOptions.TrimEntries))
                {
                    current = current.Trim();
                }
#endif

                if (!current.IsEmpty ||
                    !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MoveNextReverse()
        {
            while (!chars.IsEmpty)
            {
                var i = chars.LastIndexOf(delimiter);

                current = i >= 0 ? chars.Slice(i + delimiter.Length) : chars;

                if (i < 0)
                {
                    chars = default;
                }
                else
                {
                    chars = chars.Slice(0, i);
                }

#if NET5_0_OR_GREATER
                if (options.HasFlag(StringSplitOptions.TrimEntries))
                {
                    current = current.Trim();
                }
#endif

                if (!current.IsEmpty ||
                    !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                {
                    return true;
                }
            }

            return false;
        }

        public ReadOnlySpan<char> First() => MoveNext() ? current : throw new InvalidOperationException();

        public ReadOnlySpan<char> FirstOrDefault() => MoveNext() ? current : default;

        public ReadOnlySpan<char> Last()
        {
            var found = false;
            ReadOnlySpan<char> result = default;

            while (MoveNext())
            {
                found = true;
                result = current;
            }

            if (found)
            {
                return result;
            }

            throw new InvalidOperationException();
        }

        public ReadOnlySpan<char> LastOrDefault()
        {
            var found = false;
            ReadOnlySpan<char> result = default;

            while (MoveNext())
            {
                found = true;
                result = current;
            }

            if (found)
            {
                return result;
            }

            return default;
        }

        public ReadOnlySpan<char> ElementAt(int pos)
        {
            for (var i = 0; i <= pos; i++)
            {
                if (!MoveNext())
                {
                    throw new ArgumentOutOfRangeException(nameof(pos));
                }
            }

            return current;
        }

        public ReadOnlySpan<char> ElementAtOrDefault(int pos)
        {
            for (var i = 0; i <= pos; i++)
            {
                if (!MoveNext())
                {
                    return default;
                }
            }

            return current;
        }

        public StringSplitByStringEnumerator GetEnumerator() => this;

        public StringSplitByStringEnumerator(ReadOnlySpan<char> chars, ReadOnlySpan<char> delimiter, StringSplitOptions options, bool reverse)
        {
            current = default;
            this.chars = chars;
            this.delimiter = delimiter;
            this.options = options;
            this.reverse = reverse;
        }
    }

    public static StringSplitByCharEnumerator Split(this ReadOnlySpan<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: false);

    public static StringSplitByStringEnumerator Split(this ReadOnlySpan<char> chars, ReadOnlySpan<char> delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: false);

    public static StringSplitByCharEnumerator SplitReverse(this ReadOnlySpan<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: true);

    public static StringSplitByStringEnumerator SplitReverse(this ReadOnlySpan<char> chars, ReadOnlySpan<char> delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: true);

    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        while (!chars.IsEmpty)
        {
            var i = chars.Span.IndexOf(delimiter);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!value.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + 1);
        }
    }

    public static IEnumerable<ReadOnlyMemory<char>> SplitReverse(this ReadOnlyMemory<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        while (!chars.IsEmpty)
        {
            var i = chars.Span.LastIndexOf(delimiter);

            var value = chars.Slice(i + 1);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!value.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i < 0)
            {
                break;
            }

            chars = chars.Slice(0, i);
        }
    }

    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        while (!chars.IsEmpty)
        {
            var i = chars.Span.IndexOf(delimiter.Span);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!value.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + delimiter.Length);
        }
    }

    public static IEnumerable<ReadOnlyMemory<char>> SplitReverse(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        while (!chars.IsEmpty)
        {
            var i = chars.Span.LastIndexOf(delimiter.Span);

            var value = i >= 0 ? chars.Slice(i + delimiter.Length) : chars;

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!value.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i < 0)
            {
                break;
            }

            chars = chars.Slice(0, i);
        }
    }

    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> delimiter, StringSplitOptions options, StringComparison comparison)
    {
        while (!chars.IsEmpty)
        {
            var i = chars.Span.IndexOf(delimiter.Span, comparison);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!value.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + delimiter.Length);
        }
    }

    public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> chars) =>
        chars.Slice(0, chars.Span.TrimEnd().Length);

    public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> chars, char trimChar) =>
        chars.Slice(0, chars.Span.TrimEnd(trimChar).Length);

    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> chars) =>
        chars.Slice(chars.Length - chars.Span.TrimStart().Length);

    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> chars, char trimChar) =>
        chars.Slice(chars.Length - chars.Span.TrimStart(trimChar).Length);

    public static string InitialCapital(this ReadOnlyMemory<char> str, int MinWordLength)
    {
        if (str.IsEmpty)
        {
            return string.Empty;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ').Select(word =>
        {
            if (MemoryMarshal.ToEnumerable(word).Any(char.IsLower))
            {
                return word;
            }

            if (word.Length >= MinWordLength)
            {
                return $"{char.ToUpper(word.Span[0])}{word.Slice(1).ToString().ToLower()}".AsMemory();
            }

            return word;
        });

        return string.Join(" ", words);
    }
#endif

#if NET6_0_OR_GREATER
    public static int GetSequenceHash(this ReadOnlySpan<byte> str)
    {
        var hash = new HashCode();
        hash.AddBytes(str);
        return hash.ToHashCode();
    }
#endif

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static int GetSequenceHash<T>(this ReadOnlySpan<T> str)
    {
        var hash = new HashCode();
        foreach (var element in str)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }

    public static int GetSequenceHash<T>(this Span<T> str)
    {
        var hash = new HashCode();
        foreach (var element in str)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }

    public static int GetSequenceHash<T>(this IEnumerable<T> str)
    {
        var hash = new HashCode();
        foreach (var element in str)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(in T source, int length) =>
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(source), length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> CreateSpan<T>(ref T source, int length) =>
        MemoryMarshal.CreateSpan(ref source, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> AsReadOnlyBytes<T>(in T source) where T : unmanaged =>
        MemoryMarshal.AsBytes(CreateReadOnlySpan(source, 1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> AsBytes<T>(ref T source) where T : unmanaged =>
        MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref source, 1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString<T>(in T source) where T : unmanaged =>
        ToHexString(AsReadOnlyBytes(source));

    public static string ToHexString(this ReadOnlySpan<byte> bytes, ReadOnlySpan<char> delimiter)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        var delimiter_length = delimiter.Length;
        var str = new string('\0', (bytes.Length << 1) + delimiter_length * (bytes.Length - 1));

        var target = MemoryMarshal.AsMemory(str.AsMemory()).Span;

        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i].TryFormat(target.Slice(i * (2 + delimiter_length), 2), out _, "x2");
            if (delimiter_length > 0 && i < bytes.Length - 1)
            {
                delimiter.CopyTo(target[(i * (2 + delimiter_length) + 2)..]);
            }
        }

        return target.ToString();
    }

#elif NET45_OR_GREATER || NETSTANDARD

    public static unsafe ReadOnlySpan<T> CreateReadOnlySpan<T>(in T source, int length) =>
        new(Unsafe.AsPointer(ref Unsafe.AsRef(source)), length);

    public static unsafe Span<T> CreateSpan<T>(ref T source, int length) =>
        new(Unsafe.AsPointer(ref source), length);

    public static string ToHexString(this ReadOnlySpan<byte> data, string? delimiter)
    {
        if (data.IsEmpty)
        {
            return string.Empty;
        }

        var capacity = data.Length << 1;
        if (delimiter is not null)
        {
            capacity += delimiter.Length * (data.Length - 1);
        }

        var result = new StringBuilder(capacity);

        foreach (var b in data)
        {
            if (delimiter is not null && result.Length > 0)
            {
                result.Append(delimiter);
            }

            result.Append(b.ToString("x2", NumberFormatInfo.InvariantInfo));
        }

        return result.ToString();
    }

#endif

    public static unsafe string CreateString(in char source)
    {
        fixed (char* chars = &source)
        {
            return new(chars);
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> ReadNullTerminatedUnicode(this ReadOnlySpan<char> chars)
    {
        var endpos = chars.IndexOfTerminator();
        return chars.Slice(0, endpos);
    }

    /// <summary>
    /// Return position of first empty element, or the entire span length if
    /// no empty elements are found.
    /// </summary>
    /// <param name="buffer">Span to search</param>
    /// <returns>Position of first found empty element or entire span length if none found</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfTerminator<T>(this ReadOnlySpan<T> buffer) where T : unmanaged, IEquatable<T>
    {
        var endpos = buffer.IndexOf(default(T));
        return endpos >= 0 ? endpos : buffer.Length;
    }
#endif

}
