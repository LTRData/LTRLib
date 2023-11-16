// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;

namespace LTRLib.LTRGeneric;

public abstract class DisposableWrapper : IDisposable
{

    public static DisposableWrapper<T> Create<T>(IEnumerable<T> objects) where T : IDisposable => new(objects);

    // IDisposable
    protected abstract void Dispose(bool disposing);

    // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    ~DisposableWrapper()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(false);
    }

    // This code added by Visual Basic to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(true);
        // TODO: uncomment the following line if Finalize() is overridden above.
        GC.SuppressFinalize(this);
    }

}

public sealed class DisposableWrapper<T>(IEnumerable<T> objects) : DisposableWrapper where T : IDisposable
{
    private bool disposedValue; // To detect redundant calls

    // IDisposable
    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                if (objects is not null)
                {
                    foreach (var obj in objects)
                        obj.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.

            // TODO: set large fields to null.
            var collection = objects as ICollection<T>;
            if (collection is not null && !collection.IsReadOnly)
            {

                collection.Clear();
            }

            objects = null!;
        }

        disposedValue = true;
    }

}