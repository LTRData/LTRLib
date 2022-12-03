/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
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

    public Point X1Y0 => new(X1Y1.X, X0Y0.Y);

    public Point X0Y1 => new(X0Y0.X, X1Y1.Y);

    public static implicit operator Polygon(Rectangle rect) => rect.AsPolygon();

    public Polygon AsPolygon() => new(new[] { X0Y0, X1Y0, X1Y1, X0Y1, X0Y0 });

    public IEnumerable<Point> Corners
    {
        get
        {
            yield return X0Y0;
            yield return X1Y0;
            yield return X1Y1;
            yield return X0Y1;
        }
    }

    public IEnumerable<Line> Bounds => AsPolygon();

    public override string ToString() => Corners.Select(point => point.ToString()).Join(", ");

    public double MinX => Math.Min(X0Y0.X, X1Y1.X);

    public double MinY => Math.Min(X0Y0.Y, X1Y1.Y);

    public double MaxX => Math.Max(X0Y0.X, X1Y1.X);

    public double MaxY => Math.Max(X0Y0.Y, X1Y1.Y);

    public bool Contains(Point point) => this.MBRContains(point);

    public double Length => 2 * (MaxY - MinY + MaxX - MinX);

    public double Area => (MaxY - MinY) * (MaxX - MinX);

    public static bool operator ==(Rectangle rect1, Rectangle rect2) => rect1.Equals(rect2);

    public static bool operator !=(Rectangle line1, Rectangle line2) => !line1.Equals(line2);

    public override int GetHashCode() => X0Y0.GetHashCode() ^ X1Y1.GetHashCode();

    public override bool Equals(object obj) => obj is Rectangle rectangle && Equals(rectangle);

    public bool Equals(Rectangle other) => X0Y0.Equals(other.X0Y0) && X1Y1.Equals(other.X1Y1);
}
