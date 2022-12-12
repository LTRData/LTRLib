#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DbGeometry = NetTopologySuite.Geometries.Geometry;
using DbPolygon = NetTopologySuite.Geometries.Polygon;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable IDE1006 // Naming Styles

namespace LTRLib.Services.kml.simplified;

// LTRLib.Services.kml.simplified.BallonStyle
public class BallonStyle
{
	public string? bgColor { get; set; }

	public string? textColor { get; set; }

	public string? text { get; set; }

	public string? displayMode { get; set; }
}

// LTRLib.Services.kml.simplified.Coordinates
[XmlInclude(typeof(MultiGeometry))]
[XmlInclude(typeof(Point))]
[XmlInclude(typeof(LineString))]
[XmlInclude(typeof(LinearRing))]
[XmlInclude(typeof(Polygon))]
public abstract class Coordinates : KmlObjectBase
{
	public string? tessellate { get; set; }

	public Placemark? outerBoundaryIs { get; set; }

    /// <summary>
    /// Coordinates in the form lon,lat[,alt] [space lon,lat[,alt]] ...
    /// </summary>
    public string coordinates { get; set; } = null!;

	public Coordinates()
	{
	}

	public Coordinates(IEnumerable<KmlPosition> coordinatesArray)
	{
        coordinates = string.Join(" ", coordinatesArray.Select(c =>
        {
            var list = new List<string>(3) { c.longitude, c.latitude };
            if (c.altitude is not null
                && !string.IsNullOrWhiteSpace(c.altitude)
                && c.altitude != "0")
            {
                list.Add(c.altitude);
            }

            return string.Join(",", list);
        }));
	}

	public KmlPosition[]? ToArray()
	{
		if (coordinates == null)
		{
			return null;
		}

		return Array.ConvertAll(coordinates.Split(' ', StringSplitOptions.RemoveEmptyEntries), coord =>
		{
			var array = coord.Split(',', StringSplitOptions.RemoveEmptyEntries);
			return new KmlPosition
			{
				longitude = array[0],
				latitude = array[1],
                altitude = (array.Length >= 3) ? array[2] : null
			};
		});
	}
}

// LTRLib.Services.kml.simplified.ExtendedData

public class ExtendedData
{
	public SimpleData[]? SchemaData { get; set; }

	[XmlAttribute]
	public string? schemaUrl { get; set; }
}

// LTRLib.Services.kml.simplified.IconStyle
public class IconStyle
{
    [XmlAttribute]
    public string id { get; set; } = null!;

    public Icon? Icon { get; set; }
}

// LTRLib.Services.kml.simplified.Icon
public class Icon
{
    public string href { get; set; } = "http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png";

    public double scale { get; set; } = 1;
}

// LTRLib.Services.kml.simplified.kml

[XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
[XmlRoot("kml", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
public class kml
{
	public KmlFolder Document { get; set; }

	public kml()
	{
		Document = new KmlFolder();
	}
}

// LTRLib.Services.kml.simplified.KmlFolder

public class KmlFolder : KmlObjectBase
{
    [XmlElement("Placemark")]
    public Placemark[]? Placemarks { get; set; }

	[XmlElement("Folder")]
	public KmlFolder[]? Folders { get; set; }
}

// LTRLib.Services.kml.simplified.KmlObjectBase

[XmlInclude(typeof(KmlFolder))]
[XmlInclude(typeof(Placemark))]
[XmlInclude(typeof(Coordinates))]
public abstract class KmlObjectBase
{
	public string? name { get; set; }

	public string? open { get; set; }

	public string? styleUrl { get; set; }

	public string? description { get; set; }

	[XmlElement("Style")]
	public Style[]? Styles { get; set; }

	[XmlElement("LookAt")]
	public KmlPosition? LookAt { get; set; }
}

// LTRLib.Services.kml.simplified.KmlPosition
public class KmlPosition
{
	public string longitude { get; set; } = null!;

    public string latitude { get; set; } = null!;

    public string? altitude { get; set; }

    public string heading { get; set; } = null!;

    public string tilt { get; set; } = null!;

    public string range { get; set; } = null!;
}

// LTRLib.Services.kml.simplified.LinearRing

public class LinearRing : Coordinates
{
	public LinearRing()
	{
	}

	public LinearRing(IEnumerable<KmlPosition> coordinatesArray)
		: base(coordinatesArray)
	{
	}
}

// LTRLib.Services.kml.simplified.LineString

public class LineString : Coordinates
{
	public LineString()
	{
	}

	public LineString(IEnumerable<KmlPosition> coordinatesArray)
		: base(coordinatesArray)
	{
	}
}

// LTRLib.Services.kml.simplified.LineStyle
public class LineStyle
{
	public string? color { get; set; }

	public double width { get; set; }
}

// LTRLib.Services.kml.simplified.MultiGeometry

public class MultiGeometry : Coordinates
{
	[XmlElement("LineString", typeof(LineString))]
	[XmlElement("Point", typeof(Point))]
	[XmlElement("Polygon", typeof(Polygon))]
	[XmlElement("LinearRing", typeof(LinearRing))]
	public Coordinates[] Items { get; set; } = null!;
}

// LTRLib.Services.kml.simplified.Pair
public class Pair
{
	public string key { get; set; } = null!;

    public string? styleUrl { get; set; }
}

// LTRLib.Services.kml.simplified.Placemark

public class Placemark : KmlObjectBase
{
    [XmlElement("LineString", typeof(LineString))]
    [XmlElement("Point", typeof(Point))]
    [XmlElement("Polygon", typeof(Polygon))]
    [XmlElement("LinearRing", typeof(LinearRing))]
    [XmlElement("MultiGeometry", typeof(MultiGeometry))]
    public Coordinates Item { get; set; } = null!;

	public ExtendedData? ExtendedData { get; set; }

	public StyleMap? StyleMap { get; set; }
}

// LTRLib.Services.kml.simplified.Point

public class Point : Coordinates
{
	public Point()
	{
	}

	public Point(IEnumerable<KmlPosition> coordinatesArray)
		: base(coordinatesArray)
	{
	}
}

// LTRLib.Services.kml.simplified.Polygon

public class Polygon : Coordinates
{
	public Polygon()
	{
	}

	public Polygon(IEnumerable<KmlPosition> coordinatesArray)
		: base(coordinatesArray)
	{
	}
}

// LTRLib.Services.kml.simplified.SimpleData

public class SimpleData
{
    [XmlAttribute]
    public string name { get; set; } = null!;

	[XmlText]
	public string? value { get; set; }
}

// LTRLib.Services.kml.simplified.Style

public class Style
{
    [XmlAttribute]
    public string id { get; set; } = null!;

    public IconStyle? IconStyle { get; set; }

	public LineStyle? LineStyle { get; set; }

	public BallonStyle? BallonStyle { get; set; }
}

// LTRLib.Services.kml.simplified.StyleMap

public class StyleMap
{
    [XmlAttribute]
    public string id { get; set; } = null!;

    [XmlElement("Pair")]
    public Pair[] Pairs { get; set; } = null!;
}

#endif
