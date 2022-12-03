// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace LTRLib.LTRGeneric;

/// <summary>
/// A System.Collections.Generic.Dictionary(Of TKey, TValue) extended with IDisposable implementation that disposes each
/// value object in the dictionary when the dictionary is disposed.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[ComVisible(false)]
[SecuritySafeCritical]
[Serializable]

public partial class DisposableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable
    where TKey : notnull
    where TValue : IDisposable
{
    private bool disposedValue;    // To detect redundant calls

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: free managed resources when explicitly called
                foreach (var value in Values)
                    value.Dispose();
            }
        }
        disposedValue = true;

        // TODO: free shared unmanaged resources
        Clear();
    }

    // This code added by Visual Basic to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DisposableDictionary()
    {
        Dispose(false);
    }

    public DisposableDictionary() : base()
    {
    }

    public DisposableDictionary(int capacity) : base(capacity)
    {
    }

    public DisposableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
    {
    }

    public DisposableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
    {
    }

    public DisposableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
    {
    }

    public DisposableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
    {
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);

    protected DisposableDictionary(SerializationInfo si, StreamingContext context) : base(si, context)
    {
    }
}

