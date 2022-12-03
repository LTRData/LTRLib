// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

[ComVisible(false)]
public sealed class SequenceEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
{

    public IEqualityComparer<T> ItemComparer { get; set; }

    public SequenceEqualityComparer(IEqualityComparer<T> comparer)
    {
        ItemComparer = comparer;
    }

    public SequenceEqualityComparer()
    {
        ItemComparer = EqualityComparer<T>.Default;
    }

    public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        => ReferenceEquals(x, y) || (x is not null && y is not null && x.SequenceEqual(y, ItemComparer));

    public int GetHashCode(IEnumerable<T> obj)
    {
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
        var result = new HashCode();

        foreach (var item in obj)
        {
            if (item is not null)
            {
                result.Add(ItemComparer.GetHashCode(item));
            }
        }

        return result.ToHashCode();
#else
        var result = default(int);
        
        foreach (var item in obj)
        {
            if (item is not null)
            {
                result ^= ItemComparer.GetHashCode(item);
            }
        }

        return result;
#endif
    }
}

[ComVisible(false)]
public sealed class SequenceComparer<T> : IComparer<IEnumerable<T>>
{

    public IComparer<T> ItemComparer { get; set; }

    public SequenceComparer(IComparer<T> comparer)
    {
        ItemComparer = comparer;
    }

    public SequenceComparer()
    {
        ItemComparer = Comparer<T>.Default;
    }

    public int Compare(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }
        else if (y is null)
        {
            return 1;
        }

        using var enumx = x.GetEnumerator();
        using var enumy = y.GetEnumerator();

        for(; ;)
        {
            var enumxresult = enumx.MoveNext();
            var enumyresult = enumy.MoveNext();

            if (!enumxresult && !enumyresult)
            {
                return 0;
            }
            else if (!enumxresult)
            {
                return -1;
            }
            else if (!enumyresult)
            {
                return 1;
            }

            var value = ItemComparer.Compare(enumx.Current, enumy.Current);
            if (value != 0)
            {
                return value;
            }
        }
    }
}

#endif
