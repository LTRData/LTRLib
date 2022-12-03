// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

[ComVisible(false)]
public sealed class EquatableBox<T> : IEquatable<T>, IEquatable<EquatableBox<T>> where T : struct, IEquatable<T>
{
    public T Value { get; set; }

    public EquatableBox(T value)
    {
        Value = value;
    }

    public bool HasDefaultValue => Value.Equals(new T());

    public void ClearValue() => Value = new T();

    public static implicit operator EquatableBox<T>(T value) => new(value);

    public static implicit operator T(EquatableBox<T> box) => box.Value;

    public override string? ToString() => Value.ToString();

    public override int GetHashCode() => Value.GetHashCode();

    public bool Equals(EquatableBox<T>? other) => Value.Equals(other?.Value);

    public bool Equals(T other) => Value.Equals(other);

    public override bool Equals(object? obj)
    {
        if (obj is EquatableBox<T> box)
        {
            return Value.Equals(box.Value);
        }
        else if (obj is T t)
        {
            return Value.Equals(t);
        }
        else
        {
            return base.Equals(obj);
        }
    }

}