/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using NetTopologySuite.Geometries;
using System;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Placemarks;

[Guid("2D7C2BF3-5E48-485F-9A73-D656C44409FB")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class Region
{
    public string? name { get; set; }

    public string? id { get; set; }

    public Geometry polygons { get; set; } = null!;

    public override string? ToString()
    {
        if ((id is null) || (name is null))
        {
            return base.ToString();
        }

        return string.Concat(id, " ", name);
    }
}


#endif
