// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP

using LTRData.Extensions.Split;
using LTRGeneric;
using LTRLib.Extensions;
using LTRLib.LTRGeneric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace LTRLib.Net;

#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.

/// <summary>
/// Collection of ranges of IPv4 addresses
/// </summary>
[ComDefaultInterface(typeof(IList))]
public class IPAddressRanges : NumericRanges<uint>
{
    public AddressFamily AddressFamily { get; }

    public IPAddressRanges(AddressFamily addressFamily)
        : base()
    {
        AddressFamily = addressFamily;
    }

    public IPAddressRanges(AddressFamily addressFamily, int capacity) : base(capacity)
    {
        AddressFamily = addressFamily;
    }

    public void Add(string start, string end) => Add(IPAddress.Parse(start), IPAddress.Parse(end));

    public void Add(string range)
    {
        var start = IPAddressToNumeric(range.AsSpan().TokenEnum('-').ElementAt(0));
        var end = IPAddressToNumeric(range.AsSpan().TokenEnum('-').ElementAt(1));
        Add((Math.Min(start, end), Math.Max(start, end)));
    }

    public void Add(IPAddress start, IPAddress end) => Add(IPAddressToNumeric(start), IPAddressToNumeric(end));

    public void AddRange(IEnumerable<string> ranges)
    {
        AddRange(from range in ranges
                 let start = IPAddressToNumeric(range.AsSpan().TokenEnum('-').ElementAt(0))
                 let end = IPAddressToNumeric(range.AsSpan().TokenEnum('-').ElementAt(1))
                 select (Math.Min(start, end), Math.Max(start, end)));
    }

    public uint IPAddressToNumeric(ReadOnlySpan<char> address) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        IPAddressToNumeric(IPAddress.Parse(address.Trim()));
#else
        IPAddressToNumeric(IPAddress.Parse(address.Trim().ToString()));
#endif


    public uint IPAddressToNumeric(IPAddress address)
    {
        if (address.AddressFamily != AddressFamily)
        {
            throw new ArgumentOutOfRangeException(nameof(address), "Unsupported address family");
        }

        return IPAddressToNumeric(address.GetAddressBytes());
    }

    public static uint IPAddressToNumeric(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            bytes = bytes.CreateTypedClone();
            Array.Reverse(bytes);
        }

        return bytes.Length switch
        {
            1 => bytes[0],
            2 => BitConverter.ToUInt16(bytes, 0),
            4 => BitConverter.ToUInt32(bytes, 0),
            _ => throw new ArgumentException($"Unsupported address size: {bytes.Length}", nameof(bytes)),
        };
    }

    public static IPAddress IPAddressFromNumeric(uint numeric)
    {
        Span<byte> bytes = stackalloc byte[sizeof(uint)];

        MemoryMarshal.Write(bytes, ref numeric);

        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        return new IPAddress(bytes);
#else
        return new IPAddress(bytes.ToArray());
#endif
    }

    public static IPAddress IPAddressFromNumeric(ReadOnlySpan<byte> bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            Span<byte> buffer = stackalloc byte[bytes.Length];
            bytes.CopyTo(buffer);
            buffer.Reverse();

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            return new IPAddress(buffer);
#else
            return new IPAddress(buffer.ToArray());
#endif
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        return new IPAddress(bytes);
#else
        return new IPAddress(bytes.ToArray());
#endif
    }

    public bool Encompasses(IPAddress address)
    {
        if (address.AddressFamily != AddressFamily)
        {
            return false;
        }

        return Encompasses(IPAddressToNumeric(address));
    }

    public bool Encompasses(string address) => Encompasses(IPAddress.Parse(address));

    /// <exclude />
    public (IPAddress Network, IPAddress Mask, IPAddress Broadcast, byte BitCount) CalculateNetwork(IPAddress start, IPAddress end)
    {
        var (Network, Mask, Broadcast, BitCount) = CalculateNetwork(IPAddressToNumeric(start), IPAddressToNumeric(end));
        return (IPAddressFromNumeric(Network), IPAddressFromNumeric(Mask), IPAddressFromNumeric(Broadcast), BitCount);
    }

    /// <exclude />
    public (IPAddress Network, IPAddress Mask, IPAddress Broadcast, byte BitCount) CalculateNetwork(IPAddress start, byte bitCount)
    {
        var (Network, Mask, Broadcast, BitCount) = CalculateNetwork(IPAddressToNumeric(start), bitCount);
        return (IPAddressFromNumeric(Network), IPAddressFromNumeric(Mask), IPAddressFromNumeric(Broadcast), BitCount);
    }

    public static (uint Network, uint Mask, uint Broadcast, byte BitCount) CalculateNetwork(uint start, uint end)
    {
        byte BitCount = 32;
        var maskShift = end ^ start;

        while ((maskShift & 1) != 0)
        {
            maskShift >>= 1;
            BitCount--;
        }

        var Mask = ~((1u << (32 - BitCount)) - 1);

        var Network = start & Mask;
        var Broadcast = Network | ~Mask;

        if (Network != start || Broadcast != end)
        {
            throw new ArgumentException($"Invalid IPv4 range, expected {IPAddressFromNumeric(Network)}/{BitCount}-{IPAddressFromNumeric(Broadcast)}");
        }

        return (Network, Mask, Broadcast, BitCount);
    }

    public static (uint Network, uint Mask, uint Broadcast, byte BitCount) CalculateNetwork(uint start, byte bitCount)
    {
        var Mask = ~((1u << (32 - bitCount)) - 1);

        var Network = start & Mask;
        var Broadcast = Network | ~Mask;

        if (Network != start)
        {
            throw new ArgumentException($"Invalid IPv4 range, expected {IPAddressFromNumeric(Network)}/{bitCount}-{IPAddressFromNumeric(Broadcast)}");
        }

        return (Network, Mask, Broadcast, bitCount);
    }
}

#endif
