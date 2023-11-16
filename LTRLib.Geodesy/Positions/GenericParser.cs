using LTRData.Extensions.Buffers;
using System;

namespace LTRLib.Geodesy.Positions;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

public static class GenericParser
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static Position Parse(ReadOnlySpan<char> value)
    {
        if (value.Length >= 7 &&
            value.IndexOf(',') >= 0 &&
            value.TrimStart().Slice(0, 2).Equals("x=", StringComparison.OrdinalIgnoreCase) &&
            value.Slice(value.IndexOf(',') + 1).TrimStart().Slice(0, 2).Equals("y=", StringComparison.OrdinalIgnoreCase))
        {
            var coordstr = value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (coordstr.Length != 2)
            {
                throw new FormatException("Invalid RT90 coordinate string.");
            }

            var x = coordstr[0].AsSpan().Trim().Slice(2);
            var y = coordstr[1].AsSpan().Trim().Slice(2);

            return new RT90Position(x, y);
        }
        else if (value.StartsWith("SWEREF99:", StringComparison.OrdinalIgnoreCase))
        {
            return new SWEREF99Position(value.Slice("SWEREF99:".Length));
        }
        else
        {
            return new WGS84Position(value);
        }

    }
#endif

    public static Position Parse(string value)
    {
        if (value.Length >= 7 &&
            value.Contains(',') &&
            value.TrimStart().Substring(0, 2).Equals("x=", StringComparison.OrdinalIgnoreCase) &&
            value.Substring(value.IndexOf(',') + 1).TrimStart().Substring(0, 2).Equals("y=", StringComparison.OrdinalIgnoreCase))
        {
            var coordstr = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (coordstr.Length != 2)
            {
                throw new FormatException("Invalid RT90 coordinate string.");
            }

            var x = coordstr[0].Trim().Substring(2);
            var y = coordstr[1].Trim().Substring(2);

            return new RT90Position(x, y);
        }
        else if (value.StartsWith("SWEREF99:", StringComparison.OrdinalIgnoreCase))
        {
            return new SWEREF99Position(value.Substring("SWEREF99:".Length));
        }
        else
        {
            return new WGS84Position(value);
        }

    }

    public static TPosition ConvertTo<TPosition>(this Position fromPos)
        where TPosition : Position, new()
    {
        if (fromPos is TPosition position)
        {
            return position;
        }

        var toPos = new TPosition();

        toPos.FromWGS84(fromPos.ToWGS84());

        return toPos;
    }
}
