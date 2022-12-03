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
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace LTRLib.PolyGeometry;

[Guid("F0DFEA68-ECA8-4C21-A5C4-748F5C14BAC1")]
[StructLayout(LayoutKind.Sequential)]
public struct Line : IPolyGeometry, IEquatable<Line>
{
    public Line(Point x0y0, Point x1y1)
        : this()
    {
        x0y0_ = x0y0;
        X1Y1 = x1y1;
    }

    private Point x0y0_;
    private Point x1y1_;

    public Point X0Y0
    {
        get => x0y0_;
        set
        {
            x0y0_ = value;
            Gradient = (x1y1_.Y - x0y0_.Y) / (x1y1_.X - x0y0_.X);
            Base = x0y0_.Y - (Gradient * x0y0_.X);
        }
    }

    public Point X1Y1
    {
        get => x1y1_;
        set
        {
            x1y1_ = value;
            Gradient = (x1y1_.Y - x0y0_.Y) / (x1y1_.X - x0y0_.X);
            Base = x0y0_.Y - (Gradient * x0y0_.X);
        }
    }

    [XmlIgnore]
    public double Gradient { get; private set; }

    [XmlIgnore]
    public double Base { get; private set; }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static Line Parse(string text)
    {
        var points = text.AsMemory().Split(',').Take(2).Select(Point.Parse).ToArray();
        return new Line(points[0], points[1]);
    }
#else
    public static Line Parse(string text)
    {
        var points = Array.ConvertAll(text.Split(','), Point.Parse);
        return new Line(points[0], points[1]);
    }
#endif

    public override string ToString() => $"{X0Y0}, {X1Y1}";

    public double MinX => Math.Min(X0Y0.X, X1Y1.X);

    public double MinY => Math.Min(X0Y0.Y, X1Y1.Y);

    public double MaxX => Math.Max(X0Y0.X, X1Y1.X);

    public double MaxY => Math.Max(X0Y0.Y, X1Y1.Y);

    public double GetXForY(double y) => (y - Base) / Gradient;

    public double GetYForX(double x) => Gradient * x + Base;

    public bool Contains(Point point)
    {
        if (!this.MBRContains(point))
        {
            return false;
        }

        var dxc = point.X - X0Y0.X;
        var dyc = point.Y - X0Y0.Y;

        var dxl = X1Y1.X - X0Y0.X;
        var dyl = X1Y1.Y - X0Y0.Y;

        var cross = dxc * dyl - dyc * dxl;

        // Your point lies on the line if and only if cross is equal to zero.
        if (cross == 0)
        {
            return true;
        }

        return false;
    }

    public double Length => Math.Sqrt(Math.Pow(X1Y1.Y - X0Y0.Y, 2) + Math.Pow(X1Y1.X - X0Y0.X, 2));

    public static bool operator ==(Line line1, Line line2) => line1.Equals(line2);

    public static bool operator !=(Line line1, Line line2) => !line1.Equals(line2);

    public override int GetHashCode() => X0Y0.GetHashCode() ^ X1Y1.GetHashCode();

    public override bool Equals(object obj) => obj is Line line && Equals(line);

    public bool Equals(Line other) => X0Y0.Equals(other.X0Y0) && X1Y1.Equals(other.X1Y1);

    double IPolyGeometry.Area => 0;

    public static implicit operator Path(Line line) => new(line.X0Y0, line.X1Y1);

    IEnumerable<Point> IPolyGeometry.Corners
    {
        get
        {
            yield return X0Y0;
            yield return X1Y1;
        }
    }

    IEnumerable<Line> IPolyGeometry.Bounds
    {
        get
        {
            yield return this;
        }
    }
}
