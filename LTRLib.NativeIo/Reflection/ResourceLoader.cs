// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using LTRLib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using static LTRLib.IO.Win32API;

namespace LTRLib.Reflection;

[SecurityCritical]
public class ResourceLoader
{
    public static string SystemArchitecture { get; }
    public static string ProcessArchitecture { get; }

    public static string TemporaryDirectory { get; set; }

    [SecurityCritical]
    static ResourceLoader()
    {

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

        SystemArchitecture = RuntimeInformation.OSArchitecture.ToString();

        ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString();

#else

        SystemArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")
            ?? Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
            ?? "x86";

        ProcessArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
            ?? "x86";

#endif

        SystemArchitecture = SystemArchitecture.ToLowerInvariant();

        Trace.WriteLine($"System architecture is: {SystemArchitecture}");

        ProcessArchitecture = ProcessArchitecture.ToLowerInvariant();

        Trace.WriteLine($"Process architecture is: {ProcessArchitecture}");

        TemporaryDirectory = Path.Combine(Path.GetTempPath(), ProcessArchitecture);

        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
    }

    private static readonly List<ResourceLoader> SearchAssemblies = [];

    private ResourceLoader(Type Type)
    {
        this.Type = Type;
        Assembly = Type.Assembly;
    }

    private ResourceLoader(Assembly Assembly)
    {
        this.Assembly = Assembly;
    }

    public static void AddSearchAssembly(Type Type) => SearchAssemblies.Add(new(Type));

    public static void AddSearchAssembly(Assembly Assembly) => SearchAssemblies.Add(new(Assembly));

    private readonly Type? Type;

    private readonly Assembly Assembly;

    public static Stream? OpenEmbeddedResource(string name)
    {

        foreach (var SearchAssembly in SearchAssemblies)
        {
            var ResourceStream = SearchAssembly.Type is null
                ? SearchAssembly.Assembly.GetManifestResourceStream(name)
                : SearchAssembly.Assembly.GetManifestResourceStream(SearchAssembly.Type, name);
            if (ResourceStream is not null)
            {
                return ResourceStream;
            }
        }

        return null;

    }

    public static byte[]? ReadEmbeddedResource(string name)
    {
        using var ResourceStream = OpenEmbeddedResource(name);
        if (ResourceStream is null)
        {
            return null;
        }
        else if (ResourceStream is MemoryStream memoryStream)
        {
            return memoryStream.ToArray();
        }
        else if (ResourceStream.CanSeek)
        {
            var buffer = new byte[(int)(ResourceStream.Length - 1L) + 1];
            var readlength = ResourceStream.Read(buffer, 0, (int)ResourceStream.Length);
            Array.Resize(ref buffer, readlength);
            return buffer;
        }
        else
        {
            var buffer = ResourceStream.Read((int)ResourceStream.Length);
            return buffer;
        }
    }

    [ThreadStatic]
    private static int AsmLoadLock;

    private static Assembly? AssemblyResolver(object? sender, ResolveEventArgs args)
    {
        AsmLoadLock += 1;
        try
        {
            if (AsmLoadLock > 1)
            {
                return null;
            }

            return LoadResourceAsAssembly($"{args.Name.Split(',')[0]}.dll");
        }
        finally
        {
            AsmLoadLock -= 1;
        }
    }

    public static Assembly? LoadResourceAsAssembly(string resourcename)
    {
        try
        {
            var tempfile = ExtractResourceAsFile($"{ProcessArchitecture}.{resourcename}", resourcename, TemporaryDirectory);
            if (tempfile is not null)
            {
                var asm = Assembly.LoadFile(tempfile);
                Trace.WriteLine($"Loaded assembly {asm.FullName} from file {tempfile}.");
                return asm;
            }

            var bytes = ReadEmbeddedResource(resourcename);
            if (bytes is not null && bytes.Length > 0)
            {
                var asm = Assembly.Load(bytes);
                Trace.WriteLine($"Loaded assembly {asm.FullName} from resource '{resourcename}'.");
                return asm;
            }
            else
            {
                Trace.WriteLine($"Resource not found: '{resourcename}'");
                return null;
            }
        }

        catch (Exception ex)
        {
            Trace.WriteLine($"Unable to load resource '{resourcename}' as assembly: {ex}");
            return null;
        }
    }

    public static string? ExtractResourceAsFile(string resourcename, string name, string tempdir)
    {
        var filename = Path.Combine(tempdir, name);

        try
        {
            var bytes = ReadEmbeddedResource(resourcename);
            if (bytes is not null && bytes.Length > 0)
            {
                File.WriteAllBytes(filename, bytes);
                Trace.WriteLine($"Extracted resource '{resourcename}' as file '{filename}'.");
                return filename;
            }
            else
            {
                Trace.WriteLine($"Unable to extract resource '{resourcename}' as file '{filename}'. Resource not found.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Unable to extract resource '{resourcename}' as file '{filename}': {ex}");
            return null;
        }
    }

    [SecurityCritical, SupportedOSPlatform("windows")]
    public static string? ExtractNativeProcessModule(string name, bool loadModule)
    {
        var resourcename = $"{name}_{ProcessArchitecture}";
        var filename = ExtractResourceAsFile(resourcename, name, TemporaryDirectory);
        if (filename is not null && loadModule)
        {
            if (LoadLibrary(filename) == 0)
            {
                Trace.WriteLine($"Unable to load '{filename}': {new Win32Exception().Message}");
            }
            else
            {
                Trace.WriteLine($"Successfully loaded '{filename}'.");
            }
        }

        return filename;
    }

    [SecurityCritical]
    public static string? ExtractNativeSystemModule(string name)
    {
        var resourcename = $"{name}_{SystemArchitecture}";
        return ExtractResourceAsFile(resourcename, name, TemporaryDirectory);
    }

    [SecurityCritical, SupportedOSPlatform("windows")]
    public static void UnloadNativeModule(string name)
    {

        do
        {
            var hMod = GetModuleHandle(name);
            if (hMod == default)
            {
                break;
            }

            if (!FreeLibrary(hMod))
            {
                throw new Win32Exception();
            }
        }
        while (true);

    }

}