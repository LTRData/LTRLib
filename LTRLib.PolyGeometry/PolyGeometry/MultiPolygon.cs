/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using LTRData.Extensions.Buffers;
using LTRData.Extensions.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression

namespace LTRLib.PolyGeometry;

[Guid("D28C7681-98AF-41A6-829C-1738BDAC09A5")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IPolyGeometry))]
[SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
public sealed class MultiPolygon : ReadOnlyCollection<Polygon>, IPolyGeometry
{
    public MultiPolygon(IList<Polygon> polygons)
        : base(polygons)
    {
    }

    public MultiPolygon(params Polygon[] polygons)
        : base(polygons)
    {
    }

    public static MultiPolygon Parse(string text) => new(Array.ConvertAll(text.Split(")), ((",
            StringSplitOptions.RemoveEmptyEntries), Polygon.Parse));

    public override string ToString() => $"((({this.Select(polygon => polygon.ToString()).Join(")), ((")})))";

    public bool Contains(Point point) => this.Any(polygon => polygon.Contains(point));

    public double MinX => this.Min(polygon => polygon.MinX);

    public double MinY => this.Min(polygon => polygon.MinY);

    public double MaxX => this.Max(polygon => polygon.MaxX);

    public double MaxY => this.Max(polygon => polygon.MaxY);

    public double Length => this.Sum(polygon => polygon.Length);

    public double Area => this.Sum(polygon => polygon.Area);

    public IEnumerable<Line> Bounds => this.SelectMany(polygon => polygon);

    public IEnumerable<Point> Corners => this.SelectMany(polygon => polygon.Corners);
}
