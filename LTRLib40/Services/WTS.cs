#if NET451_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.IO;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using static LTRLib.Services.WTS.UnsafeNativeMethods;

#pragma warning disable IDE0079
#pragma warning disable SYSLIB0004 // Type or member is obsolete
#pragma warning disable SYSLIB0003 // Type or member is obsolete
#pragma warning disable CA1416 // Validate platform compatibility

namespace LTRLib.Services.WTS;

// LTRLib.IO.WTS.ConnectState
public enum ConnectState : int
{
    Active,
    Connected,
    ConnectQuery,
    Shadow,
    Disconnected,
    Idle,
    Listen,
    Reset,
    Down,
    Init
}

// LTRLib.IO.WTS.SessionFlags
public enum SessionFlags : int
{
    Locked = 0,
    Unlocked = 1,
    Unknown = -1
}

internal enum InfoClass : int
{
    WTSInitialProgram,
    WTSApplicationName,
    WTSWorkingDirectory,
    WTSOEMId,
    WTSSessionId,
    WTSUserName,
    WTSWinStationName,
    WTSDomainName,
    WTSConnectState,
    WTSClientBuildNumber,
    WTSClientName,
    WTSClientDirectory,
    WTSClientProductId,
    WTSClientHardwareId,
    WTSClientAddress,
    WTSClientDisplay,
    WTSClientProtocolType,
    WTSIdleTime,
    WTSLogonTime,
    WTSIncomingBytes,
    WTSOutgoingBytes,
    WTSIncomingFrames,
    WTSOutgoingFrames,
    WTSClientInfo,
    WTSSessionInfo,
    WTSSessionInfoEx,
    WTSConfigInfo,
    WTSValidationInfo,   // Info Class value used to fetch Validation Information through the WTSQuerySessionInformation
    WTSSessionAddressV4,
    WTSIsRemoteSession
};

internal delegate bool WTSEnumerateFunc(out SafeWTSBuffer buffer, out int count);

internal delegate bool WTSEnumerateFuncEx(out SafeWTSBufferEx buffer, out int count);

[SupportedOSPlatform("windows")]
internal static class UnsafeNativeMethods
{
    [DllImport("Advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ConvertSidToStringSid([In] byte[] sid, out IntPtr stringSid);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSQuerySessionInformation(this SafeWTSHandle Server, int SessionId, InfoClass WTSInfoClass, out SafeWTSBuffer buffer, out int size);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateSessions(this SafeWTSHandle Server, int Reserved, int Version, out SafeWTSBuffer buffer, out int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateSessionsEx(this SafeWTSHandle Server, ref int Level, int Filter, out SafeWTSBufferEx buffer, out int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateServers([MarshalAs(UnmanagedType.LPWStr)] string DomainName, int Reserved, int Version, out SafeWTSBuffer buffer, out int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateListeners(IntPtr Server, IntPtr pReserved, int Reserved, IntPtr buffer, ref int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateProcesses(this SafeWTSHandle Server, int Reserved, int Version, out SafeWTSBuffer buffer, out int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateProcessesEx(this SafeWTSHandle Server, ref int Level, int SessionId, out SafeWTSBufferEx buffer, out int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSTerminateProcess(this SafeWTSHandle Server, int processId, int exitCode);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSLogoffSession(this SafeWTSHandle Server, int sessionId, [MarshalAs(UnmanagedType.Bool)] bool wait);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSDisconnectSession(this SafeWTSHandle Server, int sessionId, [MarshalAs(UnmanagedType.Bool)] bool wait);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern void WTSFreeMemory(IntPtr buffer);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSFreeMemoryEx(WTSTypeClass typeClass, IntPtr buffer, int count);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSGetChildSessionId(out int ChildSessionId);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSIsChildSessionsEnabled([MarshalAs(UnmanagedType.Bool)] out bool Enabled);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern SafeWTSHandle WTSOpenServer(string serverName);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern void WTSCloseServer(IntPtr buffer);

    [DllImport("WTSAPI32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool WTSQueryListenerConfig(IntPtr hServer, IntPtr pReserved, int Reserved, string listener, [MarshalAs(UnmanagedType.LPStruct), Out] WTSListenerConfig info);
}

[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public class SafeWTSHandle : SafeHandleMinusOneIsInvalid
{
    [SecurityCritical]
    public SafeWTSHandle(IntPtr serverHandle, bool ownsHandle) : base(ownsHandle) => handle = serverHandle;

    [SecurityCritical]
    protected SafeWTSHandle() : base(ownsHandle: true)
    {
    }

    [SecurityCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    protected override bool ReleaseHandle()
    {
        WTSCloseServer(handle);
        return true;
    }
}

[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
internal class SafeWTSBuffer : SafeBuffer
{
    [SecurityCritical]
    protected SafeWTSBuffer() : base(ownsHandle: true)
    {
    }

    [SecurityCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    protected override bool ReleaseHandle()
    {
        WTSFreeMemory(handle);
        return true;
    }
}

internal enum WTSTypeClass
{
    WTSTypeProcessInfoLevel0,
    WTSTypeProcessInfoLevel1,
    WTSTypeSessionInfoLevel1
}

[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
internal class SafeWTSBufferEx : SafeBuffer
{
    protected WTSTypeClass typeClass;
    protected int count;

    [SecurityCritical]
    protected SafeWTSBufferEx() : base(ownsHandle: true)
    {
    }

    [SecurityCritical]
    protected internal void Initialize<T>(WTSTypeClass typeClass, int count) where T : unmanaged
    {
        Initialize<T>((uint)count);
        this.typeClass = typeClass;
        this.count = count;
    }

    [SecurityCritical]
    protected internal void Initialize(WTSTypeClass typeClass, int count, int itemSize)
    {
        Initialize((uint)count, (uint)itemSize);
        this.typeClass = typeClass;
        this.count = count;
    }

    [SecurityCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    protected override bool ReleaseHandle() => WTSFreeMemoryEx(typeClass, handle, count);
}

[SecurityCritical]
public class WTS : IDisposable
{
    private const int _currentSession = -1;

    private const int _allSessions = -2;

    private bool disposedValue;

    public static SafeWTSHandle LocalServerHandle { get; } = new(IntPtr.Zero, ownsHandle: false);

    public static WTS LocalServer { get; } = new();

    public SafeWTSHandle ServerHandle { get; }

    private WTS() => ServerHandle = LocalServerHandle;

    public WTS(string serverName) => ServerHandle = WTSOpenServer(serverName);

    public static WTSIsRemote IsRemoteSession => SessionInfo<WTSIsRemote>.Query(LocalServerHandle, _currentSession);

    public static WTSSessionAddress CurrentSessionAddress => SessionInfo<WTSSessionAddress>.Query(LocalServerHandle, _currentSession);

    public static WTSConfigInfo CurrentConfigInfo => SessionInfo<WTSConfigInfo>.Query(LocalServerHandle, _currentSession);

    public static WTSClient CurrentClient => SessionInfo<WTSClient>.Query(LocalServerHandle, _currentSession);

    public static WTSSessionInfo CurrentSessionInfo => SessionInfo<WTSSessionInfo>.Query(LocalServerHandle, _currentSession);

    public static WTSSessionInfoEx CurrentSessionInfoEx => SessionInfo<WTSSessionInfoEx>.Query(LocalServerHandle, _currentSession);

    public static int ChildSession => WTSGetChildSessionId(out var sessionId) ? sessionId : throw new Win32Exception();

    public static bool IsChildSessionsEnabled => WTSIsChildSessionsEnabled(out var enabled) ? enabled : throw new Win32Exception();

    public IEnumerable<WTSSessionItem> Sessions => EnumerateObjects<WTSSessionItem>.Query((out SafeWTSBuffer buf, out int count) => ServerHandle.WTSEnumerateSessions(0, 1, out buf, out count));

    public IEnumerable<WTSSessionItemEx> SessionsEx => EnumerateObjects<WTSSessionItemEx>.Query((out SafeWTSBufferEx buf, out int count) =>
    {
        var level = 1;
        return ServerHandle.WTSEnumerateSessionsEx(ref level, 0, out buf, out count);
    }, WTSTypeClass.WTSTypeSessionInfoLevel1);

    public static IEnumerable<WTSServerItem> GetServers(string domain) => EnumerateObjects<WTSServerItem>.Query((out SafeWTSBuffer buf, out int count) => WTSEnumerateServers(domain, 0, 1, out buf, out count));

    private static readonly int _sizeOfListener = Marshal.SizeOf<WTSListenerItem>();

    public static unsafe ReadOnlyCollection<WTSListenerItem> Listeners
    {
        get
        {
            var count = 0;
            WTSEnumerateListeners(IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, ref count);

            var buffer = stackalloc byte[count * _sizeOfListener];
            var ptr = new IntPtr(buffer);

            if (!WTSEnumerateListeners(IntPtr.Zero, IntPtr.Zero, 0, ptr, ref count))
            {
                throw new Exception("Listener enumeration failed", new Win32Exception());
            }

            var items = new List<WTSListenerItem>(count);
            for (var i = 0; i < count; i++)
            {
                items.Add(Marshal.PtrToStructure<WTSListenerItem>(ptr + (i * _sizeOfListener))!);
            }

            return items.AsReadOnly();
        }
    }

    public IEnumerable<WTSProcessItem> Processes =>
        EnumerateMethods<WTSProcessItem.NativeWTSProcessItem>.EnumerateValues((out SafeWTSBuffer buf, out int count) => ServerHandle.WTSEnumerateProcesses(0, 1, out buf, out count))
        .Select(item => new WTSProcessItem(item));

    public IEnumerable<WTSProcessItemEx> ProcessesEx => EnumerateSessionProcesses(_allSessions);

    public static IEnumerable<WTSProcessItemEx> CurrentSessionProcesses => LocalServer.EnumerateSessionProcesses(_currentSession);

    public IEnumerable<WTSProcessItemEx> EnumerateSessionProcesses(int sessionId) =>
        EnumerateMethods<WTSProcessItemEx.NativeWTSProcessItemEx>.EnumerateValues((out SafeWTSBufferEx buf, out int count) =>
        {
            var level = 1;
            return ServerHandle.WTSEnumerateProcessesEx(ref level, sessionId, out buf, out count);
        }, WTSTypeClass.WTSTypeProcessInfoLevel1)
        .Select(item => new WTSProcessItemEx(item));

    public T QuerySessionInfo<T>(int sessionId) => SessionInfo<T>.Query(ServerHandle, sessionId);

    public static WTSListenerConfig GetListenerConfig(string listener)
    {
        var info = new WTSListenerConfig();

        if (!WTSQueryListenerConfig(IntPtr.Zero, IntPtr.Zero, 0, listener, info))
        {
            throw new Exception("Query listener configuration failed", new Win32Exception());
        }

        return info;
    }

    public static void LogoffCurrentSession(bool wait)
    {
        if (!LocalServerHandle.WTSLogoffSession(_currentSession, wait))
        {
            throw new Exception("Current session logoff failed", new Win32Exception());
        }
    }

    public static void DisconnectCurrentSession(bool wait)
    {
        if (!LocalServerHandle.WTSDisconnectSession(_currentSession, wait))
        {
            throw new Exception("Current session disconnect failed", new Win32Exception());
        }
    }

    public void TerminateProcess(int processId, int exitCode)
    {
        if (!ServerHandle.WTSTerminateProcess(processId, exitCode))
        {
            throw new Exception("Process termination failed", new Win32Exception());
        }
    }

    public void LogoffSession(int sessionId, bool wait)
    {
        if (!ServerHandle.WTSLogoffSession(sessionId, wait))
        {
            throw new Exception("Session logoff failed", new Win32Exception());
        }
    }

    public void DisconnectSession(int sessionId, bool wait)
    {
        if (!ServerHandle.WTSDisconnectSession(sessionId, wait))
        {
            throw new Exception("Session disconnect failed", new Win32Exception());
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (ServerHandle != LocalServerHandle && !disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                ServerHandle.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer

            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    [SecuritySafeCritical]
    ~WTS()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    [SecuritySafeCritical]
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

internal static class EnumerateMethods<T> where T : unmanaged
{
    internal static IEnumerable<T> EnumerateValues(WTSEnumerateFunc EnumFunc)
    {
        if (!EnumFunc(out var pbuf, out var count))
        {
            throw new Exception($"{EnumFunc.Method.Name} failed", new Win32Exception());
        }

        pbuf.Initialize<T>((uint)count);

        var itemSize = pbuf.ByteLength / (ulong)count;

        using (pbuf)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = pbuf.Read<T>((ulong)i * itemSize);

                yield return obj;
            }
        }
    }

    internal static IEnumerable<T> EnumerateValues(WTSEnumerateFuncEx EnumFunc, WTSTypeClass typeClass)
    {
        if (!EnumFunc(out var pbuf, out var count))
        {
            throw new Exception($"{EnumFunc.Method.Name} failed", new Win32Exception());
        }

        pbuf.Initialize<T>(typeClass, count);

        var itemSize = pbuf.ByteLength / (ulong)count;

        using (pbuf)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = pbuf.Read<T>((ulong)i * itemSize);

                yield return obj;
            }
        }
    }
}

internal static class EnumerateObjects<T>
{
    internal static int ItemSize = Marshal.SizeOf<T>();

    public static IEnumerable<T> Query(WTSEnumerateFunc EnumFunc)
    {
        if (!EnumFunc(out var pbuf, out var count))
        {
            throw new Exception($"{EnumFunc.Method.Name} failed", new Win32Exception());
        }

        using (pbuf)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = Marshal.PtrToStructure<T>(pbuf.DangerousGetHandle() + (i * ItemSize))!;

                yield return obj;
            }
        }
    }

    public static IEnumerable<T> Query(WTSEnumerateFuncEx EnumFunc, WTSTypeClass typeClasss)
    {
        if (!EnumFunc(out var pbuf, out var count))
        {
            throw new Exception($"{EnumFunc.Method.Name} failed", new Win32Exception());
        }

        pbuf.Initialize(typeClasss, count, ItemSize);

        using (pbuf)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = Marshal.PtrToStructure<T>(pbuf.DangerousGetHandle() + (i * ItemSize))!;

                yield return obj;
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public sealed class WTSListenerConfig
{
    public int Version { get; }

    public int EnableListener { get; }

    public int MaxConnectionCount { get; }

    public int PromptForPassword { get; }

    public int InheritColorDepth { get; }

    public int ColorDepth { get; }

    public int InheritBrokenTimeoutSettings { get; }

    public int BrokenTimeoutSettings { get; }

    public int DisablePrinterRedirection { get; }

    public int DisableDriveRedirection { get; }

    public int DisableComPortRedirection { get; }

    public int DisableLPTPortRedirection { get; }

    public int DisableClipboardRedirection { get; }

    public int DisableAudioRedirection { get; }

    public int DisablePNPRedirection { get; }

    public int DisableDefaultMainClientPrinter { get; }

    public int LanAdapter { get; }

    public int PortNumber { get; }

    public int InheritShadowSettings { get; }

    public int ShadowSettings { get; }

    public int TimeoutSettingsConnection { get; }

    public int TimeoutSettingsDisconnection { get; }

    public int TimeoutSettingsIdle { get; }

    public int SecurityLayer { get; }

    public int MinEncryptionLevel { get; }

    public int UserAuthentication { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
    public string? Comment { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string? LogonUserName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
    public string? LogonDomain { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? WorkDirectory { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? InitialProgram { get; }

    internal WTSListenerConfig()
    {
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public sealed class WTSSessionItem
{
    public int SessionId { get; }

    [field: MarshalAs(UnmanagedType.LPWStr)]
    public string? WinStationName { get; }

    public ConnectState State { get; }

    public override string ToString() => $"{SessionId} - {WinStationName} - {State}";

    private WTSSessionItem()
    {
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public sealed class WTSSessionItemEx
{
    public int ExecEnvId { get; }

    public ConnectState State { get; }

    public int SessionId { get; }

    [field: MarshalAs(UnmanagedType.LPWStr)]
    public string? SessionName { get; }

    [field: MarshalAs(UnmanagedType.LPWStr)]
    public string? HostName { get; }

    [field: MarshalAs(UnmanagedType.LPWStr)]
    public string? UserName { get; }

    [field: MarshalAs(UnmanagedType.LPWStr)]
    public string? DomainName { get; }

    [field: MarshalAs(UnmanagedType.LPWStr)]
    public string? FarmName { get; }

    public override string ToString() => $@"{SessionId} - {SessionName} - {DomainName}\{UserName} - {State}";

    private WTSSessionItemEx()
    {
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public sealed class WTSListenerItem
{
    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string? ListenerName { get; }

    public override string? ToString() => ListenerName;

    private WTSListenerItem()
    {
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public sealed class WTSServerItem
{
    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string? ServerName { get; }

    public override string? ToString() => ServerName;

    private WTSServerItem()
    {
    }
}

public sealed class WTSProcessItem
{
    internal readonly struct NativeWTSProcessItem
    {
        public int SessionId { get; }

        public int ProcessId { get; }

        public IntPtr ProcessName { get; }

        public IntPtr Sid { get; }
    }

    public int SessionId { get; }

    public int ProcessId { get; }

    public string? ProcessName { get; }

    public SecurityIdentifier? Sid { get; }

    public NTAccount? Account => Sid?.Translate(typeof(NTAccount)) as NTAccount;

    public override string ToString() => $"{SessionId} - {ProcessId} - {ProcessName} - {Sid}";

    internal WTSProcessItem(in NativeWTSProcessItem native)
    {
        SessionId = native.SessionId;
        ProcessId = native.ProcessId;
        ProcessName = Marshal.PtrToStringUni(native.ProcessName);
        var length = ProcessRoutines.GetNativeSidLength(native.Sid);
        if (length == 0)
        {
            return;
        }

        var bytes = new byte[length];
        Marshal.Copy(native.Sid, bytes, 0, length);
        Sid = new SecurityIdentifier(bytes, 0);
    }
}

public sealed class WTSProcessItemEx
{
    internal readonly struct NativeWTSProcessItemEx
    {
        public int SessionId { get; }
        public int ProcessId { get; }
        public IntPtr ProcessName { get; }
        public IntPtr UserSid { get; }
        public int NumberOfThreads { get; }
        public int HandleCount { get; }
        public int PagefileUsage { get; }
        public int PeakPagefileUsage { get; }
        public int WorkingSetSize { get; }
        public int PeakWorkingSetSize { get; }
        public TimeSpan UserTime { get; }
        public TimeSpan KernelTime { get; }
    }

    public int SessionId { get; }
    public int ProcessId { get; }
    public string? ProcessName { get; }
    public SecurityIdentifier? UserSid { get; }
    public int NumberOfThreads { get; }
    public int HandleCount { get; }
    public int PagefileUsage { get; }
    public int PeakPagefileUsage { get; }
    public int WorkingSetSize { get; }
    public int PeakWorkingSetSize { get; }
    public TimeSpan UserTime { get; }
    public TimeSpan KernelTime { get; }
    public TimeSpan TotalProcessorTime => UserTime + KernelTime;
    public NTAccount? UserAccount => UserSid?.Translate(typeof(NTAccount)) as NTAccount;

    [SecuritySafeCritical]
    public override string ToString() => $"{SessionId} - {ProcessId} - {ProcessName} - {UserSid}";

    internal WTSProcessItemEx(in NativeWTSProcessItemEx native)
    {
        SessionId = native.SessionId;
        ProcessId = native.ProcessId;
        ProcessName = Marshal.PtrToStringUni(native.ProcessName);
        var length = ProcessRoutines.GetNativeSidLength(native.UserSid);
        if (length != 0)
        {
            UserSid = new(native.UserSid);
        }

        NumberOfThreads = native.NumberOfThreads;
        HandleCount = native.HandleCount;
        PagefileUsage = native.PagefileUsage;
        PeakPagefileUsage = native.PeakPagefileUsage;
        WorkingSetSize = native.WorkingSetSize;
        PeakWorkingSetSize = native.PeakWorkingSetSize;
        UserTime = native.UserTime;
        KernelTime = native.KernelTime;
    }
}

// LTRLib.IO.WTS.WTS
[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public static class SessionInfo<T>
{
    internal static InfoClass InfoClass { get; } =
        (typeof(T).GetCustomAttributes(typeof(InfoClassAttribute), inherit: false).FirstOrDefault() as InfoClassAttribute)?.InfoClass ??
        throw new Exception($"Attribute {nameof(InfoClassAttribute)} missing on type {typeof(T).FullName}");

    internal static int DataSize { get; } = Marshal.SizeOf<T>();

    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public static T Query(SafeWTSHandle serverHandle, int sessionId)
    {
        if (!serverHandle.WTSQuerySessionInformation(sessionId, InfoClass, out var pbuf, out var size))
        {
            throw new Exception("Query session information failed", new Win32Exception());
        }

        using (pbuf)
        {
            if (size < DataSize)
            {
                throw new NotSupportedException($"Unexpected size {size} for type {typeof(T).FullName}. Expected: {DataSize}");
            }

            var obj = Marshal.PtrToStructure<T>(pbuf.DangerousGetHandle())!;

            return obj;
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class InfoClassAttribute : Attribute
{
    public InfoClass InfoClass { get; set; }
}

[InfoClass(InfoClass = InfoClass.WTSIsRemoteSession)]
public readonly struct WTSIsRemote
{
    [field: MarshalAs(UnmanagedType.U1)]
    public bool IsRemote { get; }

    public override string ToString() => $"IsRemote: {IsRemote}";
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[InfoClass(InfoClass = InfoClass.WTSSessionAddressV4)]
public sealed class WTSSessionAddress
{
    public AddressFamily AddressFamily { get; }

    [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[]? RawAddress { get; }

    public IPAddress? Address => RawAddress is not null
        ? AddressFamily switch
        {
            AddressFamily.InterNetwork => new IPAddress(RawAddress),
            AddressFamily.InterNetworkV6 => new IPAddress(RawAddress),
            _ => null
        }
        : null;

    private WTSSessionAddress()
    {
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[InfoClass(InfoClass = InfoClass.WTSConfigInfo)]
public sealed class WTSConfigInfo
{
    public int Version { get; }

    public int ConnectClientDrivesAtLogon { get; }

    public int ConnectPrinterAtLogon { get; }

    public int DisablePrinterRedirection { get; }

    public int DisableDefaultMainClientPrinter { get; }

    public int ShadowSettings { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string? LogonUserName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
    public string? LogonDomain { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? WorkDirectory { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? InitialProgram { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? ApplicationName { get; }

    private WTSConfigInfo()
    {
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[InfoClass(InfoClass = InfoClass.WTSClientInfo)]
public sealed class WTSClient
{
    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string? ClientName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
    public string? Domain { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string? UserName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? WorkDirectory { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? InitialProgram { get; }

    public byte EncryptionLevel { get; }

    public AddressFamily ClientAddressFamily { get; }

    [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
    public ushort[]? RawClientAddress { get; }

    public IPAddress? ClientAddress => RawClientAddress is not null
        ? ClientAddressFamily switch
        {
            AddressFamily.InterNetwork => new(RawClientAddress.Take(4).Select(v => (byte)v).ToArray()),
            AddressFamily.InterNetworkV6 => new(RawClientAddress.Take(16).Select(v => (byte)v).ToArray()),
            _ => null
        }
        : null;

    public ushort HRes { get; }

    public ushort VRes { get; }

    public ushort ColorDepth { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? ClientDirectory { get; }

    public int ClientBuildNumber { get; }

    public int ClientHardwareId { get; }

    public ushort ClientProductId { get; }

    public ushort OutBufCountHost { get; }

    public ushort OutBufCountClient { get; }

    public ushort OutBufLength { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
    public string? DeviceId { get; }

    private WTSClient()
    {
    }

    public override string ToString() => $@"{ClientName} - {Domain}\{UserName}";
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[InfoClass(InfoClass = InfoClass.WTSSessionInfo)]
public sealed class WTSSessionInfo
{
    public ConnectState SessionState { get; }

    public int SessionId { get; }

    public int IncomingBytes { get; }

    public int OutgoingBytes { get; }

    public int IncomingFrames { get; }

    public int OutgoingFrames { get; }

    public int IncomingCompressedBytes { get; }

    public int OutgoingCompressedBytes { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string? WinStationName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
    public string? DomainName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string? UserName { get; }

    public long ConnectTime { get; }

    public long DisconnectTime { get; }

    public long LastInputTime { get; }

    public long LogonTime { get; }

    public long CurrentTime { get; }

    private WTSSessionInfo()
    {
    }

    public override string ToString() => $@"{SessionId} - {DomainName}\{UserName} - {SessionState}";
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[InfoClass(InfoClass = InfoClass.WTSSessionInfoEx)]
public sealed class WTSSessionInfoEx
{
    public int Level { get; }

    public int Reserved { get; }

    public int SessionId { get; }

    public ConnectState SessionState { get; }

    public SessionFlags SessionFlags { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string? WinStationName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string? UserName { get; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
    public string? DomainName { get; }

    public long LogonTime { get; }

    public long ConnectTime { get; }

    public long DisconnectTime { get; }

    public long LastInputTime { get; }

    public long CurrentTime { get; }

    public int IncomingBytes { get; }

    public int OutgoingBytes { get; }

    public int IncomingFrames { get; }

    public int OutgoingFrames { get; }

    public int IncomingCompressedBytes { get; }

    public int OutgoingCompressedBytes { get; }

    private WTSSessionInfoEx()
    {
    }

    public override string ToString() => $@"{SessionId} - {DomainName}\{UserName} - {SessionState}";
}

#endif
