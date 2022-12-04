/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

public static class NativeBitConverter
{
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static Guid ToGuid(byte[] bytes, int offset)
        => MemoryMarshal.Read<Guid>(bytes.AsSpan(offset, 16));
#else
    public static unsafe Guid ToGuid(byte[] bytes, int offset)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        if (offset < 0 || offset >= bytes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset must point to a position within the array");
        }

        if (bytes.Length - offset < 16)
        {
            throw new ArgumentException("Too few bytes for a Guid value", nameof(bytes));
        }

        fixed (byte* ptr = &bytes[offset])
        {
            return *(Guid*)ptr;
        }
    }
#endif
}
