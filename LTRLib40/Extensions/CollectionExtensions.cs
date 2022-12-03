using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LTRLib.Extensions;

public static class CollectionExtensions
{
#if NET35_OR_GREATER && !NET471_OR_GREATER

    public static IEnumerable<T> Append<T>(this IEnumerable<T> values, T nextValue) => values.Concat(SingleValueEnumerable.Get(nextValue));

    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> values, T previousValue) => SingleValueEnumerable.Get(previousValue).Concat(values);

#endif

    /// <summary>
    /// Adds or replaces a dictionary entry.
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="Dict">IDictionary(Of TKey, TValue) to add item to.</param>
    /// <param name="key">Key for item to add or replace.</param>
    /// <param name="item">Item to add or replace in IDictionary(Of TKey, TValue).</param>
    /// <returns>Returns True if element was added or False if an existing element was replaced.</returns>
    public static bool AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> Dict, TKey key, TValue item)
    {
        if (Dict.ContainsKey(key))
        {
            Dict[key] = item;
            return false;
        }
        else
        {
            Dict[key] = item;
            return true;
        }
    }

    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> Dict, IEnumerable<KeyValuePair<TKey, TValue>> itemPairs)
    {
        foreach (var itemPair in itemPairs)
        {
            Dict.Add(itemPair);
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> Dict, IEnumerable<(TKey Key, TValue Value)> itemPairs)
    {
        foreach (var (Key, Value) in itemPairs)
        {
            Dict.Add(Key, Value);
        }
    }
#endif

    /// <summary>
    /// Adds an element to an ICollection(Of T) if that element is not already present in
    /// the list.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the list.</typeparam>
    /// <param name="List">List to add element to.</param>
    /// <param name="item">Item to add to ICollection(Of T) object.</param>
    /// <returns>Returns True if element was added or False if already present in list.</returns>
    public static bool TryAddNew<T>(this ICollection<T> List, T item)
    {
        if (List.Contains(item))
        {
            return false;
        }
        else
        {
            List.Add(item);
            return true;
        }
    }

    /// <summary>
    /// Removes an element from an ICollection(Of T) if that element is present in the list.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the list.</typeparam>
    /// <param name="List">List to remove element from.</param>
    /// <param name="item">Item to remove from ICollection(Of T) object.</param>
    /// <returns>Returns True if element was removed or False if not present in list.</returns>
    public static bool TryRemove<T>(this ICollection<T> List, T item)
    {
        if (List.Contains(item))
        {
            List.Remove(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns value at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Value at specified index.</returns>
    /// <remarks>Throws an exception if index is out of bounds.</remarks>
    public static TValue ValueAt<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        else
        {
            return list[index].Value;
        }
    }

    /// <summary>
    /// Returns value at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Value at specified index.</returns>
    /// <remarks>Returns default value of TValue if index is out of bounds.</remarks>
    public static TValue? ValueAtOrDefault<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            return default;
        }
        else
        {
            return list[index].Value;
        }
    }

    /// <summary>
    /// Returns key at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Value at specified index.</returns>
    /// <remarks>Throws an exception if index is out of bounds.</remarks>
    public static TKey KeyAt<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        else
        {
            return list[index].Key;
        }
    }

    /// <summary>
    /// Returns key at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Key at specified index.</returns>
    /// <remarks>Returns default value of TKey if index is out of bounds.</remarks>
    public static TKey? KeyAtOrDefault<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            return default;
        }
        else
        {
            return list[index].Key;
        }
    }

    /// <summary>
    /// Returns KeyValuePair(Of TKey, TValue) at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>KeyValuePair(Of TKey, TValue) at specified index.</returns>
    /// <remarks>Throws an exception if index is out of bounds.</remarks>
    public static KeyValuePair<TKey, TValue> KeyValuePairAt<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        else
        {
            return list[index];
        }
    }

    /// <summary>
    /// Returns KeyValuePair(Of TKey, TValue) at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>KeyValuePair(Of TKey, TValue) at specified index.</returns>
    /// <remarks>Returns default value of KeyValuePair(Of TKey, TValue) if index is out of bounds.</remarks>
    public static KeyValuePair<TKey, TValue> KeyValuePairAtOrDefault<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            return default;
        }
        else
        {
            return list[index];
        }
    }

    /// <summary>
    /// Merges two arrays to a new array.
    /// </summary>
    public static T[] Extend<T>(this T[] a1, T[] a2)
    {
        if (a1 is null)
        {
            return a2;
        }
        else if (a2 is null)
        {
            return a1;
        }
        else
        {
            var a2Index = a1.Length;
            Array.Resize(ref a1, a1.Length + a2.Length);
            Array.Copy(a2, 0, a1, a2Index, a2.Length);
            return a1;
        }
    }

    /// <summary>
    /// Copies elements to another IList(Of T).
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    /// <param name="source">List to copy from.</param>
    /// <param name="target">List to copy to.</param>
    public static void CopyTo<T>(this List<T> source, IList<T> target)
        => source.ForEach(target.Add);

    /// <summary>
    /// Copies elements to another IList(Of T).
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    /// <param name="source">List to copy from.</param>
    /// <param name="target">List to copy to.</param>
    public static void CopyTo<T>(this T[] source, IList<T> target)
        => Array.ForEach(source, target.Add);

    /// <summary>
    /// Adds a value to a dictionary and returns key selected for value. Keys are automatically
    /// selected for added values.
    /// </summary>
    /// <typeparam name="T">Type of values in dictionary.</typeparam>
    /// <param name="dict">Dictionary object.</param>
    /// <param name="value">Value to add to dictionary.</param>
    /// <returns>Selected key for value.</returns>
    public static int AddWithNewKey<T>(this IDictionary<int, T> dict, T value)
    {
        for (var i = 1; i <= int.MaxValue; i++)
        {
            if (!dict.ContainsKey(i))
            {
                dict.Add(i, value);
                return i;
            }
        }

        throw new IOException("No more keys available.");
    }

    /// <summary>
    /// Adds a value to a dictionary and returns key selected for value. Keys are automatically
    /// selected for added values.
    /// </summary>
    /// <typeparam name="T">Type of values in dictionary.</typeparam>
    /// <param name="dict">Dictionary object.</param>
    /// <param name="value">Value to add to dictionary.</param>
    /// <returns>Selected key for value.</returns>
    public static long AddWithNewKey<T>(this IDictionary<long, T> dict, T value)
    {
        for (var i = 1L; i <= long.MaxValue; i++)
        {
            if (!dict.ContainsKey(i))
            {
                dict.Add(i, value);
                return i;
            }
        }

        throw new IOException("No more keys available.");
    }

    #if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
            /// Returns an IEnumerable(Of T) object that enumerates elements enumerated by all elements in
            /// array.
            /// </summary>
            /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
            /// <param name="e">Array of enumerators</param>
    public static IEnumerable<T> GetFlatEnumerable<T>(this IEnumerable<T>[] e)
    {
        return from coll in e
               from obj in coll
               select obj;
    }

    /// <summary>
    /// Returns an IEnumerable(Of T) object that enumerates elements enumerated by all elements in
    /// array. At each array boundary a special separator object is enumerated.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    /// <param name="Separator">Separator object that will appear in enumeration after each array
    /// element has been enumerated</param>
    public static IEnumerable<T> GetFlatEnumerable<T>(this IEnumerable<T>[] e, T Separator) => e.GetFlatEnumerable(new[] { Separator });

    /// <summary>
    /// Returns an IEnumerable(Of T) object that enumerates elements enumerated by all elements in
    /// array. At each array boundary a special separator object is enumerated.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    /// <param name="Separator">Separator object that will be enumerated in enumeration after each
    /// array element has been enumerated</param>
    public static IEnumerable<T> GetFlatEnumerable<T>(this IEnumerable<T>[] e, IEnumerable<T> Separator)
    {
        return from coll in e
               from obj in coll.Concat(Separator)
               select obj;
    }

    /// <summary>
    /// Returns an new array of objects enumerated by elements in source array.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    public static T[] ToFlatArray<T>(this IEnumerable<T>[] e) => e.GetFlatEnumerable().ToArray();

    /// <summary>
    /// Returns an new array of objects enumerated by elements in source array.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    /// <param name="Separator">Separator object that will appear in resulting array after each
    /// source array element has been enumerated</param>
    public static T[] ToFlatArray<T>(this IEnumerable<T>[] e, T Separator) => e.GetFlatEnumerable(new[] { Separator }).ToArray();

    /// <summary>
    /// Returns an new array of objects enumerated by elements in source array.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    /// <param name="Separator">Separator objects to be placed in target array after each
    /// source array element has been enumerated</param>
    public static T[] ToFlatArray<T>(this IEnumerable<T>[] e, IEnumerable<T> Separator) => e.GetFlatEnumerable(Separator).ToArray();

    /// <summary>
    /// Returns an IEnumerable(Of T) object that enumerates elements enumerated by an enumerator.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    public static IEnumerable<T> GetFlatEnumerable<T>(this IEnumerable<IEnumerable<T>> e)
    {
        return from coll in e
               from obj in coll
               select obj;
    }

    /// <summary>
    /// Returns an IEnumerable(Of T) object that enumerates elements enumerated by an enumerator.
    /// After each enumerator a special separator object is enumerated.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by enumerators.</typeparam>
    /// <param name="e">Enumerator of enumerators</param>
    /// <param name="Separator">Separator object that will be returned in target enumeration after
    /// each source enumeration has been enumerated</param>
    public static IEnumerable<T> GetFlatEnumerable<T>(this IEnumerable<IEnumerable<T>> e, T Separator) => e.GetFlatEnumerable(new[] { Separator });

    /// <summary>
    /// Returns an IEnumerable(Of T) object that enumerates elements enumerated by an enumerator.
    /// After each enumerator a special separator object is enumerated.
    /// </summary>
    /// <typeparam name="T">Type of object enumerated by elements in array.</typeparam>
    /// <param name="e">Array of enumerators</param>
    /// <param name="Separator">Separator object that will be enumerated by target enumeration after
    /// each source enumeration has been enumerated</param>
    public static IEnumerable<T> GetFlatEnumerable<T>(this IEnumerable<IEnumerable<T>> e, IEnumerable<T> Separator)
    {
        return from coll in e
               from obj in coll.Concat(Separator)
               select obj;
    }

    #endif

    public static IEnumerable<string> EnumerateLines(this TextReader tr)
    {

        do
        {

            var line = tr.ReadLine();

            if (line is null)
            {
                yield break;
            }

            yield return line;
        }

        while (true);

    }

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static T Second<T>(this IEnumerable<T> seq) => seq.ElementAt(1);

    public static T? SecondOrDefault<T>(this IEnumerable<T> seq) => seq.ElementAtOrDefault(1);

#endif

    public static void Dispose<T>(this ICollection<T> list) where T : IDisposable
    {
        foreach (var obj in list)
        {
            obj.Dispose();
        }

        if (!list.IsReadOnly)
        {
            list.Clear();
        }
    }

    public static void Dispose<TKey, TValue>(this IDictionary<TKey, TValue> dict) where TValue : IDisposable
    {
        foreach (var obj in dict.Values)
        {
            obj.Dispose();
        }

        if (!dict.IsReadOnly)
        {
            dict.Clear();
        }
    }

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

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static T? MemberwiseMerge<T>(params T[] sequence) where T : class => Reflection.MemberwiseMerger<T>.MergeSequence(sequence);

    public static T? MemberwiseMerge<T>(this IEnumerable<T> sequence) where T : class => Reflection.MemberwiseMerger<T>.MergeSequence(sequence);

#endif

#endif

#nullable enable

    public static IEnumerable<Exception> Enumerate(this Exception? ex)
    {
        while (ex is not null)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            else if (ex is AggregateException aex)
            {
                foreach (var iex in aex.InnerExceptions.SelectMany(Enumerate))
                {
                    yield return iex;
                }

                yield break;
            }
#endif
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
            else if (ex is ReflectionTypeLoadException rtlex)
            {
                yield return ex;

                foreach (var iex in rtlex.LoaderExceptions.SelectMany(Enumerate))
                {
                    yield return iex;
                }

                ex = ex.InnerException;
            }
#endif
            else
            {
                yield return ex;

                ex = ex.InnerException;
            }
        }
    }

    public static IEnumerable<string> EnumerateMessages(this Exception? ex)
    {
        while (ex is not null)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            else if (ex is AggregateException agex)
            {
                foreach (var msg in agex.InnerExceptions.SelectMany(EnumerateMessages))
                {
                    yield return msg;
                }

                yield break;
            }
#endif
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
            else if (ex is ReflectionTypeLoadException tlex)
            {
                yield return ex.Message;

                foreach (var msg in tlex.LoaderExceptions.SelectMany(EnumerateMessages))
                {
                    yield return msg;
                }

                ex = ex.InnerException;
            }
#endif
            else if (ex is Win32Exception win32ex)
            {
                yield return $"{win32ex.Message} ({win32ex.NativeErrorCode})";

                ex = ex.InnerException;
            }
            else
            {
                yield return ex.Message;

                ex = ex.InnerException;
            }
        }
    }

    public static string JoinMessages(this Exception exception) =>
        exception.JoinMessages(Environment.NewLine + Environment.NewLine);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public static string JoinMessages(this Exception exception, char separator) =>
        string.Join(separator, exception.EnumerateMessages());
#else
    public static string JoinMessages(this Exception exception, char separator) =>
        exception.JoinMessages(separator.ToString());
#endif

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static string JoinMessages(this Exception exception, string separator) =>
        string.Join(separator, exception.EnumerateMessages());
#elif NET35_OR_GREATER
    public static string JoinMessages(this Exception exception, string separator) =>
        string.Join(separator, exception.EnumerateMessages().ToArray());
#else
    public static string JoinMessages(this Exception exception, string separator) =>
        string.Join(separator, new List<string>(exception.EnumerateMessages()).ToArray());
#endif

    public static string? FormatLogMessages(this Exception exception) =>
#if DEBUG
        Debugger.IsAttached
        ? exception.JoinMessages()
        : exception?.ToString();
#else
        exception.JoinMessages();
#endif

}