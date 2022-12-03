/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace LTRLib.PolyGeometry;

[Guid("0BB752BB-E50B-49A2-B36E-A77AA20FA290")]
[StructLayout(LayoutKind.Sequential)]
public struct Point : IEquatable<Point>, IPolyGeometry
{
    public Point(double x, double y)
        : this()
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public bool MBRWithin(IPolyGeometry geometry) => geometry.MBRContains(this);

    public bool Within(IPolyGeometry geometry) => geometry.Contains(this);

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

    public override bool Equals(object obj) => obj is Point point && Equals(point);

    public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);

    public override string ToString() => $"{X.ToString(NumberFormatInfo.InvariantInfo)} {Y.ToString(NumberFormatInfo.InvariantInfo)}";

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static Point Parse(ReadOnlyMemory<char> text)
    {
        var xy = 
            text.TrimStart('(').TrimEnd(')').Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(
            v => double.Parse(v.Span.Trim(), provider: NumberFormatInfo.InvariantInfo)).ToArray();

        return new Point(xy[0], xy[1]);
    }

    public static Point Parse(string text) => Parse(text.AsMemory());
#elif NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER
    public static Point Parse(ReadOnlyMemory<char> text)
    {
        var xy = 
            text.TrimStart('(').TrimEnd(')').Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(
            v => double.Parse(v.Span.Trim().ToString(), NumberFormatInfo.InvariantInfo)).ToArray();

        return new Point(xy[0], xy[1]);
    }

    public static Point Parse(string text) => Parse(text.AsMemory());
#else
    public static Point Parse(string text)
    {
        var xy = Array.ConvertAll(
            text.Trim('(', ')').Split(' ', StringSplitOptions.RemoveEmptyEntries),
            v => double.Parse(v.Trim(), NumberFormatInfo.InvariantInfo));

        return new Point(xy[0], xy[1]);
    }
#endif

    bool IPolyGeometry.Contains(Point point) => Equals(point);

    public static bool operator ==(Point point1, Point point2) => point1.Equals(point2);

    public static bool operator !=(Point point1, Point point2) => !point1.Equals(point2);

    double IPolyGeometry.MinX => X;

    double IPolyGeometry.MinY => Y;

    double IPolyGeometry.MaxX => X;

    double IPolyGeometry.MaxY => Y;

    double IPolyGeometry.Area => 0;

    double IPolyGeometry.Length => 0;

    IEnumerable<Point> IPolyGeometry.Corners
    {
        get
        {
            yield return this;
        }
    }

    IEnumerable<Line> IPolyGeometry.Bounds
    {
        get
        {
            yield return new Line(this, this);
        }
    }
}
