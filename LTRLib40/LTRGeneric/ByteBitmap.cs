/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

namespace LTRLib.LTRGeneric;

/// <summary>
/// Routines for manipulating individual bits in bitmaps stored as byte array.
/// </summary>
public static class ByteBitmap
{
    /// <summary>
    /// Sets a bit to 1 in a bit field.
    /// </summary>
    /// <param name="data">Bit field</param>
    /// <param name="bitnumber">Bit number to set to 1</param>
    public static void SetBit(this byte[] data, int bitnumber) =>
        data[bitnumber >> 3] |= (byte)(1 << ((~bitnumber) & 7));

    /// <summary>
    /// Sets a bit to 0 in a bit field.
    /// </summary>
    /// <param name="data">Bit field</param>
    /// <param name="bitnumber">Bit number to set to 0</param>
    public static void ClearBit(this byte[] data, int bitnumber) =>
        data[bitnumber >> 3] &= unchecked((byte)~(1 << ((~bitnumber) & 7)));

    /// <summary>
    /// Gets a bit from a bit field.
    /// </summary>
    /// <param name="data">Bit field</param>
    /// <param name="bitnumber">Bit number to get</param>
    /// <returns>True if value of specified bit is 1, false if 0.</returns>
    public static bool GetBit(this byte[] data, int bitnumber) =>
        (data[bitnumber >> 3] & (1 << ((~bitnumber) & 7))) != 0;
}
