using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

namespace LTRLib.LTRGeneric;


public struct ConfiguredLocalDateTime : IXmlSerializable, IComparable<DateTime>, IComparable<ConfiguredLocalDateTime>, IEquatable<DateTime>, IEquatable<ConfiguredLocalDateTime>, IConvertible, IFormattable, ISerializable
{
    public DateTime DateTime { get; private set; }

    public DateTime Date => DateTime.Date;

    public TimeSpan TimeOfDay => DateTime.TimeOfDay;

    public static DateTime Now => DateTimeSupport.CurrentConfiguredTimeZoneLocalTime;

    public static DateTime UtcNow => DateTime.UtcNow;

    public ConfiguredLocalDateTime(DateTime DateTime)
    {
        this.DateTime = DateTime;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();
        var dt = reader.ReadElementContentAsDateTime();
        DateTime = TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), DateTimeSupport.ConfiguredTimeZone);
    }

    public void WriteXml(XmlWriter writer) => writer.WriteString(XmlConvert.ToString(DateTime, XmlDateTimeSerializationMode.Unspecified));

    XmlSchema? IXmlSerializable.GetSchema() => null;

    public static implicit operator DateTime(ConfiguredLocalDateTime XmlDateTime) => XmlDateTime.DateTime;

    public static implicit operator ConfiguredLocalDateTime(DateTime DateTime) => new(DateTime);

    public override bool Equals(object? obj)
        => obj switch
        {
            ConfiguredLocalDateTime time => time.DateTime.Equals(DateTime),
            DateTime dateTime => dateTime.Equals(DateTime),
            _ => false
        };

    public override int GetHashCode() => DateTime.GetHashCode();

    public override string ToString() => DateTime.ToString();

    public int CompareTo(DateTime other) => DateTime.CompareTo(other);

    public int CompareTo(ConfiguredLocalDateTime other) => DateTime.CompareTo(other.DateTime);

    public bool Equals(DateTime other) => DateTime.Equals(other);

    public bool Equals(ConfiguredLocalDateTime other) => DateTime.Equals(other.DateTime);

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

    public TypeCode GetTypeCode() => DateTime.GetTypeCode();

    public bool ToBoolean(IFormatProvider? provider) => ((IConvertible)DateTime).ToBoolean(provider);

    public byte ToByte(IFormatProvider? provider) => ((IConvertible)DateTime).ToByte(provider);

    public char ToChar(IFormatProvider? provider) => ((IConvertible)DateTime).ToChar(provider);

    public DateTime ToDateTime(IFormatProvider? provider) => ((IConvertible)DateTime).ToDateTime(provider);

    public decimal ToDecimal(IFormatProvider? provider) => ((IConvertible)DateTime).ToDecimal(provider);

    public double ToDouble(IFormatProvider? provider) => ((IConvertible)DateTime).ToDouble(provider);

    public short ToInt16(IFormatProvider? provider) => ((IConvertible)DateTime).ToInt16(provider);

    public int ToInt32(IFormatProvider? provider) => ((IConvertible)DateTime).ToInt32(provider);

    public long ToInt64(IFormatProvider? provider) => ((IConvertible)DateTime).ToInt64(provider);

    public sbyte ToSByte(IFormatProvider? provider) => ((IConvertible)DateTime).ToSByte(provider);

    public float ToSingle(IFormatProvider? provider) => ((IConvertible)DateTime).ToSingle(provider);

    public string ToString(IFormatProvider? provider) => DateTime.ToString(provider);

    public object ToType(Type conversionType, IFormatProvider? provider) => ((IConvertible)DateTime).ToType(conversionType, provider);

    public ushort ToUInt16(IFormatProvider? provider) => ((IConvertible)DateTime).ToUInt16(provider);

    public uint ToUInt32(IFormatProvider? provider) => ((IConvertible)DateTime).ToUInt32(provider);

    public ulong ToUInt64(IFormatProvider? provider) => ((IConvertible)DateTime).ToUInt64(provider);

    public string ToString(string? format, IFormatProvider? formatProvider) => DateTime.ToString(format, formatProvider);

    public void GetObjectData(SerializationInfo info, StreamingContext context) => ((ISerializable)DateTime).GetObjectData(info, context);
}

#endif
