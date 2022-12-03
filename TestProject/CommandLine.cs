﻿using LTRLib.LTRGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

public class CommandLine
{
    [Fact]
    public void Test1()
    {
        var args = new[] {
            "--switch1=arg1",
            "--switch2=arg with spaces2",
            "--switch3=arg3first",
            "--switch3=arg3another",
            "-S",
            "-h",
            "parameter1",
            "parameter with spaces2",
            "parameter2"
        };

        var cmd = StringSupport.ParseCommandLine(args, StringComparer.Ordinal);

        Assert.Equal(6, cmd.Count);
        Assert.Equal(3, cmd[""].Length);
        Assert.Equal(2, cmd["switch3"].Length);
        Assert.Equal("arg with spaces2", cmd["switch2"][0]);
        Assert.Equal("parameter with spaces2", cmd[""][1]);
    }

    [WindowsOnlyFact]
    public void Test2Windows()
    {
        var args = new[] {
            "/switch1=arg1",
            "/switch2=arg with spaces2",
            "--switch3=arg3first",
            "--switch3=arg3another",
            "-S",
            "-h",
            "parameter1",
            "parameter with spaces2",
            "parameter2"
        };

        var cmd = StringSupport.ParseCommandLine(args, StringComparer.Ordinal);

        Assert.Equal(6, cmd.Count);
        Assert.Equal(3, cmd[""].Length);
        Assert.Equal(2, cmd["switch3"].Length);
        Assert.Equal("arg with spaces2", cmd["switch2"][0]);
        Assert.Equal("parameter with spaces2", cmd[""][1]);
    }

    [NotOnWindowsFact]
    public void Test2Unix()
    {
        var args = new[] {
            "/mnt/New folder/New Bitmap Image.bmp"
        };

        var cmd = StringSupport.ParseCommandLine(args, StringComparer.Ordinal);

        Assert.Equal(args[0], cmd[""][0]);
    }

}
