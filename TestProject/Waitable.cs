﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LTRLib.Extensions;
using System.Threading;
using LTRLib.LTRGeneric;

namespace LTRLib;

public class Waitable
{
    [Fact]
    public async Task RunProcess()
    {
        using var process = new Process
        {
            EnableRaisingEvents = true
        };

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c exit 20";

        process.Start();

        var result = await process;

        Assert.Equal(20, result);
    }

    [Fact]
    public async Task Event()
    {
        using var evt = new ManualResetEvent(initialState: false);

        var result = await evt.WithTimeout(1000);

        Assert.False(result);

        evt.Set();

        result = await evt;

        Assert.True(result);

        evt.Reset();

        ThreadPool.QueueUserWorkItem(_ => { evt.Set(); });

        result = await evt;
    }

    [Fact]
    public async Task EventSlim()
    {
        using var evt = new ManualResetEventSlim();

        var result = await evt.WaitHandle.WithTimeout(1000);

        Assert.False(result);

        evt.Set();

        result = await evt.WaitHandle;

        Assert.True(result);

        evt.Reset();

        ThreadPool.QueueUserWorkItem(_ => { evt.Set(); });

        result = await evt.WaitHandle;
    }
}
