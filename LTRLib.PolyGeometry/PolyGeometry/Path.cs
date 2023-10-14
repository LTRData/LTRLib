/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using LTRLib.Extensions;
using LTRData.Extensions.Buffers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using LTRData.Extensions.Formatting;

#pragma warning disable IDE0079 // Remove unnecessary suppression

namespace LTRLib.PolyGeometry;

[Guid("F5802B40-E611-4972-89FE-0916C4D76AD1")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IPolyGeometry))]
[SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
public sealed class Path : ReadOnlyCollection<Line>, IPolyGeometry
{
    public Path(IList<Point> points)
        : base(points.GetLines())
    {
    }

    public Path(params Point[] points)
        : this(points as IList<Point>)
    {
    }

    public static Path Parse(string text) => new(Array.ConvertAll(text.Split(','), Point.Parse));

    public bool Contains(Point point) => this.MBRContains(point) && this.Any(line => line.Contains(point));

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public IEnumerable<Point> Corners => this.Select(line => line.X1Y1).Prepend(this[0].X0Y0);
#else
    public IEnumerable<Point> Corners => SingleValueEnumerable.Get(this[0].X0Y0).Concat(this.Select(line => line.X1Y1));
#endif

    public IEnumerable<Line> Bounds => this;

    double IPolyGeometry.Area => 0;

    public double MinX => this.Min(line => line.MinX);

    public double MinY => this.Min(line => line.MinY);

    public double MaxX => this.Max(line => line.MaxX);

    public double MaxY => this.Max(line => line.MaxY);

    public double Length => this.Sum(line => line.Length);

    public override string ToString() => Corners.Select(point => point.ToString()).Join(", ");
}
