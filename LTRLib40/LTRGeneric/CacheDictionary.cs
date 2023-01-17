// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LTRLib.LTRGeneric;

[ComVisible(false)]
public class CacheDictionary<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, (DateTime ExpiryTime, Task<TValue> Task)> Cache;

    public CacheDictionary()
    {
        Cache = new ConcurrentDictionary<TKey, (DateTime ExpiryTime, Task<TValue> Task)>();
    }

    public CacheDictionary(IEqualityComparer<TKey> Comparer)
    {
        Cache = new ConcurrentDictionary<TKey, (DateTime ExpiryTime, Task<TValue> Task)>(Comparer);
    }

    public void CleanExpired()
    {
        var toremove = new List<KeyValuePair<TKey, (DateTime ExpiryTime, Task<TValue> Task)>>();

        lock (Cache)
        {
            foreach (var Record in Cache)
            {
                if (Record.Value.ExpiryTime < DateTime.UtcNow
                    || Record.Value.Task.IsCanceled
                    || Record.Value.Task.IsFaulted
                    || (Record.Value.Task.IsCompleted && Record.Value.Task.Result is null))
                {
                    toremove.Add(Record);
                }
            }
        }

        foreach (var oldrecord in toremove)
        {
#if NET5_0_OR_GREATER
            Cache.TryRemove(oldrecord);
#else
            Cache.TryRemove(oldrecord.Key, out var argvalue);
#endif

            if (oldrecord.Value.Task.IsCompleted
                && !oldrecord.Value.Task.IsCanceled
                && !oldrecord.Value.Task.IsFaulted
                && oldrecord.Value.Task.Result is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public void Clear() => Cache.Clear();

    public bool TryGetValue(TKey Key, out Task<TValue>? Value)
    {
        CleanExpired();


        if (Cache.TryGetValue(Key, out var Record))
        {
            Value = Record.Task;
            return true;
        }
        else
        {
            Value = null;
            return false;
        }
    }

    public void AddOrUpdate(TKey Key, DateTime ExpiryDateTimeUtc, Task<TValue> Task)
    {
        CleanExpired();

        var oldRecord = Cache.AddOrUpdate(Key, k => (ExpiryDateTimeUtc, Task), (k, old) =>
        {
            if (old.Task.IsCompleted && !old.Task.IsFaulted && (old.Task.Result is IDisposable disposable))
            {
                disposable.Dispose();
            }

            return (ExpiryDateTimeUtc, Task);
        });
    }

    public void AddOrUpdate(TKey Key, double SecondsToLive, Task<TValue> Value) => AddOrUpdate(Key, DateTime.UtcNow.AddSeconds(SecondsToLive), Value);

    public void AddOrUpdate(TKey Key, TimeSpan TimeToLive, Task<TValue> Value) => AddOrUpdate(Key, DateTime.UtcNow + TimeToLive, Value);

    public Task<TValue> GetOrAdd(TKey Key, DateTime ExpiryDateTimeUtc, Func<TKey, Task<TValue>> ValueFactory)
    {
        CleanExpired();

        var item = Cache.GetOrAdd(Key, key => (ExpiryDateTimeUtc, ValueFactory(key)));

        return item.Task;
    }

    public Task<TValue> GetOrAdd(TKey Key, TimeSpan TimeToLive, Func<TKey, Task<TValue>> ValueFactory) => GetOrAdd(Key, DateTime.UtcNow + TimeToLive, ValueFactory);

    public Task<TValue> GetOrAdd(TKey Key, double SecondsToLive, Func<TKey, Task<TValue>> ValueFactory) => GetOrAdd(Key, TimeSpan.FromSeconds(SecondsToLive), ValueFactory);

    public int Count // Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Count
    {
        get
        {
            CleanExpired();
            return Cache.Count;
        }
    }

    public bool ContainsKey(TKey key) // Implements IDictionary(Of TKey, TValue).ContainsKey
    {
        CleanExpired();
        return Cache.ContainsKey(key);
    }

    public Task<TValue> this[TKey key] // Implements IDictionary(Of TKey, TValue).Item
    {
        get
        {
            CleanExpired();
            if (Cache.TryGetValue(key, out var value))
            {
                return value.Task;
            }
            else
            {
                throw new KeyNotFoundException("Key not found in dictionary.");
            }
        }
        set => Cache[key] = (default(DateTime), value);
    }

    public ICollection<TKey> Keys // Implements IDictionary(Of TKey, TValue).Keys
    {
        get
        {
            CleanExpired();
            return Cache.Keys;
        }
    }

    public bool TryRemove(TKey key, out (DateTime ExpiryTime, Task<TValue> Task) value) // Implements IDictionary(Of TKey, TValue).Remove
    {
        CleanExpired();
        return Cache.TryRemove(key, out value);
    }

    public IEnumerable<Task<TValue>> Values // Implements IDictionary(Of TKey, TValue).Values
    {
        get
        {
            CleanExpired();
            return from item in Cache.Values
                   select item.Task;
        }
    }
}

#endif
