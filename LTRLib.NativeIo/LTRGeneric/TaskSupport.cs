﻿/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
using System.IO;
using System.Linq;
#endif

namespace LTRLib.LTRGeneric;

public static class TaskSupport
{
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static WaitHandleAwaiter WithTimeout(this WaitHandle handle, TimeSpan timeout) =>
        new(handle, timeout);

    public static WaitHandleAwaiter WithTimeout(this WaitHandle handle, int msecTimeout) =>
        new(handle, TimeSpan.FromMilliseconds(msecTimeout));

    public static WaitHandleAwaiter GetAwaiter(this WaitHandle handle) =>
        new(handle, new(0, 0, 0, 0, -1));

    public static ProcessAwaiter GetAwaiter(this Process process) =>
        new(process);
#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static WaitHandle CreateWaitHandle(this Process process, bool inheritable) =>
        NativeWaitHandle.DuplicateExisting(process.Handle, inheritable);
#endif

    private sealed class NativeWaitHandle : WaitHandle
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
                                                   IntPtr hSourceHandle,
                                                   IntPtr hTargetProcessHandle,
                                                   out SafeWaitHandle lpTargetHandle,
                                                   uint dwDesiredAccess,
                                                   bool bInheritHandle,
                                                   uint dwOptions);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        public static NativeWaitHandle DuplicateExisting(IntPtr handle, bool inheritable)
        {
            if (!DuplicateHandle(GetCurrentProcess(), handle, GetCurrentProcess(), out var new_handle, 0, inheritable, 0x2))
            {
                throw new Win32Exception();
            }

            return new(new_handle);
        }

        public NativeWaitHandle(SafeWaitHandle handle)
        {
            SafeWaitHandle = handle;
        }
    }
}

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

public readonly struct ProcessAwaiter : ICriticalNotifyCompletion
{
    public Process? Process { get; }

    public ProcessAwaiter(Process? process)
    {
        try
        {
            if (process is null || process.Handle == IntPtr.Zero)
            {
                Process = null;
                return;
            }

            if (!process.EnableRaisingEvents)
            {
                throw new NotSupportedException("Events not available for this Process object.");
            }
        }
        catch (Exception ex)
        {
            throw new NotSupportedException("ProcessAwaiter requires a local, running Process object with EnableRaisingEvents property set to true when Process object was created.", ex);
        }

        Process = process;
    }

    public ProcessAwaiter GetAwaiter() => this;

    public bool IsCompleted => Process?.HasExited ?? true;

    public int GetResult() => Process?.ExitCode ?? 0;

    public void OnCompleted(Action continuation) => throw new NotSupportedException();

    public void UnsafeOnCompleted(Action continuation)
    {
        if (Process is null)
        {
            throw new InvalidOperationException();
        }

        var completion_counter = 0;

        Process.Exited += (sender, e) =>
        {
            if (Interlocked.Exchange(ref completion_counter, 1) == 0)
            {
                continuation();
            }
        };

        if (Process.HasExited && Interlocked.Exchange(ref completion_counter, 1) == 0)
        {
            continuation();
        }
    }
}

public sealed class WaitHandleAwaiter : ICriticalNotifyCompletion
{
    private readonly WaitHandle handle;
    private readonly TimeSpan timeout;
    private RegisteredWaitHandle? callbackHandle;
    private Action? continuation;
    private bool result = true;

    public WaitHandleAwaiter(WaitHandle handle, TimeSpan timeout)
    {
        this.handle = handle;
        this.timeout = timeout;
    }

    public WaitHandleAwaiter GetAwaiter() => this;

    public bool IsCompleted => handle.WaitOne(0);

    public bool GetResult() => result;

    public void OnCompleted(Action continuation) => throw new NotSupportedException();

    public void UnsafeOnCompleted(Action continuation)
    {
        this.continuation = continuation;

        callbackHandle = ThreadPool.RegisterWaitForSingleObject(
            waitObject: handle,
            callBack: WaitProc,
            state: this,
            timeout: timeout,
            executeOnlyOnce: true);
    }

    private static void WaitProc(object? state, bool timedOut)
    {
        var obj = state as WaitHandleAwaiter
            ?? throw new InvalidAsynchronousStateException();

        obj.result = !timedOut;

        while (obj.callbackHandle is null)
        {
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            Thread.Yield();
#else
            Thread.Sleep(0);
#endif
        }

        obj.callbackHandle.Unregister(null);

        obj.continuation?.Invoke();
    }
}

#endif

