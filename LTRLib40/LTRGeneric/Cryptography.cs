// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using LTRLib.Extensions;

namespace LTRLib.LTRGeneric;

public static class Cryptography
{
    private static class HashProviderCache<T> where T : HashAlgorithm, new()
    {
        [ThreadStatic]
        private static T? _Instance;

        public static T Instance => _Instance ??= new T();
    }

#if NET6_0_OR_GREATER
    private static Random Random => Random.Shared;
#else
    [ThreadStatic]
    private static Random? random;

    private static Random Random => random ??= new Random();
#endif

    /// <summary>
    /// Generates a random challenge/response pair for a given shared key.
    /// </summary>
    /// <param name="SharedKey">Shared key used in authentication</param>
    /// <param name="Challenge">Generated challenge</param>
    /// <param name="Response">Generated response</param>
    public static void GenerateChallengeResponse<T>(string SharedKey, ref byte[] Challenge, ref byte[] Response) where T : HashAlgorithm, new()
    {
        Challenge = new byte[16];
        Random.NextBytes(Challenge);
        var Bytes = new List<byte>(Challenge);
        Bytes.AddRange(Encoding.Unicode.GetBytes(SharedKey));
        Response = HashProviderCache<T>.Instance.ComputeHash([.. Bytes]);
    }

    /// <summary>
    /// Generates a response pair for a given shared key and challenge.
    /// </summary>
    /// <param name="SharedKey">Shared key used in authentication</param>
    /// <param name="Challenge">Challenge phrase</param>
    public static byte[] GetResponseFromChallenge<T>(string SharedKey, byte[] Challenge) where T : HashAlgorithm, new()
    {
        var Bytes = new List<byte>(Challenge);
        Bytes.AddRange(Encoding.Unicode.GetBytes(SharedKey));
        return HashProviderCache<T>.Instance.ComputeHash([.. Bytes]);
    }

    /// <summary>
    /// Computes a hash for the bytes describing a Unicode string and returns the first 64
    /// bits converted to a decimal string.
    /// </summary>
    /// <param name="str">Any Unicode string or Null</param>
    public static string Get64BitHash<T>(string str) where T : HashAlgorithm, new() => BitConverter.ToUInt64(GetHash<T>(str), 0).ToString();

    /// <summary>
    /// Computes a hash for the bytes describing a Unicode string.
    /// </summary>
    /// <param name="str">Any Unicode string or Null</param>
    public static byte[] GetHash<T>(string str) where T : HashAlgorithm, new() => GetHash<T>(str, Encoding.Unicode);

    /// <summary>
    /// Computes a hash for the bytes describing a string.
    /// </summary>
    /// <param name="str">Any Unicode string or Null</param>
    /// <param name="Encoding"></param>
    public static byte[] GetHash<T>(string str, Encoding Encoding) where T : HashAlgorithm, new()
    {
        var bytes = str is null ? [] : Encoding.GetBytes(str);
        return HashProviderCache<T>.Instance.ComputeHash(bytes);
    }

    /// <summary>
    /// Get an array of bytes with random numbers.
    /// </summary>
    /// <param name="count">Number of bytes to return</param>
    public static byte[] GetRandomBytes(int count)
    {
        var bytes = new byte[count];
        Random.NextBytes(bytes);
        return bytes;
    }

    /// <summary>
    /// Returns a non-negative random number.
    /// </summary>
    public static int GetRandomNumber() => Random.Next();

    /// <summary>
    /// Returns a non-negative random number less than maxValue.
    /// </summary>
    /// <param name="maxValue">Exclusive upper bound of random number</param>
    public static int GetRandomNumber(int maxValue) => Random.Next(maxValue);

    /// <summary>
    /// Returns a non-negative random number with specified range.
    /// </summary>
    /// <param name="minValue">Inclusive lower bound of random number</param>
    /// <param name="maxValue">Exclusive upper bound of random number</param>
    public static int GetRandomNumber(int minValue, int maxValue) => Random.Next(minValue, maxValue);

    /// <summary>
    /// Returns random number between 0.0 and 1.0.
    /// </summary>
    public static double GetRandomDouble() => Random.NextDouble();

    /// <summary>
    /// Returns a Guid value filled with random data.
    /// </summary>
    public static Guid GetRandomId() => new(GetRandomBytes(16));

#if NET6_0_OR_GREATER

    public static Guid GetHashAsGuid(string str)
    {
        Span<Guid> guid = stackalloc Guid[1];

        if (!MD5.TryHashData(MemoryMarshal.AsBytes(str.AsSpan()), MemoryMarshal.AsBytes(guid), out var written) || written != 16)
        {
            throw new InvalidOperationException("Failed to generate MD5 hash for string");
        }

        return guid[0];
    }

#elif NETCOREAPP || NETSTANDARD2_1_OR_GREATER

    [ThreadStatic]
    private static MD5? cachedMD5;

    /// <summary>
    /// Returns MD5 hash of string as Guid
    /// </summary>
    /// <param name="str">String from which to calculate hash value</param>
    public static Guid GetHashAsGuid(string str)
    {
        cachedMD5 ??= MD5.Create();

        Span<Guid> guid = stackalloc Guid[1];

        if (!cachedMD5.TryComputeHash(MemoryMarshal.AsBytes(str.AsSpan()), MemoryMarshal.AsBytes(guid), out var written) || written != 16)
        {
            throw new InvalidOperationException("Failed to generate MD5 hash for string");
        }

        return guid[0];
    }

#else
    /// <summary>
    /// Returns MD5 hash of string as Guid
    /// </summary>
    /// <param name="str">String from which to calculate hash value</param>
    public static Guid GetHashAsGuid(string str) => new(GetHash<MD5CryptoServiceProvider>(str));
#endif

    public static MemoryStream DecompressStream(byte[] buffer, bool throwOnDecoderError) => DecompressStream(new BinaryReader(new MemoryStream(buffer)), throwOnDecoderError);

    public static MemoryStream DecompressStream(byte[] buffer, int index, int count, bool throwOnDecoderError) => DecompressStream(new BinaryReader(new MemoryStream(buffer, index, count)), throwOnDecoderError);

    public static MemoryStream DecompressStream(BinaryReader reader, bool throwOnDecoderError)
    {
        var uncompressed = new MemoryStream();

        var BufferedFlags = 0U;
        var BufferedFlagCount = 0;
        var LastLengthHalfByte = default(byte?);

        try
        {
            for(; ;)
            {
                if (BufferedFlagCount == 0)
                {
                    if (reader.BaseStream.CanSeek && reader.BaseStream.Position > reader.BaseStream.Length - 4L)
                    {
                        break;
                    }

                    BufferedFlags = reader.ReadUInt32();

                    BufferedFlagCount = 32;
                }

                BufferedFlagCount -= 1;

                if ((BufferedFlags & 1U << BufferedFlagCount) == 0U)
                {
                    if (reader.BaseStream.CanSeek && reader.BaseStream.Position > reader.BaseStream.Length - 1L)
                    {
                        break;
                    }

                    uncompressed.WriteByte(reader.ReadByte());
                }
                else
                {
                    if (reader.BaseStream.CanSeek && reader.BaseStream.Position > reader.BaseStream.Length - 2L)
                    {
                        break;
                    }

                    var MatchBytes = reader.ReadUInt16();

                    var MatchLength = (ushort)(MatchBytes & 0x7);
                    var MatchOffset = (ushort)((MatchBytes >> 3) + 1);

                    if (MatchLength == 7)
                    {
                        if (!LastLengthHalfByte.HasValue)
                        {
                            LastLengthHalfByte = reader.ReadByte();

                            MatchLength = (ushort)(LastLengthHalfByte.Value & 0xF);
                        }
                        else
                        {
                            MatchLength = (ushort)(LastLengthHalfByte.Value >> 4);

                            LastLengthHalfByte = default;
                        }

                        if (MatchLength == 15)
                        {
                            MatchLength = reader.ReadByte();

                            if (MatchLength == 255)
                            {
                                MatchLength = reader.ReadUInt16();

                                if (MatchLength < 15 + 7)
                                {
                                    if (throwOnDecoderError)
                                    {
                                        throw new InvalidDataException();
                                    }
                                    else
                                    {
                                        return uncompressed;
                                    }
                                }

                                MatchLength = (ushort)(MatchLength - (15 + 7));
                            }

                            MatchLength = (ushort)(MatchLength + 15);
                        }

                        MatchLength = (ushort)(MatchLength + 7);
                    }

                    MatchLength = (ushort)(MatchLength + 3);

                    for (ushort i = 0, loopTo = (ushort)(MatchLength - 1); i <= loopTo; i++)
                    {
                        if (MatchOffset > uncompressed.Position)
                        {
                            if (throwOnDecoderError)
                            {
                                throw new InvalidDataException();
                            }
                            else
                            {
                                return uncompressed;
                            }

                        }

                        uncompressed.WriteByte(uncompressed.GetBuffer()[(int)(uncompressed.Position - MatchOffset)]);
                    }
                }
            }
        }
        catch (EndOfStreamException)
        {
        }

        return uncompressed;

    }

}