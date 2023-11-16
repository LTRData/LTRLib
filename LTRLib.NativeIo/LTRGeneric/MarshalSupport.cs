// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using static System.Environment;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.Marshal;
using System.Security;
using System.Security.Permissions;
using System.Text;
using LTRData.Extensions.Buffers;
using static LTRLib.IO.Win32API;
using System.Runtime.Versioning;
using LTRData.Extensions.Formatting;

namespace LTRLib.LTRGeneric;

[Obsolete("Use modern cross-platform marshalling routines instead")]
[SecuritySafeCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public static class MarshalSupport
{

    [SecuritySafeCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class AnsiBSTRMarshaller : ICustomMarshaler
    {

        private AnsiBSTRMarshaller()
        {
        }

        private static readonly AnsiBSTRMarshaller Instance = new();

        [SecuritySafeCritical]
        public void CleanUpManagedData(object ManagedObj)
        {

        }

        [SecuritySafeCritical]
        public void CleanUpNativeData(nint pNativeData) => FreeBSTR(pNativeData);

        [SecuritySafeCritical]
        public int GetNativeDataSize() => IntPtr.Size;

        [SecuritySafeCritical]
        [SupportedOSPlatform("windows")]
        public nint MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj is string str)
            {
                var bytes = Encoding.Default.GetBytes(str);
                return SysAllocStringByteLen(bytes, bytes.Length);
            }
            else
            {
                return 0;
            }
        }

        [SecuritySafeCritical]
        [SupportedOSPlatform("windows")]
        public object MarshalNativeToManaged(nint PtrBSTR)
        {
            if (PtrBSTR == 0)
            {
                return string.Empty;
            }

            return PtrToStringAnsi(PtrBSTR, SysStringByteLen(PtrBSTR));
        }

        [SecuritySafeCritical]
        public static ICustomMarshaler GetInstance(string _1) => Instance;
    }

    /// <summary>
    /// Converts an AnsiBSTR in unmanaged memory to a System.String instance. FreeBSTR is
    /// automatically called to free the unmanaged string.
    /// </summary>
    /// <param name="PtrBSTR">Pointer to an AnsiBSTR in unmanaged memory.</param>
    [SupportedOSPlatform("windows")]
    public static string CharsFromAnsiBSTR(nint PtrBSTR)
    {
        if (PtrBSTR == 0)
        {
            return string.Empty;
        }

        var str = PtrToStringAnsi(PtrBSTR, SysStringByteLen(PtrBSTR));

        FreeBSTR(PtrBSTR);

        return str;
    }

    /// <summary>
    /// Adds a semicolon separated list of paths to the PATH environment variable of
    /// current process. Any paths already in present PATH variable are not added again.
    /// </summary>
    /// <param name="AddPaths">Semicolon separated list of directory paths</param>
    /// <param name="BeforeExisting">Indicates whether to insert new paths before existing path list or move
    /// existing of specified paths first if True, or add new paths after existing path list if False.</param>
    public static void AddProcessPaths(bool BeforeExisting, string AddPaths)
    {
        if (string.IsNullOrEmpty(AddPaths))
        {
            return;
        }

        var AddPathsArray = AddPaths.Split(';', StringSplitOptions.RemoveEmptyEntries);

        AddProcessPaths(BeforeExisting, AddPathsArray);
    }

    /// <summary>
    /// Adds a list of paths to the PATH environment variable of current process. Any
    /// paths already in present PATH variable are not added again.
    /// </summary>
    /// <param name="AddPathsArray">Array of directory paths</param>
    /// <param name="BeforeExisting">Indicates whether to insert new paths before existing path list or move
    /// existing of specified paths first if True, or add new paths after existing path list if False.</param>
    public static void AddProcessPaths(bool BeforeExisting, params string[] AddPathsArray)
    {
        if (AddPathsArray is null || AddPathsArray.Length == 0)
        {
            return;
        }

        var Paths = new List<string>(GetEnvironmentVariable("PATH")!.Split(';', StringSplitOptions.RemoveEmptyEntries));

        if (BeforeExisting)
        {
            foreach (var AddPath in AddPathsArray)
            {
                if (Paths.BinarySearch(AddPath, StringComparer.CurrentCultureIgnoreCase) >= 0)
                {
                    Paths.Remove(AddPath);
                }
            }

            Paths.InsertRange(0, AddPathsArray);
        }
        else
        {
            foreach (var AddPath in AddPathsArray)
            {
                if (Paths.BinarySearch(AddPath, StringComparer.CurrentCultureIgnoreCase) < 0)
                {
                    Paths.Add(AddPath);
                }
            }
        }

        SetEnvironmentVariable("PATH", Paths.Join(";"));
    }

    public static Dictionary<string, MethodInfo> DllMethods { get; } = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NETFRAMEWORK

#if NETFRAMEWORK
    private static readonly ModuleBuilder DynModule = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicDllCalls"), AssemblyBuilderAccess.Run).DefineDynamicModule("DynamicDllCalls");
#else
    private static readonly ModuleBuilder DynModule = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicDllCalls"), AssemblyBuilderAccess.Run).DefineDynamicModule("DynamicDllCalls");
#endif

    public static MethodInfo GetDllCallMethod(string dllname, Type return_type, string funcname, Type[] argTypes)
    {
        var type_string = $"{dllname}$${return_type}$${funcname}({string.Join(",", Array.ConvertAll(argTypes, argType => argType.ToString()))})";

        if (!DllMethods.TryGetValue(type_string, out var method))
        {
            var tb = DynModule.DefineType(type_string, TypeAttributes.Abstract | TypeAttributes.UnicodeClass | TypeAttributes.Sealed);

            var mb = tb.DefinePInvokeMethod(funcname, dllname, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl, CallingConventions.Standard, return_type, argTypes, CallingConvention.Winapi, CharSet.Auto);

            mb.SetImplementationFlags(mb.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);

            tb.CreateType();

            method = tb.GetMethod(funcname)
                ?? throw new MissingMethodException("Exported function not found");

            DllMethods.Add(type_string, method);
        }

        return method;
    }

    public static object? DllCall(string dllname, Type return_type, string funcname, Type[] param_types, object[] param_values)
    {
        var method = GetDllCallMethod(dllname, return_type, funcname, param_types);

        param_types = Array.ConvertAll(method.GetParameters(), param =>
        {
            var param_type = param.ParameterType;
            if (param_type.IsByRef)
            {
                return param_type.GetElementType()!;
            }
            else
            {
                return param_type;
            }
        });

        for (int i = 0, loopTo = param_values.Length - 1; i <= loopTo; i++)
        {
            var param_type = param_types[i];

            if (param_values[i] is not null && !param_type.IsAssignableFrom(param_values[i].GetType()))
            {
                if (ReferenceEquals(param_type, typeof(nint)) && param_values[i] is SafeHandle handle)
                {
                    param_values[i] = handle.DangerousGetHandle();
                }
                else if (ReferenceEquals(param_type, typeof(nint)) && param_values[i].GetType().IsPrimitive)
                {
                    param_values[i] = IntPtr.Size < 8 ? (int)param_values[i] : (nint)(long)param_values[i];
                }
                else
                {
                    param_values[i] = Convert.ChangeType(param_values[i], param_type);
                }
            }
        }

        return method.Invoke(null, param_values);
    }
#endif

}