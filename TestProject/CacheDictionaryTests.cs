#if NET47_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

public class CacheDictionaryTests
{
    [Fact]
    public async Task TimingAsync()
    {
        var dict = new CacheDictionary<string, string>();

        dict.AddOrUpdate("TEST", TimeSpan.FromSeconds(4), Task.FromResult("OBJECT1"));

        var found1 = dict["TEST"];
        Assert.Equal("OBJECT1", await found1);

        var found2 = dict.GetOrAdd("TEST", TimeSpan.FromSeconds(4), _ => Task.FromResult("OBJECT2"));
        Assert.Equal("OBJECT1", await found2);

        Thread.Sleep(TimeSpan.FromSeconds(4.2));

        var found3 = dict.GetOrAdd("TEST", TimeSpan.FromSeconds(4), _ => Task.FromResult("OBJECT3"));
        Assert.Equal("OBJECT3", await found3);
    }

    [Fact]
    public async Task FaultedAsync()
    {
        var dict = new CacheDictionary<string, string>();

        var found1 = dict.GetOrAdd("TEST", TimeSpan.FromSeconds(4), _ => Task.FromException<string>(new Exception("EXCEPTION1")));
        await Assert.ThrowsAnyAsync<Exception>(async () => await found1);

        var found2 = dict.GetOrAdd("TEST", TimeSpan.FromSeconds(4), _ => Task.FromResult("OBJECT2"));
        Assert.Equal("OBJECT2", await found2);
    }

    [Fact]
    public async Task CanceledAsync()
    {
        var dict = new CacheDictionary<string, string>();

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.Cancel();

        var found1 = dict.GetOrAdd("TEST", TimeSpan.FromSeconds(4), _ => Task.FromCanceled<string>(cancellationToken));
        await Assert.ThrowsAnyAsync<TaskCanceledException>(async () => await found1);

        var found2 = dict.GetOrAdd("TEST", TimeSpan.FromSeconds(4), _ => Task.FromResult("OBJECT2"));
        Assert.Equal("OBJECT2", await found2);
    }
}

#endif
