using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTRLib.Extensions;

public static class DisposableExtensions
{
    public static DisposableList<T> ToDisposableList<T>(this IEnumerable<T> collection) where T : IDisposable => new(collection);

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static DisposableDictionary<TKey, TValue> ToDisposableDictionary<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> keySelector)
        where TKey : notnull
        where TValue : IDisposable
    {
        var dict = collection is ICollection<TValue> collectionTValue
            ? new DisposableDictionary<TKey, TValue>(collectionTValue.Count)
            : new DisposableDictionary<TKey, TValue>();

        foreach (var item in collection)
        {
            dict.Add(keySelector(item), item);
        }

        return dict;
    }

    public static DisposableDictionary<TKey, TValue> ToDisposableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        where TKey : notnull
        where TValue : IDisposable
        => new(dict);

#endif

}
