#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRData.Extensions.Buffers;
using System;
using System.Drawing;
using System.Linq;
using Vector3D = System.Numerics.Vector3;

namespace LTRLib.Extensions;

public static class Math3DExtensions
{
    public static double DistanceToRGB(this Color Color1, Color Color2) => new Vector3D(Color1.R, Color1.G, Color1.B).DistanceTo(new Vector3D(Color2.R, Color2.G, Color2.B));

    /// <summary>
    /// Calculates the area covered by a size.
    /// </summary>
    public static double Volume(this Vector3D Size) => (double)(Size.X * Size.Y * Size.Z);

    public static double Radius(this Vector3D Point) => Math.Sqrt(Math.Pow(Math.Sqrt((double)(Point.X * Point.X + Point.Y * Point.Y)), 2d) + (double)(Point.Z * Point.Z));

    public static double DistanceTo(this Vector3D Point1, Vector3D Point2) => (Point1 - new Vector3D(Point2.X, Point2.Y, Point2.Z)).Radius();

    public static double DistanceToHSB(this Color Color1, Color Color2) => new Vector3D(Color1.GetHue(), Color1.GetSaturation(), Color1.GetBrightness()).DistanceTo(new Vector3D(Color2.GetHue(), Color2.GetSaturation(), Color2.GetBrightness()));

    public static Color GetNearestColorRGB(this Color Color, Color[] Colors)
        => Colors.MinBy(c => c.DistanceToRGB(Color));
    
    private static Color GetClosestConsoleCompatibleColor(Color Color) => Color.FromArgb(Color.A, GetClosestConsoleCompatibleColorComponent(Color.R), GetClosestConsoleCompatibleColorComponent(Color.G), GetClosestConsoleCompatibleColorComponent(Color.B));

    private static byte GetClosestConsoleCompatibleColorComponent(byte ColorComponent)
        => ColorComponent switch
        {
            < 64 => 0,
            > 192 => 255,
            < 128 => 64,
            _ => 192
        };

    public static Color GetNearestColorHSB(this Color Color, Color[] Colors)
    {
        return Colors.MinBy(c => c.DistanceToHSB(Color));
    }

#if NET461_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static Color GetNearestKnownColorRGB(this Color Color) => Color.GetNearestColorRGB(KnownColors);

    public static Color GetNearestKnownColorHSB(this Color Color) => Color.GetNearestColorHSB(KnownColors);

    public static ConsoleColor GetNearestConsoleColorRGB(this Color Color) => LTRGeneric.EnumTools.ParseEnumName<ConsoleColor>(GetClosestConsoleCompatibleColor(Color).GetNearestColorRGB(ConsoleColors).Name);

    public static ConsoleColor GetNearestConsoleColorHSB(this Color Color) => LTRGeneric.EnumTools.ParseEnumName<ConsoleColor>(Color.GetNearestColorHSB(ConsoleColors).Name);

    private static readonly Color[] ConsoleColors = Array.ConvertAll(Enum.GetNames(typeof(ConsoleColor)), Color.FromName);

    private static readonly Color[] KnownColors = Array.ConvertAll((KnownColor[])Enum.GetValues(typeof(KnownColor)), Color.FromKnownColor);
#endif

}

#endif
