/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.PolyGeometry;

[Guid("AD487C27-826F-49B4-A88F-FDE5D2E632FD")]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IPolyGeometry
{
    bool Contains(Point point);

    double Length { get; }

    [ComVisible(false)]
    IEnumerable<Line> Bounds { get; }

    [ComVisible(false)]
    IEnumerable<Point> Corners { get; }

    double Area { get; }

    double MinX { get; }

    double MinY { get; }

    double MaxX { get; }

    double MaxY { get; }
}
