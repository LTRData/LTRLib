using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Threading;
using LTRLib.LTRGeneric;
using System.Runtime.InteropServices;

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

#if NET471_OR_GREATER || NETCOREAPP || NETSTANDARD
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c exit 20";
        }
        else
        {
            process.StartInfo.FileName = "/bin/sh";
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            process.StartInfo.ArgumentList.Add("-c");
            process.StartInfo.ArgumentList.Add("exit 20");
#else
            process.StartInfo.Arguments = "-c 'exit 20'";
#endif
        }
#else
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c exit 20";
#endif

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
