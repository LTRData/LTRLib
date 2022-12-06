#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LTRLib.Extensions;

public static class DrawingExtensions
{
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
}

#endif
