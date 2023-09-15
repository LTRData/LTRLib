// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.LTRGeneric;
using System;
using System.Threading;

namespace LTRLib.IO;

public class ConsoleSpinProgress : IDisposable
{

    public Timer Timer { get; }

    private char currentChar;

    public char CurrentChar => currentChar;

    public ConsoleSpinProgress(int dueTime, int period)
    {
        Timer = new Timer(Tick);
        Timer.Change(dueTime, period);
    }

    public ConsoleSpinProgress(TimeSpan dueTime, TimeSpan period)
    {
        Timer = new Timer(Tick);
        Timer.Change(dueTime, period);
    }

    private void Tick(object? o) => ConsoleSupport.UpdateConsoleSpinProgress(ref currentChar);

    private bool disposedValue; // To detect redundant calls

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                Timer.Dispose();
                ConsoleSupport.FinishConsoleProgressBar();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.

            // TODO: set large fields to null.
        }

        disposedValue = true;
    }

    // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    ~ConsoleSpinProgress()
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

#endif
