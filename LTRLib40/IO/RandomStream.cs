/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace LTRLib.IO;

public class RandomStream : ZeroStream
{
    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool SystemFunction036(out byte ptr, int length);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP

    public override void FillBuffer(byte[] buffer, int offset, int count)
        => FillBuffer(buffer.AsSpan(offset, count));

    public override void FillBuffer(Span<byte> buffer) =>
        RandomNumberGenerator.Fill(buffer);

#elif NET45_OR_GREATER || NETSTANDARD

    public override void FillBuffer(byte[] buffer, int offset, int count)
        => FillBuffer(buffer.AsSpan(offset, count));

    public void FillBuffer(Span<byte> buffer)
    {
        if (!SystemFunction036(out buffer[0], buffer.Length))
        {
            throw new Win32Exception();
        }
    }

#else

    public override void FillBuffer(byte[] buffer, int offset, int count)
    {
        if (count < 0 || offset < 0 || count + offset > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (!SystemFunction036(out buffer[offset], buffer.Length - count))
        {
            throw new Win32Exception();
        }
    }

#endif
}
