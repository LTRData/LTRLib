using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LTRLib.LTRGeneric;

public static class SingleValueEnumerable
{
    public static SingleValueEnumerable<T?> Get<T>(T? value) => new(value);
}

public readonly struct SingleValueEnumerable<T> : IEnumerable<T?>
{
    public T? Value { get; }

    public SingleValueEnumerable(T value)
    {
        Value = value;
    }

    public IEnumerator<T?> GetEnumerator() => new SingleValueEnumerator(Value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct SingleValueEnumerator : IEnumerator<T?>
    {
        public SingleValueEnumerator(T? value) : this()
        {
            Value = value;
        }

        public T? Value { get; }

        public bool Started { get; private set; }

        public T? Current => Started ? Value : default;

        object? IEnumerator.Current => Current;

        void IDisposable.Dispose() { }

        public bool MoveNext()
        {
            if (Started)
            {
                return false;
            }

            Started = true;
            return true;
        }

        public void Reset() => Started = false;
    }
}
