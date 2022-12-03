// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace LTRLib.IO;

/// <summary>
/// Class that simplifies use of command line conversion tools. This class, along with all
/// static and instance members, are completely thread safe.
/// </summary>
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public partial class CommandLineConverter : MarshalByRefObject
{

    /// <summary>
    /// Full path to conversion tool executable.
    /// </summary>
    private readonly string Converter;

    /// <summary>Command line arguments passed to conversion tool. {0} is replaced by source
    /// filename and {1} by target filename.</summary>
    private readonly string ConverterArgs;

    /// <summary>Directory where application should start.</summary>
    private readonly string ConverterStartDir;

    /// <summary>
    /// Initiates a new instance of CommandLineConverter.
    /// </summary>
    /// <param name="Converter">Full path to conversion tool executable.</param>
    /// <param name="ConverterArgs">Command line arguments passed to conversion tool. {0} is replaced by source
    /// filename and {1} by target filename. Example: {0} {1}</param>
    /// <param name="ConverterStartDir">Directory where application should start.</param>
    public CommandLineConverter(string Converter, string ConverterArgs, string ConverterStartDir)
    {
        this.Converter = Converter;
        this.ConverterArgs = ConverterArgs;
        this.ConverterStartDir = ConverterStartDir;
    }

    /// <summary>
    /// Executes conversion tool with source and target filenames inserted in predefined command line.
    /// If tool returns zero, method returns with success. Otherwise an ExternalToolException is thrown.
    /// If command line cannot execute, exception is thrown by Process.Start() method.
    /// </summary>
    /// <param name="sources">Source files path.</param>
    /// <param name="target">Target file path.</param>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public void Execute(string[] sources, string target)
    {
        for (int i = sources.GetLowerBound(0), loopTo = sources.GetUpperBound(0); i <= loopTo; i++)
        {
            if (sources[i].IndexOf('"') < 0)
            {
                sources[i] = $"\"{sources[i]}\"";
            }
        }

        var source = string.Join(" ", sources);

        if (target.IndexOf('"') < 0)
        {
            target = $"\"{target}\"";
        }

        var ProcessStartInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            FileName = Converter,
            Arguments = string.Format(ConverterArgs, source, target),
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = ConverterStartDir
        };

        int ExitCode;
        var StdOut = "";
        var StdErr = "";

        using var ps = Process.Start(ProcessStartInfo)
            ?? throw new FileNotFoundException("Failed to start application");

        var stdOutReader = () => StdOut = ps.StandardOutput.ReadToEnd();
        var stdOutRead = stdOutReader.BeginInvoke(null, null);
        var stdErrReader = () => StdErr = ps.StandardError.ReadToEnd();
        var stdErrRead = stdErrReader.BeginInvoke(null, null);
        ps.WaitForExit();
        ExitCode = ps.ExitCode;
        stdErrReader.EndInvoke(stdErrRead);
        stdOutReader.EndInvoke(stdOutRead);

        if (ExitCode != 0)
        {
            throw new ExternalToolException(ExitCode, ProcessStartInfo.FileName, ProcessStartInfo.Arguments, StdOut, StdErr);
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// Executes conversion tool with source and target filenames inserted in predefined command line.
    /// If tool returns zero, method returns with success. Otherwise an ExternalToolException is thrown.
    /// If command line cannot execute, exception is thrown by Process.Start() method.
    /// </summary>
    /// <param name="sources">Source files path.</param>
    /// <param name="target">Target file path.</param>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public async Task ExecuteAsync(string[] sources, string target)
    {
        for (int i = sources.GetLowerBound(0), loopTo = sources.GetUpperBound(0); i <= loopTo; i++)
        {
            if (!sources[i].Contains('"'))
            {
                sources[i] = $"\"{sources[i]}\"";
            }
        }

        var source = string.Join(" ", sources);
        if (!target.Contains('"'))
        {
            target = $"\"{target}\"";
        }

        var ProcessStartInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            FileName = Converter,
            Arguments = string.Format(ConverterArgs, source, target),
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = ConverterStartDir
        };

        int ExitCode;
        var StdOut = "";
        var StdErr = "";
        
        using var ps = Process.Start(ProcessStartInfo)
            ?? throw new FileNotFoundException("Failed to start application");

        var stdOutReader = () => StdOut = ps.StandardOutput.ReadToEnd();
        var stdOutRead = stdOutReader.BeginInvoke(null, null);
        var stdErrReader = () => StdErr = ps.StandardError.ReadToEnd();
        var stdErrRead = stdErrReader.BeginInvoke(null, null);
        ExitCode = await ps;
        stdErrReader.EndInvoke(stdErrRead);
        stdOutReader.EndInvoke(stdOutRead);

        if (ExitCode != 0)
        {
            throw new ExternalToolException(ExitCode, ProcessStartInfo.FileName, ProcessStartInfo.Arguments, StdOut, StdErr);
        }
    }
#endif

    /// <summary>
    /// Exception thrown when non-zero exit code is returned from external tool.
    /// </summary>
    [Serializable]
    public partial class ExternalToolException : Exception
    {

        /// <summary>
        /// Command that was executed.
        /// </summary>
        public readonly string? Command;

        /// <summary>
        /// Arguments passed to command.
        /// </summary>
        public readonly string? CommandArgs;

        /// <summary>
        /// Exit code returned by command.
        /// </summary>
        public readonly int ExitCode;

        /// <summary>
        /// Standard output data from command.
        /// </summary>
        public readonly string? StdOut;

        /// <summary>
        /// Standard error data from command.
        /// </summary>
        public readonly string? StdErr;

        /// <summary>
        /// Extended error message containing complete command line, exit code and contents of
        /// standard output and standard error.
        /// </summary>
        public readonly string? LongMessage;

        private string GetLongMessage()
        {
            return $@"
Command line: {Command} {CommandArgs}

Exit code: {ExitCode}

Result:
{StdOut}{StdErr}";
        }

        public ExternalToolException(int ExitCode, string Command, string CommandArgs, string StdOut, string StdErr) : base(string.IsNullOrEmpty(StdErr) ? StdOut : StdErr)
        {
            this.Command = Command;
            this.CommandArgs = CommandArgs;
            this.ExitCode = ExitCode;
            this.StdOut = StdOut;
            this.StdErr = StdErr;
            LongMessage = GetLongMessage();
        }

        public ExternalToolException(int ExitCode, string Command, string CommandArgs, string StdOut, string StdErr, Exception InnerException) : base(string.IsNullOrEmpty(StdErr) ? StdOut : StdErr, InnerException)
        {
            this.Command = Command;
            this.CommandArgs = CommandArgs;
            this.ExitCode = ExitCode;
            this.StdOut = StdOut;
            this.StdErr = StdErr;
            LongMessage = GetLongMessage();
        }

        // <SecurityPermission(SecurityAction.Demand, flags:=SecurityPermissionFlag.SerializationFormatter)>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);

        protected ExternalToolException(SerializationInfo si, StreamingContext context) : base(si, context)
        {
        }
    }
}

#endif
