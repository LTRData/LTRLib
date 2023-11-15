using System;
using System.Runtime.Serialization;
#if NET462_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Text.Json;
using System.Text.Json.Serialization;
#endif
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

namespace LTRLib.LTRGeneric;

#if NET462_OR_GREATER || NETSTANDARD || NETCOREAPP
[JsonConverter(typeof(ConfiguredLocalDateTimeConverter))]
#endif
public struct ConfiguredLocalDateTime(DateTime DateTime) : IXmlSerializable, IComparable<DateTime>, IComparable<ConfiguredLocalDateTime>, IEquatable<DateTime>, IEquatable<ConfiguredLocalDateTime>, IConvertible, IFormattable, ISerializable
{
    public DateTime DateTime { get; private set; } = DateTime;

    public readonly DateTime Date => DateTime.Date;

    public readonly TimeSpan TimeOfDay => DateTime.TimeOfDay;

    public static DateTime Now => TimeZoneSupport.CurrentConfiguredTimeZoneLocalTime;

    public static DateTime UtcNow => DateTime.UtcNow;

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();
        var dt = reader.ReadElementContentAsDateTime();
        DateTime = TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), TimeZoneSupport.ConfiguredTimeZone);
    }

    public readonly void WriteXml(XmlWriter writer) => writer.WriteString(XmlConvert.ToString(DateTime, XmlDateTimeSerializationMode.Unspecified));

    readonly XmlSchema? IXmlSerializable.GetSchema() => null;

    public static implicit operator DateTime(ConfiguredLocalDateTime XmlDateTime) => XmlDateTime.DateTime;

    public static implicit operator ConfiguredLocalDateTime(DateTime DateTime) => new(DateTime);

    public override readonly bool Equals(object? obj)
        => obj switch
        {
            ConfiguredLocalDateTime time => time.DateTime.Equals(DateTime),
            DateTime dateTime => dateTime.Equals(DateTime),
            _ => false
        };

    public override readonly int GetHashCode() => DateTime.GetHashCode();

    public override readonly string ToString() => DateTime.ToString();

    public readonly int CompareTo(DateTime other) => DateTime.CompareTo(other);

    public readonly int CompareTo(ConfiguredLocalDateTime other) => DateTime.CompareTo(other.DateTime);

    public readonly bool Equals(DateTime other) => DateTime.Equals(other);

    public readonly bool Equals(ConfiguredLocalDateTime other) => DateTime.Equals(other.DateTime);

    public static bool operator ==(ConfiguredLocalDateTime first, ConfiguredLocalDateTime second) => first.Equals(second);

    public static bool operator !=(ConfiguredLocalDateTime first, ConfiguredLocalDateTime second) => !first.Equals(second);

    public static bool operator ==(ConfiguredLocalDateTime first, DateTime second) => first.Equals(second);

    public static bool operator !=(ConfiguredLocalDateTime first, DateTime second) => !first.Equals(second);

    public static bool operator ==(DateTime first, ConfiguredLocalDateTime second) => first.Equals(second.DateTime);

    public static bool operator !=(DateTime first, ConfiguredLocalDateTime second) => !first.Equals(second.DateTime);

    public static bool operator <(ConfiguredLocalDateTime first, DateTime second) => first.DateTime < second;

    public static bool operator >(ConfiguredLocalDateTime first, DateTime second) => first.DateTime > second;

    public static bool operator <=(ConfiguredLocalDateTime first, DateTime second) => first.DateTime <= second;

    public static bool operator >=(ConfiguredLocalDateTime first, DateTime second) => first.DateTime >= second;

    public static bool? operator <(ConfiguredLocalDateTime first, DateTime? second) => first.DateTime < second;

    public static bool? operator >(ConfiguredLocalDateTime first, DateTime? second) => first.DateTime > second;

    public static bool? operator <=(ConfiguredLocalDateTime first, DateTime? second) => first.DateTime <= second;

    public static bool? operator >=(ConfiguredLocalDateTime first, DateTime? second) => first.DateTime >= second;

    public static DateTime operator +(ConfiguredLocalDateTime @base, TimeSpan offset) => @base.DateTime + offset;

    public static DateTime operator -(ConfiguredLocalDateTime @base, TimeSpan offset) => @base.DateTime - offset;

    public static TimeSpan operator -(ConfiguredLocalDateTime first, ConfiguredLocalDateTime second) => first.DateTime - second.DateTime;

    public static TimeSpan operator -(ConfiguredLocalDateTime first, DateTime second) => first.DateTime - second;

    public readonly TypeCode GetTypeCode() => DateTime.GetTypeCode();

    public readonly bool ToBoolean(IFormatProvider? provider) => ((IConvertible)DateTime).ToBoolean(provider);

    public readonly byte ToByte(IFormatProvider? provider) => ((IConvertible)DateTime).ToByte(provider);

    public readonly char ToChar(IFormatProvider? provider) => ((IConvertible)DateTime).ToChar(provider);

    public readonly DateTime ToDateTime(IFormatProvider? provider) => ((IConvertible)DateTime).ToDateTime(provider);

    public readonly decimal ToDecimal(IFormatProvider? provider) => ((IConvertible)DateTime).ToDecimal(provider);

    public readonly double ToDouble(IFormatProvider? provider) => ((IConvertible)DateTime).ToDouble(provider);

    public readonly short ToInt16(IFormatProvider? provider) => ((IConvertible)DateTime).ToInt16(provider);

    public readonly int ToInt32(IFormatProvider? provider) => ((IConvertible)DateTime).ToInt32(provider);

    public readonly long ToInt64(IFormatProvider? provider) => ((IConvertible)DateTime).ToInt64(provider);

    public readonly sbyte ToSByte(IFormatProvider? provider) => ((IConvertible)DateTime).ToSByte(provider);

    public readonly float ToSingle(IFormatProvider? provider) => ((IConvertible)DateTime).ToSingle(provider);

    public readonly string ToString(IFormatProvider? provider) => DateTime.ToString(provider);

    public readonly object ToType(Type conversionType, IFormatProvider? provider) => ((IConvertible)DateTime).ToType(conversionType, provider);

    public readonly ushort ToUInt16(IFormatProvider? provider) => ((IConvertible)DateTime).ToUInt16(provider);

    public readonly uint ToUInt32(IFormatProvider? provider) => ((IConvertible)DateTime).ToUInt32(provider);

    public readonly ulong ToUInt64(IFormatProvider? provider) => ((IConvertible)DateTime).ToUInt64(provider);

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => DateTime.ToString(format, formatProvider);

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public readonly void GetObjectData(SerializationInfo info, StreamingContext context) => ((ISerializable)DateTime).GetObjectData(info, context);
}
#endif

#if NET462_OR_GREATER || NETSTANDARD || NETCOREAPP
public class ConfiguredLocalDateTimeConverter : JsonConverter<ConfiguredLocalDateTime>
{
    public override ConfiguredLocalDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dt = reader.GetDateTime();
        return TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), TimeZoneSupport.ConfiguredTimeZone);
    }

    public override void Write(Utf8JsonWriter writer, ConfiguredLocalDateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(XmlConvert.ToString(value, XmlDateTimeSerializationMode.Unspecified));
}
#endif
