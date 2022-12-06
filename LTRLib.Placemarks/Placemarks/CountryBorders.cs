/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.IO;
using LTRLib.LTRGeneric;
using LTRLib.Services.kml.simplified;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using DbPolygon = NetTopologySuite.Geometries.Polygon;
using DbGeometry = NetTopologySuite.Geometries.Geometry;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Placemarks;

[Guid("5B35346F-CB4B-4DCE-ACDC-2FB771254A97")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class CountryBorders
{
    public CountryBorders(string csvPath)
    {
        _Countries = Task.Run(() => CsvToRegion(csvPath).ToArray());
    }

    protected static IEnumerable<Region> CsvToRegion(string csvPath) =>
        new CsvReader<WorldCountryCsv>(csvPath).Select(item => new Region
        {
            name = item.Name,
            id = item.Name,
            polygons = PlacemarkSupport.GetKMLReader().Read(item.geometry)
        });

    protected static Coordinates Deserialize(XDocument xdoc) => ((xdoc.Nodes().FirstOrDefault() as XElement)?.Name.LocalName switch
    {
        "MultiGeometry" => XmlSupport.XmlDeserialize<MultiGeometry>(xdoc),
        "Polygon" => XmlSupport.XmlDeserialize<Polygon>(xdoc),
        _ => null as Coordinates
    }) ?? throw new NotSupportedException($"XML document not supported: {xdoc}");

    public Region[] Countries => _Countries.Result;

    private readonly Task<Region[]> _Countries;

    private class WorldCountryCsv
    {
        public string Name { get; set; } = null!;
        public string geometry { get; set; } = null!;
    }
}

#endif
