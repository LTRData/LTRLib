// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

/// <summary>
/// An extension to Dictionary(Of TKey, TValue) that returns a
/// default item for non-existing keys
/// </summary>
[ComVisible(false)]
public abstract class NullSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{

    private readonly Dictionary<TKey, TValue> m_Dictionary;

    /// <summary>Gets a value that is returned as item for non-existing
    /// keys in dictionary</summary>
    protected abstract TValue GetDefaultValue();

    public object SyncRoot => ((ICollection)m_Dictionary).SyncRoot;

    /// <summary>
    /// Creates a new NullSafeDictionary object
    /// </summary>
    public NullSafeDictionary()
    {
        m_Dictionary = new Dictionary<TKey, TValue>();
    }

    /// <summary>
    /// Creates a new NullSafeDictionary object
    /// </summary>
    public NullSafeDictionary(IEqualityComparer<TKey> Comparer)
    {
        m_Dictionary = new Dictionary<TKey, TValue>(Comparer);
    }

    /// <summary>
    /// Gets or sets the item for a key in dictionary. If no item exists for key, the default
    /// value for this SafeDictionary is returned
    /// </summary>
    /// <param name="key"></param>
    public TValue this[TKey key]
    {
        get
        {
            lock (SyncRoot)
            {
                if (!m_Dictionary.TryGetValue(key, out var ItemRet))
                {
                    var NewItem = GetDefaultValue();
                    m_Dictionary.Add(key, NewItem);
                    return NewItem;
                }

                return ItemRet;
            }
        }
        set => m_Dictionary[key] = value;
    }

    private void ICollection_Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Add(item);

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ICollection_Add(item);

    public void Clear() => m_Dictionary.Clear();

    private bool ICollection_Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Contains(item);

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ICollection_Contains(item);

    private void ICollection_CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).CopyTo(array, arrayIndex);

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ICollection_CopyTo(array, arrayIndex);

    public int Count => m_Dictionary.Count;

    public bool IsReadOnly => false;

    private bool ICollection_Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Remove(item);

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ICollection_Remove(item);

    public void Add(TKey key, TValue value) => m_Dictionary.Add(key, value);

    public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);

    public ICollection<TKey> Keys => m_Dictionary.Keys;

    public bool Remove(TKey key) => m_Dictionary.Remove(key);

    public bool TryGetValue(TKey key, out TValue value) => m_Dictionary.TryGetValue(key, out value!);

    public ICollection<TValue> Values => m_Dictionary.Values;

    private IEnumerator<KeyValuePair<TKey, TValue>> ICollection_GetEnumerator() => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => ICollection_GetEnumerator();

    public IEnumerator GetEnumerator() => m_Dictionary.GetEnumerator();
}