// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

/// <summary>
/// A System.Collections.Generic.List(Of T) extended with IDisposable implementation that disposes each
/// object in the list when the list is disposed.
/// </summary>
[ComVisible(false)]
public partial class DisposableList : DisposableList<IDisposable>
{
    public DisposableList() : base()
    {
    }

    public DisposableList(int capacity) : base(capacity)
    {
    }

    public DisposableList(IEnumerable<IDisposable> collection) : base(collection)
    {
    }
}

/// <summary>
/// A System.Collections.Generic.List(Of T) extended with IDisposable implementation that disposes each
/// object in the list when the list is disposed.
/// </summary>
/// <typeparam name="T">Type of elements in list. Type needs to implement IDisposable interface.</typeparam>
[ComVisible(false)]
public partial class DisposableList<T> : List<T>, IDisposable where T : IDisposable
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
                foreach (var obj in this)
                    obj.Dispose();
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

    ~DisposableList()
    {
        Dispose(false);
    }

    public DisposableList() : base()
    {
    }

    public DisposableList(int capacity) : base(capacity)
    {
    }

    public DisposableList(IEnumerable<T> collection) : base(collection)
    {
    }
}
