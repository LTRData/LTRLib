using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Text;
using LTRLib.Extensions;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

namespace LTRLib.LTRGeneric;

public static class ConsoleSupport
{
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

    public static void WriteMsg(TraceLevel level, string msg)
    {
        var color = default(ConsoleColor);

        switch (level)
        {
            case var @case when @case <= TraceLevel.Off:
                {
                    color = ConsoleColor.Cyan;
                    break;
                }

            case TraceLevel.Error:
                {
                    color = ConsoleColor.Red;
                    break;
                }

            case TraceLevel.Warning:
                {
                    color = ConsoleColor.Yellow;
                    break;
                }

            case TraceLevel.Info:
                {
                    color = ConsoleColor.Gray;
                    break;
                }

            case var case1 when case1 >= TraceLevel.Verbose:
                {
                    color = ConsoleColor.DarkGray;
                    break;
                }
        }

        if (level <= WriteMsgTraceLevel)
        {
            lock (_ConsoleSync)
            {
                Console.ForegroundColor = color;

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

                Console.WriteLine(msg.LineFormat());

#else

                Console.WriteLine(msg)

#endif

                Console.ResetColor();
            }
        }
    }

    private static readonly object _ConsoleSync = new();

}

#endif
