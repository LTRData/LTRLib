// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using ArraySupport = System.Array;
using System.Linq;
using System;
using System.Reflection;

namespace LTRLib.LTRGeneric;

public class FieldComparer
{
    private class MemberHolder<TSource, TTarget>
    {
        public static readonly FieldInfo[][] fields;

        public static readonly PropertyInfo[][] properties;

        static MemberHolder()
        {
            fields = [.. from field1 in typeof(TSource).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                      join field2 in typeof(TTarget).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) on field1.Name equals field2.Name
                      select new[] { field1, field2 }];

            properties = [.. from prop1 in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                          where prop1.GetIndexParameters().Length == 0 && prop1.CanRead && prop1.CanWrite
                          join prop2 in typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) on prop1.Name equals prop2.Name
                          where prop2.GetIndexParameters().Length == 0 && prop2.CanRead && prop2.CanWrite
                          select new[] { prop1, prop2 }];
        }
    }

    public static bool Equals<TSource, TTarget>(TSource obj1, TTarget obj2)
    {
        if (MemberHolder<TSource, TTarget>.fields.Length == 0)
        {
            throw new Exception($"No accessible (public or private + instance + non-inherited) fields in types {typeof(TSource)} and {typeof(TTarget)}");
        }

        return ArraySupport.TrueForAll(MemberHolder<TSource, TTarget>.fields, field => Equals(field[0].GetValue(obj1), field[1].GetValue(obj2)));
    }

    public static void CopyProperties<TSource, TTarget>(TSource source, TTarget target)
    {
        if (MemberHolder<TSource, TTarget>.properties.Length == 0)
        {
            throw new Exception($"No accessible (public + instance + read + write + non-indexed + non-inherited) properties in types {typeof(TSource)} and {typeof(TTarget)}");
        }

        ArraySupport.ForEach(MemberHolder<TSource, TTarget>.properties, prop => prop[1].SetValue(target, prop[0].GetValue(source, null), null));
    }

    public static TTarget Convert<TSource, TTarget>(TSource source) where TTarget : new()
    {
        if (MemberHolder<TSource, TTarget>.properties.Length == 0)
        {
            throw new Exception($"No accessible (public or private + instance + non-inherited) fields in types {typeof(TSource)} and {typeof(TTarget)}");
        }

        var target = new TTarget();
        ArraySupport.ForEach(MemberHolder<TSource, TTarget>.properties, prop => prop[1].SetValue(target, prop[0].GetValue(source, null), null));
        return target;
    }
}

#endif
