// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.LTRGeneric;
using System;
using System.Security;
using System.Threading;

namespace LTRLib.IO;

[SecuritySafeCritical]
public class ConsoleProgressBar : IDisposable
{
    public Timer Timer { get; }

    public int CurrentValue { get; }

    private readonly Func<double> updateFunc;

    [SecuritySafeCritical]
    private ConsoleProgressBar(Func<double> update)
    {
        Timer = new Timer(Tick);
        updateFunc = update;
        ConsoleSupport.CreateConsoleProgressBar();
    }

    public ConsoleProgressBar(int dueTime, int period, Func<double> update) : this(update)
    {
        Timer.Change(dueTime, period);
    }

    public ConsoleProgressBar(TimeSpan dueTime, TimeSpan period, Func<double> update) : this(update)
    {
        Timer.Change(dueTime, period);
    }

    [SecuritySafeCritical]
    private void Tick(object? _)
    {
        var newvalue = updateFunc();

        if (newvalue != 1d && (int)Math.Round(100d * newvalue) == CurrentValue)
        {
            return;
        }

        ConsoleSupport.UpdateConsoleProgressBar(newvalue);

    }

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
    ~ConsoleProgressBar()
    {
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
