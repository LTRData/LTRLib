// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Xml;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace LTRLib.LTRGeneric;

[SecuritySafeCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public static class ProcessConstants
{
    public static Process CurrentProcess { get; } = Process.GetCurrentProcess();
    public static Assembly EntryOrExecutingAssembly { get; } = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    public static AssemblyName EntryOrExecutingAssemblyNameObject { get; } = EntryOrExecutingAssembly.GetName();
    public static string? EntryOrExecutingAssemblyName { get; } = EntryOrExecutingAssemblyNameObject?.Name;

    // ' Assembly version
    public static Version? EntryOrExecutingAssemblyVersion { get; } = EntryOrExecutingAssemblyNameObject?.Version;
    public static string EntryOrExecutingAssemblyVersionShortString { get; } = $"{EntryOrExecutingAssemblyVersion?.Major}.{EntryOrExecutingAssemblyVersion?.Minor:000}";

    /// <summary>Dummy object used to synchronize access to process critical shared resources</summary>
    public static object ProcessLevelSyncObject { get; } = new();
}

public static class TextConstants
{
    public static readonly XmlWriterSettings XmlFragmentWriterSettings = new() { Indent = true, IndentChars = "  ", ConformanceLevel = ConformanceLevel.Fragment };

    public static readonly string DashRow = new('-', 40);
    public static readonly string DoubleDashRow = new('=', 40);
    public static readonly string UnderscoreRow = new('_', 40);

    public static readonly string DashHalfRow = new('-', 20);
    public static readonly string DoubleDashHalfRow = new('=', 20);
    public static readonly string UnderscoreHalfRow = new('_', 20);
}

public static class DateTimeConstants
{
    public static readonly long MinFileTime = 1L;

    public static readonly long MaxFileTime = DateTime.MaxValue.ToFileTime();

}