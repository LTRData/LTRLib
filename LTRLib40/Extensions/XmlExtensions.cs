// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using LTRLib.LTRGeneric;
using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
using System.Xml.Linq;
#endif

namespace LTRLib.Extensions;

public static class XmlExtensions
{

    /// <summary>
    /// Creates and returns an XML serializer for a specific type. The XML serializer is
    /// cached and returned again the next time an XML serializer is requested for the
    /// same type.
    /// </summary>
    /// <param name="type">A Type to create an XML serializer for</param>
    public static XmlSerializer GetXmlSerializer(this Type type)
    {
        var GetXmlSerializerRet = default(XmlSerializer);

        lock (XmlSupport.XmlSerializers)
        {
            GetXmlSerializerRet = null;
            
            if (XmlSupport.XmlSerializers.TryGetValue(type, out GetXmlSerializerRet) == true)
            {
                return GetXmlSerializerRet;
            }

            var defaultNamespace = type.GetCustomAttribute<XmlTypeAttribute>(true);
            
            if (defaultNamespace is not null && !string.IsNullOrEmpty(defaultNamespace.Namespace))
            {
                GetXmlSerializerRet = new XmlSerializer(type, defaultNamespace.Namespace);
            }
            else
            {
                GetXmlSerializerRet = new XmlSerializer(type);
            }

            XmlSupport.XmlSerializers.Add(type, GetXmlSerializerRet);
        }

        return GetXmlSerializerRet;
    }

    /// <summary>
    /// Creates and returns an XML serializer for a specific type. The XML serializer is
    /// cached and returned again the next time an XML serializer is requested for the
    /// same type.
    /// </summary>
    /// <param name="type">A Type to create an XML serializer for</param>
    /// <param name="defaultnamespace">Default XML namespace</param>
    public static XmlSerializer GetXmlSerializer(this Type type, string defaultnamespace)
    {
        var GetXmlSerializerRet = default(XmlSerializer);

        lock (XmlSupport.XmlSerializers)
        {
            GetXmlSerializerRet = null;
            if (XmlSupport.XmlSerializers.TryGetValue(type, out GetXmlSerializerRet) == true)
            {
                return GetXmlSerializerRet;
            }

            if (!string.IsNullOrEmpty(defaultnamespace))
            {
                GetXmlSerializerRet = new XmlSerializer(type, defaultnamespace);
            }
            else
            {
                GetXmlSerializerRet = new XmlSerializer(type);
            }

            XmlSupport.XmlSerializers.Add(type, GetXmlSerializerRet);
        }

        return GetXmlSerializerRet;
    }

    /// <summary>
    /// Creates and returns an XmlSerializerNamespaces object representing XML namespaces for
    /// a specific type. The object is cached and returned again the next time it is requested
    /// for the same type.
    /// 
    /// If type does not contain any XML namespace attributes, an empty namespace object is
    /// returned causing XML serializers to not use any namespace annotations for objects of
    /// this type.
    /// </summary>
    /// <param name="type">A Type to create an XmlSerializerNamespaces object for</param>
    public static XmlSerializerNamespaces GetXmlNamespaces(this Type type)
    {
        var GetXmlNamespacesRet = default(XmlSerializerNamespaces);

        lock (XmlSupport.XmlSerializers)
        {
            GetXmlNamespacesRet = null;

            if (XmlSupport.XmlNamespaces.TryGetValue(type, out GetXmlNamespacesRet) == true)
            {
                return GetXmlNamespacesRet;
            }
            
            var defaultNamespace = type.GetCustomAttribute<XmlTypeAttribute>(true);
            if (defaultNamespace is not null && !string.IsNullOrEmpty(defaultNamespace.Namespace))
            {
                GetXmlNamespacesRet = new XmlSerializerNamespaces();

                try
                {
                    GetXmlNamespacesRet.Add("", defaultNamespace.Namespace);
                }
                catch
                {
                }

                try
                {
                    GetXmlNamespacesRet.Add("", "");
                }
                catch
                {
                }
            }
            else
            {
                GetXmlNamespacesRet = XmlSupport.XmlSerializerEmptyNamespaces;
            }

            XmlSupport.XmlNamespaces.Add(type, GetXmlNamespacesRet);
        }

        return GetXmlNamespacesRet;
    }

    /// <summary>
    /// Serializes an object as an XML string.
    /// </summary>
    public static string ToXmlString(this object Obj)
    {
        if (Obj is null)
        {
            return string.Empty;
        }

        var sw = new StringWriter();

        using var XmlWr = XmlWriter.Create(sw, XmlSupport.XmlWriterSettings);
        Obj.XmlSerialize(XmlWr);
        XmlWr.Flush();
        return sw.ToString();
    }

    #if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Serializes an object as an XML document.
    /// </summary>
    public static XDocument? ToXDocument(this object Obj)
    {
        if (Obj is null)
        {
            return null;
        }

        var sw = new StringWriter();
        using var XmlWr = XmlWriter.Create(sw, XmlSupport.XmlWriterSettings);
        Obj.XmlSerialize(XmlWr);
        XmlWr.Flush();
        return XDocument.Parse(sw.ToString());
    }

    /// <summary>
    /// Serializes an object as an XML document.
    /// </summary>
    public static XElement? ToXElement(this object Obj)
    {
        if (Obj is null)
        {
            return null;
        }

        var sw = new StringWriter();
        using var XmlWr = XmlWriter.Create(sw, XmlSupport.XmlWriterSettings);
        Obj.XmlSerialize(XmlWr);
        XmlWr.Flush();
        return XElement.Parse(sw.ToString());
    }

    #endif

    /// <summary>
    /// XML serializes an object and returns a byte stream containing the serialized
    /// object.
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    public static byte[] XmlSerialize(this object obj)
    {
        var ms = new MemoryStream();
        obj.XmlSerialize(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// XML serializes an object to a stream.
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <param name="stream">Stream where XML data is stored</param>
    public static void XmlSerialize(this object obj, Stream stream) => obj.GetType().GetXmlSerializer().Serialize(stream, obj, obj.GetType().GetXmlNamespaces());

    /// <summary>
    /// XML serializes an object for storing by an XML writer.
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <param name="writer">Writer that stores output XML data</param>
    public static void XmlSerialize(this object obj, XmlWriter writer) => obj.GetType().GetXmlSerializer().Serialize(writer, obj, obj.GetType().GetXmlNamespaces());

    /// <summary>
    /// XML serializes an object for storing by a TextWriter object.
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <param name="writer">Writer that stores output XML data</param>
    public static void XmlSerialize(this object obj, TextWriter writer) => obj.GetType().GetXmlSerializer().Serialize(writer, obj, obj.GetType().GetXmlNamespaces());

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Generates keyhole markup language adjusted color string.
    /// </summary>
    /// <param name="lineColor">Input color value to convert</param>
    public static string ToAdjustedKmlColor(this Color lineColor)
    {
        var lineColorBytes = BitConverter.GetBytes(lineColor.ToArgb());

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lineColorBytes);
        }

        Array.Reverse(lineColorBytes, 1, 3);

        if (lineColorBytes.Skip(1).All(lineColorByte => lineColorByte < 128))
        {
            lineColorBytes[3] += 64;
            lineColorBytes[2] += 64;
            lineColorBytes[1] += 128;
        }
        else if (lineColorBytes.Skip(1).All(lineColorByte => lineColorByte > 192))
        {
            lineColorBytes[3] -= 64;
            lineColorBytes[2] -= 64;
        }

        var kmlColor = string.Concat(from lineColorByte in lineColorBytes
                                     select lineColorByte.ToString("x2"));
        return kmlColor;
    }

#endif
}