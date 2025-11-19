#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Collections;
#endif
using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;

namespace LTRLib.Extensions;

public static class DisposableExtensions
{
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static DisposableList<T> ToDisposableList<T>(this IEnumerable<T> collection) where T : IDisposable => [.. collection];
#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static DisposableDictionary<TKey, TValue> ToDisposableDictionary<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> keySelector)
        where TKey : notnull
        where TValue : IDisposable
    {
        var dict = collection is ICollection<TValue> collectionTValue
            ? new DisposableDictionary<TKey, TValue>(collectionTValue.Count)
            : [];

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
