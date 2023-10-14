using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;
using LTRData.Extensions.Formatting;
using LTRLib.Extensions;
using LTRLib.IO;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

namespace LTRLib.LTRGeneric;

public static class ConsoleSupport
{
    [SupportedOSPlatform("windows")]
    public static void AllocConsole() => Win32API.AllocConsole();

    [SupportedOSPlatform("windows")]
    public static void FreeConsole() => Win32API.FreeConsole();

    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public static void CreateConsoleProgressBar()
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        var row = new StringBuilder(Console.WindowWidth);

        row.Append('[');

        row.Append('.', Math.Max(Console.WindowWidth - 3, 0));

        row.Append("]\r");

        lock (_ConsoleSync)
        {
            Console.ForegroundColor = ConsoleProgressBarColor;

            Console.Write(row.ToString());

            Console.ResetColor();
        }
    }

    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public static void UpdateConsoleProgressBar(double value)
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        if (value > 1d)
        {
            value = 1d;
        }
        else if (value < 0d)
        {
            value = 0d;
        }

        var currentPos = (int)Math.Round((Console.WindowWidth - 3) * value);

        var row = new StringBuilder(Console.WindowWidth);

        row.Append('[');

        row.Append('=', Math.Max(currentPos, 0));

        row.Append('.', Math.Max(Console.WindowWidth - 3 - currentPos, 0));

        var percent = $" {100d * value:0} % ";

        var midpos = Console.WindowWidth - 3 - percent.Length >> 1;

        if (midpos > 0 && row.Length >= percent.Length)
        {
            row.Remove(midpos, percent.Length);

            row.Insert(midpos, percent);
        }

        row.Append("]\r");

        lock (_ConsoleSync)
        {
            Console.ForegroundColor = ConsoleProgressBarColor;

            Console.Write(row.ToString());

            Console.ResetColor();
        }
    }

    public static void FinishConsoleProgressBar()
    {
        UpdateConsoleProgressBar(1d);

        Console.WriteLine();
    }

    public static void UpdateConsoleSpinProgress(ref char chr)
    {
        switch (chr)
        {
            case '\\':
                {
                    chr = '|';
                    break;
                }
            case '|':
                {
                    chr = '/';
                    break;
                }
            case '/':
                {
                    chr = '-';
                    break;
                }

            default:
                {
                    chr = '\\';
                    break;
                }
        }

        lock (_ConsoleSync)
        {
            Console.ForegroundColor = ConsoleProgressBarColor;

            Console.Write(chr);
            Console.Write('\b');

            Console.ResetColor();
        }
    }

    public static ConsoleColor ConsoleProgressBarColor { get; set; } = ConsoleColor.Cyan;

    public static TraceLevel WriteMsgTraceLevel { get; set; } = TraceLevel.Info;

    public static void WriteMsg(TraceLevel level, Func<string>? msgFunc)
    {
        var color = level switch
        {
            <= TraceLevel.Off => ConsoleColor.Cyan,
            TraceLevel.Error => ConsoleColor.Red,
            TraceLevel.Warning => ConsoleColor.Yellow,
            TraceLevel.Info => ConsoleColor.Gray,
            >= TraceLevel.Verbose => ConsoleColor.DarkGray
        };

        if (level > WriteMsgTraceLevel)
        {
            return;
        }

        var msg = msgFunc?.Invoke();

        if (msg is null)
        {
            return;
        }

        lock (_ConsoleSync)
        {
            Console.ForegroundColor = color;

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

            Console.WriteLine(StringFormatting.LineFormat(msg.AsSpan()));

#else

            Console.WriteLine(msg)

#endif

            Console.ResetColor();
        }
    }

    private static readonly object _ConsoleSync = new();

}

#endif
