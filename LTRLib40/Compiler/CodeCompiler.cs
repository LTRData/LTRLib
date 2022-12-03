// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

#pragma warning disable SYSLIB0003 // Type or member is obsolete
#pragma warning disable IDE0057 // Use range operator

namespace LTRLib.Compiler;

[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public static class CodeCompiler
{
    public enum CompilerVersion
    {
        v1_0 = 10,
        v1_1 = 11,
        v2_0 = 20,
        v3_0 = 30,
        v3_5 = 35,
        v4_0 = 40
    }

    public enum CompilerLanguage
    {
        CSharp,
        JScript,
        VB,
        Cpp
    }

    private static string[] GetDefaultLibrariesList(CompilerVersion CompilerVersion)
    {
        var References = new List<string>() { "System.dll", "System.Xml.dll", "System.Drawing.dll", "System.Management.dll", "System.Windows.Forms.dll" };
        if (CompilerVersion >= CompilerVersion.v3_5)
        {
            References.Add("System.Core.dll");
            References.Add("System.Xml.Linq.dll");
        }

        return References.ToArray();

    }

    private static readonly Dictionary<CompilerLanguage, Dictionary<CompilerVersion, CodeDomProvider>> CodeProviders = new();

    private static readonly string[] lineBreaks = { "\r\n", "\r", "\n" };

    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public static CompilerResults CompileSourceCodeToMemory(string[] SourceCodes, CompilerLanguage CompilerLanguage, CompilerVersion CompilerVersion, string AdditionalCompilerParameters)
    {
        // ' Compiler command line options
        var Parameters = new CompilerParameters();
        Parameters.ReferencedAssemblies.AddRange(GetDefaultLibrariesList(CompilerVersion));
        Parameters.GenerateExecutable = false;
        Parameters.GenerateInMemory = true;
        Parameters.TreatWarningsAsErrors = true;
        Parameters.IncludeDebugInformation = true;

        // ' Analyze source code for any reference tags
        foreach (var SourceCode in SourceCodes)
        {
            var SourceLines = SourceCode.Split(lineBreaks, StringSplitOptions.RemoveEmptyEntries);
            foreach (var Row in SourceLines)
            {
                if (Row.StartsWith("'Reference ", StringComparison.InvariantCultureIgnoreCase))
                {
                    Parameters.ReferencedAssemblies.Add(Row.Substring("'Reference ".Length));
                }
                else if (Row.StartsWith("//#using ", StringComparison.InvariantCultureIgnoreCase))
                {
                    Parameters.ReferencedAssemblies.Add(Row.Substring("//#using ".Length));
                }
                else
                {
                    break;
                }
            }
        }

        CodeDomProvider? CodeProvider = null;

        lock (CodeProviders)
        {
            if (!CodeProviders.TryGetValue(CompilerLanguage, out var LanguageCodeProviders))
            {
                LanguageCodeProviders = new Dictionary<CompilerVersion, CodeDomProvider>();
                CodeProviders.Add(CompilerLanguage, LanguageCodeProviders);
            }

            if (LanguageCodeProviders.TryGetValue(CompilerVersion, out CodeProvider) == false)
            {
                var CompilerOptions = new Dictionary<string, string>() { { "CompilerVersion", CompilerVersion.ToString().Replace('_', '.') } };
                
                // ' Use specific language code provider
                switch (CompilerLanguage)
                {
                    case CompilerLanguage.CSharp:
                        {
                            CodeProvider = new Microsoft.CSharp.CSharpCodeProvider(CompilerOptions);
                            Parameters.CompilerOptions = "/warnaserror- /optimize+";
                            break;
                        }
                    case CompilerLanguage.VB:
                        {
                            CodeProvider = new Microsoft.VisualBasic.VBCodeProvider(CompilerOptions);
                            Parameters.CompilerOptions = "/warnaserror- /optionexplicit+";
                            if (CompilerVersion >= CompilerVersion.v3_5)
                            {
                                Parameters.CompilerOptions += " /optioninfer+";
                            }

                            break;
                        }

                    default:
                        {
                            CodeProvider = CodeDomProvider.CreateProvider(CompilerLanguage.ToString());
                            break;
                        }
                }

                LanguageCodeProviders.Add(CompilerVersion, CodeProvider);
            }
        }

        if (!string.IsNullOrEmpty(AdditionalCompilerParameters))
        {
            Parameters.CompilerOptions += " " + AdditionalCompilerParameters;
        }

        // ' Compile. Results stored in CompilerResult variable
        return CodeProvider.CompileAssemblyFromSource(Parameters, SourceCodes);

    }

}

