// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using LTRLib.Extensions;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Buffers;
using LTRData.Extensions.Formatting;
using System.Linq;
using System.Xml.Linq;
#endif

namespace LTRLib.LTRGeneric;

public static class XmlSupport
{

    public static readonly XmlReaderSettings XmlReaderSettings = new() { IgnoreWhitespace = true, IgnoreComments = true };

    public static readonly XmlWriterSettings XmlWriterSettings = new() { IndentChars = "  ", Indent = true, OmitXmlDeclaration = true };

    public static readonly Dictionary<Type, XmlSerializer> XmlSerializers = [];

    public static readonly Dictionary<Type, XmlSerializerNamespaces> XmlNamespaces = [];

    public static readonly XmlSerializerNamespaces XmlSerializerEmptyNamespaces = new([new XmlQualifiedName("", "")]);

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static string[]? ArrayFromWebCompatibleQueryString(IEnumerable<string> @params)
    {
        if (@params is null)
        {
            return null;
        }

        var array = @params.Join(",").Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (array.Length == 0)
        {
            return null;
        }

        for (int i = array.GetLowerBound(0), loopTo = array.GetUpperBound(0); i <= loopTo; i++)
        {
            array[i] = array[i].Trim();
        }

        return array;
    }

    public static string[]? ArrayFromWebCompatibleQueryString(string @params)
    {
        if (@params is null)
        {
            return null;
        }

        var array = @params.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (array.Length == 0)
        {
            return null;
        }

        for (int i = array.GetLowerBound(0), loopTo = array.GetUpperBound(0); i <= loopTo; i++)
        {
            array[i] = array[i].Trim();
        }

        return array;
    }

    /// <summary>
    /// Enumerates XElement object from a stream of XML fragments
    /// </summary>
    /// <param name="xmlString">Source string that XML object will be read from</param>
    public static IEnumerable<XElement> EnumerateXElements(string xmlString) => EnumerateXElements(new StringReader(xmlString));

    /// <summary>
    /// Enumerates XElement object from a stream of XML fragments
    /// </summary>
    /// <param name="reader">Source where XML object will be read from</param>
    public static IEnumerable<XElement> EnumerateXElements(TextReader reader) => EnumerateXElements(XmlReader.Create(reader, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }));

    /// <summary>
    /// Enumerates XElement object from a stream of XML fragments
    /// </summary>
    /// <param name="reader">Source where XML object will be read from</param>
    public static IEnumerable<XElement> EnumerateXElements(Stream reader) => EnumerateXElements(XmlReader.Create(reader, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }));

    /// <summary>
    /// Enumerates XElement object from a stream of XML fragments
    /// </summary>
    /// <param name="reader">XmlReader object with ConformanceLevel set to Fragment</param>
    public static IEnumerable<XElement> EnumerateXElements(XmlReader reader)
    {

        while (reader.Read())
        {

            if (reader.NodeType == XmlNodeType.Element)
            {

                yield return XElement.Load(reader.ReadSubtree());

            }

        }

    }


#endif

    /// <summary>
    /// Deserializes an object stored as XML string.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML string</typeparam>
    /// <param name="XmlString">XML data that describes serialized object</param>
    public static T? CreateObjectFromXmlString<T>(string XmlString)
    {
        if (string.IsNullOrEmpty(XmlString))
        {
            return default;
        }

        return XmlDeserialize<T>(new StringReader(XmlString));
    }

    /// <summary>
    /// Deserializes an object stored as XML string.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML string</typeparam>
    /// <param name="XmlString">XML data that describes serialized object</param>
    /// <param name="defaultnamespace"></param>
    public static T? CreateObjectFromXmlString<T>(string XmlString, string defaultnamespace)
    {
        if (string.IsNullOrEmpty(XmlString))
        {
            return default;
        }

        return XmlDeserialize<T>(new StringReader(XmlString), defaultnamespace);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="data">XML data that describes serialized object</param>

    public static T? XmlDeserialize<T>(byte[] data)
        => XmlDeserialize<T>(new MemoryStream(data));

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="data">XML data that describes serialized object</param>
    /// <param name="defaultnamespace"></param>
    public static T? XmlDeserialize<T>(byte[] data, string defaultnamespace)
        => XmlDeserialize<T>(new MemoryStream(data), defaultnamespace);

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="stream">XML data stream that describes serialized object</param>

    public static T? XmlDeserialize<T>(Stream stream)
    {
        using var Reader = XmlReader.Create(stream, XmlReaderSettings);
        return XmlDeserialize<T>(Reader);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="stream">XML data stream that describes serialized object</param>
    /// <param name="defaultnamespace"></param>

    public static T? XmlDeserialize<T>(Stream stream, string defaultnamespace)
    {
        using var Reader = XmlReader.Create(stream, XmlReaderSettings);
        return XmlDeserialize<T>(Reader, defaultnamespace);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="path">XML file that describes serialized object</param>

    public static T? XmlDeserialize<T>(string path)
    {
        using var Reader = XmlReader.Create(path, XmlReaderSettings);
        return XmlDeserialize<T>(Reader);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="path">XML file that describes serialized object</param>
    /// <param name="defaultnamespace"></param>

    public static T? XmlDeserialize<T>(string path, string defaultnamespace)
    {
        using var Reader = XmlReader.Create(path, XmlReaderSettings);
        return XmlDeserialize<T>(Reader, defaultnamespace);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="reader">Reader that produces XML serialized object</param>
    public static T? XmlDeserialize<T>(XmlReader reader) => (T?)typeof(T).GetXmlSerializer().Deserialize(reader);

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="reader">Reader that produces XML serialized object</param>
    /// <param name="defaultnamespace"></param>

    public static T? XmlDeserialize<T>(XmlReader reader, string defaultnamespace) => (T?)typeof(T).GetXmlSerializer(defaultnamespace).Deserialize(reader);

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="element">XML object</param>

    public static T? XmlDeserialize<T>(XElement element)
    {
        using var Reader = element.CreateReader();
        return (T?)typeof(T).GetXmlSerializer().Deserialize(Reader);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="element">XML object</param>
    /// <param name="defaultnamespace"></param>

    public static T? XmlDeserialize<T>(XElement element, string defaultnamespace)
    {
        using var Reader = element.CreateReader();
        return (T?)typeof(T).GetXmlSerializer(defaultnamespace).Deserialize(Reader);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="element">XML object</param>

    public static T? XmlDeserialize<T>(XDocument element)
    {
        using var Reader = element.CreateReader();
        return (T?)typeof(T).GetXmlSerializer().Deserialize(Reader);
    }

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="element">XML object</param>
    /// <param name="defaultnamespace"></param>

    public static T? XmlDeserialize<T>(XDocument element, string defaultnamespace)
    {
        using var Reader = element.CreateReader();
        return (T?)typeof(T).GetXmlSerializer(defaultnamespace).Deserialize(Reader);
    }


#endif

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="reader">Reader that produces XML serialized object</param>

    public static T? XmlDeserialize<T>(TextReader reader) => (T?)typeof(T).GetXmlSerializer().Deserialize(reader);

    /// <summary>
    /// Deserializes an object stored as XML data.
    /// </summary>
    /// <typeparam name="T">Type of object stored as XML data</typeparam>
    /// <param name="reader">Reader that produces XML serialized object</param>
    /// <param name="defaultnamespace"></param>

    public static T? XmlDeserialize<T>(TextReader reader, string defaultnamespace) => (T?)typeof(T).GetXmlSerializer(defaultnamespace).Deserialize(reader);

}