/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0618 // Type or member is obsolete

namespace LTRLib.Geodesy.Positions;

[Guid("b2b50306-dc97-4a44-b948-b6dec26da09a")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class GridSquare : IXmlSerializable
{
    public string Gridsquare
    {
        get => new WGS84Position((corner_ne.Latitude - corner_sw.Latitude) / 2,
                (corner_ne.Longitude - corner_sw.Longitude) / 2).ToMaidenhead();
        set
        {
            switch (value.Length)
            {
                case 8:
                case 6:
                case 4:
                case 2:
                    break;

                default:
                    throw new ArgumentException("gridsquare");
            }

            corner_sw.SetMaidenhead(value, LatLonPosition.GridSquarePosition.SouthWest);
            corner_ne.SetMaidenhead(value, LatLonPosition.GridSquarePosition.NorthEast);
        }
    }

    private readonly WGS84Position corner_sw = new();
    private readonly WGS84Position corner_ne = new();

    public double SouthLatitude
    {
        get => corner_sw.Latitude;
        set => corner_sw.Latitude = value;
    }

    public double NorthLatitude
    {
        get => corner_ne.Latitude;
        set => corner_ne.Latitude = value;
    }

    public double WestLongitude
    {
        get => corner_sw.Latitude;
        set => corner_sw.Latitude = value;
    }

    public double EastLongitude
    {
        get => corner_ne.Latitude;
        set => corner_ne.Latitude = value;
    }

    public GridSquare()
    {
    }

    public GridSquare(string gridsquare)
    {
        Gridsquare = gridsquare;
    }

    public override string ToString() =>
        corner_sw.Longitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        corner_sw.Latitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        "0 " +
        corner_ne.Longitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        corner_sw.Latitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        "0 " +
        corner_ne.Longitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        corner_ne.Latitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        "0 " +
        corner_sw.Longitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        corner_ne.Latitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        "0 " +
        corner_sw.Longitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        corner_sw.Latitude.ToString(NumberFormatInfo.InvariantInfo) + "," +
        "0 ";

    public virtual void ReadXml(XmlReader reader) => Gridsquare = reader.ReadElementContentAsString();

    public virtual void WriteXml(XmlWriter writer) => writer.WriteRaw(Gridsquare);

    public virtual XmlSchema GetSchema() => null;
}
