#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Formatting;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LTRLib.Extensions;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

public static class DrawingExtensions
{
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Generates keyhole markup language adjusted color string.
    /// </summary>
    /// <param name="lineColor">Input color value to convert</param>
    public static string ToAdjustedKmlColor(this Color lineColor)
    {
        var lineColorValue = lineColor.ToArgb();
        var lineColorBytes = MemoryMarshal.AsBytes((stackalloc int[] { lineColorValue }));

        if (BitConverter.IsLittleEndian)
        {
            lineColorBytes.Reverse();
        }

        lineColorBytes.Slice(1).Reverse();

        if ((lineColorValue & 0x808080) == 0) // Very dark colors
        {
            lineColorBytes[3] += 64;
            lineColorBytes[2] += 64;
            lineColorBytes[1] += 128;
        }
        else if ((lineColorValue & 0xC0C0C0) == 0xC0C0C0) // Very bright colors
        {
            lineColorBytes[3] -= 64;
            lineColorBytes[2] -= 64;
        }

        var kmlColor = lineColorBytes.ToHexString();

        return kmlColor;
    }
#else
    /// <summary>
    /// Generates keyhole markup language adjusted color string.
    /// </summary>
    /// <param name="lineColor">Input color value to convert</param>
    public static string ToAdjustedKmlColor(this Color lineColor)
    {
        var lineColorBytes = BitConverter.GetBytes(lineColor.ToArgb());

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lineColorBytes);
        }

        Array.Reverse(lineColorBytes, 1, 3);

        if (lineColorBytes.Skip(1).All(lineColorByte => lineColorByte < 128))
        {
            lineColorBytes[3] += 64;
            lineColorBytes[2] += 64;
            lineColorBytes[1] += 128;
        }
        else if (lineColorBytes.Skip(1).All(lineColorByte => lineColorByte > 192))
        {
            lineColorBytes[3] -= 64;
            lineColorBytes[2] -= 64;
        }

        var kmlColor = string.Concat(from lineColorByte in lineColorBytes
                                     select lineColorByte.ToString("x2"));
        return kmlColor;
    }
#endif
}

#endif
