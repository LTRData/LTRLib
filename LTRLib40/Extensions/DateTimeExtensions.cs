// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Globalization;
using System.Xml;

namespace LTRLib.Extensions;

public static class DateTimeExtensions
{

    public enum MondayBasedDayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public static MondayBasedDayOfWeek ToMondayBased(this DayOfWeek dow)
    {
        if (dow == DayOfWeek.Sunday)
        {
            return MondayBasedDayOfWeek.Sunday;
        }
        else
        {
            return (MondayBasedDayOfWeek)((int)dow - 1);
        }
    }

    public static DayOfWeek ToSundayBased(this MondayBasedDayOfWeek dow)
    {
        if (dow == MondayBasedDayOfWeek.Sunday)
        {
            return DayOfWeek.Sunday;
        }
        else
        {
            return (DayOfWeek)((int)dow + 1);
        }
    }

    /// <summary>
    /// Gets the week of year for this instance by week numbering rules of calendar of current culture.
    /// </summary>
    /// <param name="date">DateTime instance</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static int GetWeekOfYear(this DateTime date) => date.GetWeekOfYear(DateTimeFormatInfo.CurrentInfo);

    /// <summary>
    /// Gets the week of year for this instance by week numbering rules of calendar in specified format.
    /// </summary>
    /// <param name="date">DateTime instance</param>
    /// <param name="format">Format specification with calendar rules</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static int GetWeekOfYear(this DateTime date, DateTimeFormatInfo format) => format.Calendar.GetWeekOfYear(date, format.CalendarWeekRule, format.FirstDayOfWeek);

    public static DateTime AddWeeks(this DateTime date, int weeks) => date.AddWeeks(weeks, DateTimeFormatInfo.CurrentInfo);

    public static DateTime AddWeeks(this DateTime date, int weeks, DateTimeFormatInfo format) => format.Calendar.AddWeeks(date, weeks);

    public static string ToStringYWWDD(this DateTime date) => (date.Year % 10).ToString() + date.GetWeekOfYear().ToString("00") + "-" + ((int)date.DayOfWeek).ToString();

    public static string ToStringYWWDD(this DateTime date, DateTimeFormatInfo format) => (date.Year % 10).ToString() + date.GetWeekOfYear(format).ToString("00") + "-" + ((int)date.DayOfWeek).ToString();

    public static string ToStringYYWWDD(this DateTime date) => date.Year.ToString("00") + date.GetWeekOfYear().ToString("00") + "-" + ((int)date.DayOfWeek).ToString();

    public static string ToStringYYWWDD(this DateTime date, DateTimeFormatInfo format) => date.Year.ToString("00") + date.GetWeekOfYear(format).ToString("00") + "-" + ((int)date.DayOfWeek).ToString();

    public static string ToXmlDateTime(this DateTime date, XmlDateTimeSerializationMode dateTimeOption) => XmlConvert.ToString(date, dateTimeOption);

}