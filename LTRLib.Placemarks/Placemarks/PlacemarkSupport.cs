/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Extensions;
using LTRLib.Geodesy.Positions;
using LTRLib.LTRGeneric;
using LTRLib.Services.kml.simplified;
using Coordinates = LTRLib.Services.kml.simplified.Coordinates;
using KmlPolygon = LTRLib.Services.kml.simplified.Polygon;
using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;
using Polygon = NetTopologySuite.Geometries.Polygon;
using LinearRing = NetTopologySuite.Geometries.LinearRing;
using MultiPolygon = NetTopologySuite.Geometries.MultiPolygon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NetTopologySuite.IO;
using System.Security;
using NetTopologySuite.IO.KML;
using System.IO;
using NetTopologySuite.Operation.OverlayNG;
using NetTopologySuite.Operation.Distance;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

namespace LTRLib.Placemarks;

public static class PlacemarkSupport
{
    private static WKTReader? _wktReader;
    public static WKTReader GetWKTReader() => _wktReader ??= new();

    private static KMLReader? _kmlReader;
    public static KMLReader GetKMLReader() => _kmlReader ??= new();

    public static Geometry GeometryFromText(string wkt)
    {
        if (wkt.StartsWith("POINT (", StringComparison.Ordinal))
        {
            var xy = wkt.AsMemory("POINT (".Length).TrimEnd(')').Split(' ').Take(2).ToArray();
            return PointFromLatLon(xy[1].Span, xy[0].Span);
        }
        else
        {
            return GetWKTReader().Read(wkt);
        }
    }

    public static double InherentScale(double dec) => PrecisionUtility.InherentScale(dec);

    public static Geometry PointFromLatLon(string latitude, string longitude) =>
        PointFromLatLon(double.Parse(latitude, NumberFormatInfo.InvariantInfo),
            double.Parse(longitude, NumberFormatInfo.InvariantInfo));

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static Geometry PointFromLatLon(ReadOnlySpan<char> latitude, ReadOnlySpan<char> longitude) =>
        PointFromLatLon(double.Parse(latitude, provider: NumberFormatInfo.InvariantInfo),
            double.Parse(longitude, provider: NumberFormatInfo.InvariantInfo));
#else
    public static Geometry PointFromLatLon(ReadOnlySpan<char> latitude, ReadOnlySpan<char> longitude) =>
        PointFromLatLon(double.Parse(latitude.ToString(), provider: NumberFormatInfo.InvariantInfo),
            double.Parse(longitude.ToString(), provider: NumberFormatInfo.InvariantInfo));
#endif

    public static Geometry PointFromLatLon(double latitude, double longitude) =>
        new Point(longitude, latitude);

    public static Geometry PointFromGeoPosition(WGS84Position position) =>
        new Point(position.Longitude, position.Latitude);

    public static double GetNearestGeographicalDistance(this Geometry geometry, WGS84Position position)
    {
        var distanceop = new DistanceOp(geometry, PointFromGeoPosition(position)).NearestPoints();
        var geoposition = new WGS84Position(distanceop[0].Y, distanceop[0].X);
        var geodistance = geoposition.GetSurfaceDistance(position);
        return geodistance;
    }

    public static double GetNearestGeographicalDistance(this Geometry geometry, Point position)
    {
        var distanceop = new DistanceOp(geometry, position).NearestPoints();
        var geoposition = new WGS84Position(distanceop[0].Y, distanceop[0].X);
        var geodistance = geoposition.GetSurfaceDistance(new WGS84Position(position.Y, position.X));
        return geodistance;
    }

    public static double GetNearestGeographicalBearing(this Geometry geometry, WGS84Position position)
    {
        var poscoordinate = new Coordinate(position.Longitude, position.Latitude);
        var coordinates = geometry.Coordinates;
        var segment = coordinates
            .Take(coordinates.Length - 1)
            .Select((item, i) => new LineSegment(item, coordinates[i + 1]))
            .OrderBy(s => s.Distance(poscoordinate))
            .First();
        var segmentbearing = new WGS84Position(segment.P0.Y, segment.P0.X).GetBearing(new WGS84Position(segment.P1.Y, segment.P1.X));
        return segmentbearing;
    }

    public static double GetNearestGeographicalBearing(this Geometry geometry, Point position)
    {
        var coordinates = geometry.Coordinates;
        var segment = coordinates
            .Take(coordinates.Length - 1)
            .Select((item, i) => new LineSegment(item, coordinates[i + 1]))
            .OrderBy(s => s.Distance(position.Coordinate))
            .First();
        var segmentbearing = new WGS84Position(segment.P0.Y, segment.P0.X).GetBearing(new WGS84Position(segment.P1.Y, segment.P1.X));
        return segmentbearing;
    }

    public static (WGS84Position P0, WGS84Position P1) GetNearestGeographicalSegment(this Geometry geometry, WGS84Position position)
    {
        var poscoordinate = new Coordinate(position.Longitude, position.Latitude);
        var coordinates = geometry.Coordinates;
        var segment = coordinates
            .Take(coordinates.Length - 1)
            .Select((item, i) => new LineSegment(item, coordinates[i + 1]))
            .OrderBy(s => s.Distance(poscoordinate))
            .First();
        var segmentwgs84 = (P0: new WGS84Position(segment.P0.Y, segment.P0.X), P1: new WGS84Position(segment.P1.Y, segment.P1.X));
        return segmentwgs84;
    }

    public static (WGS84Position P0, WGS84Position P1) GetNearestGeographicalSegment(this Geometry geometry, Point position)
    {
        var poscoordinate = position.Coordinate;
        var coordinates = geometry.Coordinates;
        var segment = coordinates
            .Take(coordinates.Length - 1)
            .Select((item, i) => new LineSegment(item, coordinates[i + 1]))
            .OrderBy(s => s.Distance(poscoordinate))
            .First();
        var segmentwgs84 = (P0: new WGS84Position(segment.P0.Y, segment.P0.X), P1: new WGS84Position(segment.P1.Y, segment.P1.X));
        return segmentwgs84;
    }

    public static Region? FindRegion(this IEnumerable<Region> list, Geometry point, double distanceTolerance, ref Region? last_found)
    {
        if ((last_found is null) || (!last_found.polygons.Contains(point)))
        {
            last_found = list.FirstOrDefault(muni =>
                {
                    try
                    {
                        return muni.polygons.Contains(point);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
        }

        if (distanceTolerance > 0 && last_found is null)
        {
            var closest = list
                .Select(region => new { muni = region, dist = region.polygons.Distance(point) })
                .OrderBy(reg_dist => reg_dist.dist)
                .First();

            var distance = closest.dist * 111195;

            if (distance < distanceTolerance)
            {
                Trace.WriteLine($"Distance to {closest.muni.name} is {distance}");
                Trace.WriteLine($"Accepted as close enough to {closest.muni.name}");
                last_found = closest.muni;
            }
        }

        return last_found;
    }

    private static Region PlacemarkToMunicipality(this Placemark placemark)
    {
        try
        {
            var id = placemark.ExtendedData?.SchemaData?.First(data => "ID" == data.name || "LKFV" == data.name);
            var name = placemark.ExtendedData?.SchemaData?.First(data => data.name.EndsWith("NAMN", StringComparison.Ordinal));

            return new Region
            {
                id = id?.value,
                name = name?.value,
                polygons = GetKMLReader().Read(placemark.Item.ToXmlString())
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error importing placemark. Source data: {placemark.ToXmlString()}", ex);
        }
    }

    public static Region[] ParseKmlRegions(string kmlFile) => ParseKmlRegions(XmlSupport.XmlDeserialize<kml>(kmlFile)
        ?? throw new ArgumentException($"Invalid XML data in file '{kmlFile}'"));

    public static Region[] ParseKmlRegions(this kml kmlData) => ParseKmlRegions(kmlData.Document.Folders?[0].Placemarks
        ?? throw new ArgumentException("Invalid kml document"));

    public static async Task<Region[]> ParseKmlRegions(this Task<kml> kmlData) => ParseKmlRegions((await kmlData.ConfigureAwait(false)).Document.Folders?[0].Placemarks
        ?? throw new ArgumentException("Invalid kml document"));

    public static Region[] ParseKmlRegions(this IEnumerable<Placemark> kmlPlacemarks)
    {
        return kmlPlacemarks
            .Select(PlacemarkToMunicipality)
            .Where(item => item.polygons.IsValid)
            .ToArray();
    }

    public static async Task<Region[]> ParseKmlRegions(this Task<IEnumerable<Placemark>> kmlPlacemarks)
    {
        return (await kmlPlacemarks.ConfigureAwait(false))
            .Select(PlacemarkToMunicipality)
            .Where(item => item.polygons.IsValid)
            .ToArray();
    }

    public static string? FindRegionForPoint(this IEnumerable<Region> regions, Geometry point, double distanceTolerance, ref Region? last_found)
    {
        last_found = regions.FindRegion(point, distanceTolerance, ref last_found);

        if (last_found is null)
        {
            return null;
        }
        else
        {
            return last_found.name;
        }
    }

    public static string? FindRegionForPoint(this IEnumerable<Region> regions, double latitude, double longitude, double distanceTolerance, ref Region? last_found)
    {
        var point = PointFromLatLon(latitude, longitude);

        return regions.FindRegionForPoint(point, distanceTolerance, ref last_found);
    }

    public static string? FindRegionForPoint(this IEnumerable<Region> regions, string latitude, string longitude, double distanceTolerance, ref Region? last_found)
    {
        var point = PointFromLatLon(latitude, longitude);

        return regions.FindRegionForPoint(point, distanceTolerance, ref last_found);
    }

    public static Task<Region[]> ParseKmlRegionsAsync(string xmlPath) => Task.Run(() => ParseKmlRegions(xmlPath));

    public static Task<Region[]> ParseKmlRegionsAsync(this kml kmlData) => Task.Run(() => ParseKmlRegions(kmlData));

    public static Task<Region[]> ParseKmlRegionsAsync(this Task<kml> kmlDataTask) => kmlDataTask.ContinueWith(kmlData => ParseKmlRegions(kmlData.Result));

    public static Task<Region[]> ParseKmlRegionsAsync(this Task<Placemark[]> kmlDataTask) => kmlDataTask.ContinueWith(kmlData => ParseKmlRegions(kmlData.Result));
}

#endif
