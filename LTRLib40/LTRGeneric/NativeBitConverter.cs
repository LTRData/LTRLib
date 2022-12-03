﻿/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;

namespace LTRLib.LTRGeneric;

public static class NativeBitConverter
{
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
}
