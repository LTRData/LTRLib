/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LTRLib.LTRGeneric;

public class SimplePropertyDescriptor : PropertyDescriptor
{
    private readonly Func<object?, object?>? getObject;
    private readonly PropertyInfo property;

    private class SimpleMemberDescriptor : MemberDescriptor
    {
        public SimpleMemberDescriptor(PropertyInfo prop)
            : base(prop.Name, [.. prop.GetCustomAttributes(inherit: false).OfType<Attribute>()])
        {
        }
    }

    public SimplePropertyDescriptor(
        PropertyInfo prop, Func<object?, object?> getObjectFunc)
        : base(new SimpleMemberDescriptor(prop))
    {
        property = prop;
        getObject = getObjectFunc;
    }

    public SimplePropertyDescriptor(
        PropertyInfo prop)
        : base(new SimpleMemberDescriptor(prop))
    {
        property = prop;
    }

    public override Type ComponentType => GetType();

    public override bool IsReadOnly => !property.CanWrite;

    public override Type PropertyType => property.PropertyType;

    public override bool CanResetValue(object component) => false;

    public override object? GetValue(object? component)
    {
        component = getObject is not null ? getObject(component) : component;
        if (component is null)
        {
            return null;
        }

        return property.GetValue(component, null);
    }

    public override void ResetValue(object component) => throw new NotImplementedException();

    public override void SetValue(object? component, object? value)
    {
        component = getObject is not null ? getObject(component) : component;
        if (component is null)
        {
            return;
        }

        property.SetValue(component, value, null);
    }

    public override bool ShouldSerializeValue(object component) => false;

    public override string ToString() => DisplayName;

    public static IEnumerable<SimplePropertyDescriptor> CreateFromTypeProperties(Type type, Func<object?, object?> getComponent) => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Select(p => new SimplePropertyDescriptor(p, getComponent));
}

#endif
