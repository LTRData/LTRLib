#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LTRLib.Extensions;

public static class DataExtensions
{
    public static T RecordToEntityObject<T>(this IDataRecord record) where T : new() => RecordToEntityObject(record, new T());

    public static T RecordToEntityObject<T>(this IDataRecord record, T obj)
    {
        var props = ExpressionSupport.PropertiesAssigners<T>.Setters;

        for (int i = 0, loopTo = record.FieldCount - 1; i <= loopTo; i++)
        {
            if (props.TryGetValue(record.GetName(i), out var prop))
            {
                prop(obj, record[i] is DBNull ? null : record[i]);
            }
        }

        return obj;
    }
}

#endif
