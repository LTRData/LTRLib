﻿/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using LTRData.Extensions.Formatting;
using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTRLib.PolyGeometry;

public struct Rectangle : IPolyGeometry
{
    public Rectangle(Point x0y0, Point x1y1)
        : this()
    {
        X0Y0 = x0y0;
        X1Y1 = x1y1;
    }

    public Rectangle(Line diagonale)
        : this(diagonale.X0Y0, diagonale.X1Y1)
    {
    }

    public Point X0Y0 { get; set; }

    public Point X1Y1 { get; set; }

    public readonly Point X1Y0 => new(X1Y1.X, X0Y0.Y);

    public readonly Point X0Y1 => new(X0Y0.X, X1Y1.Y);

    public static implicit operator Polygon(Rectangle rect) => rect.AsPolygon();

    public readonly Polygon AsPolygon() => new([X0Y0, X1Y0, X1Y1, X0Y1, X0Y0]);

    public readonly IEnumerable<Point> Corners
    {
        get
        {
            yield return X0Y0;
            yield return X1Y0;
            yield return X1Y1;
            yield return X0Y1;
        }
    }

    public readonly IEnumerable<Line> Bounds => AsPolygon();

    public override readonly string ToString() => Corners.Select(point => point.ToString()).Join(", ");

    public readonly double MinX => Math.Min(X0Y0.X, X1Y1.X);

    public readonly double MinY => Math.Min(X0Y0.Y, X1Y1.Y);

    public readonly double MaxX => Math.Max(X0Y0.X, X1Y1.X);

    public readonly double MaxY => Math.Max(X0Y0.Y, X1Y1.Y);

    public readonly bool Contains(Point point) => this.MBRContains(point);

    public readonly double Length => 2 * (MaxY - MinY + MaxX - MinX);

    public readonly double Area => (MaxY - MinY) * (MaxX - MinX);

    public static bool operator ==(Rectangle rect1, Rectangle rect2) => rect1.Equals(rect2);

    public static bool operator !=(Rectangle line1, Rectangle line2) => !line1.Equals(line2);

    public override readonly int GetHashCode() => X0Y0.GetHashCode() ^ X1Y1.GetHashCode();

    public override readonly bool Equals(object? obj) => obj is Rectangle rectangle && Equals(rectangle);

    public readonly bool Equals(Rectangle other) => X0Y0.Equals(other.X0Y0) && X1Y1.Equals(other.X1Y1);
}
