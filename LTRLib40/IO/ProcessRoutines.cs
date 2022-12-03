/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Principal;

namespace LTRLib.IO;

[SecurityCritical]
public static class ProcessRoutines
{
    [Flags]
    public enum TokenAccess
    {
        AssignPrimary = 0x0001,
        Duplicate = 0x0002,
        Impersonate = 0x0004,
        Query = 0x0008,
        QuerySource = 0x0010,
        AdjustPrivileges = 0x0020,
        AdjustGroups = 0x0040,
        AdjustDefault = 0x0080,
        AdjustSessionid = 0x0100
    }

    public enum TokenInformationClass
    {
        TokenUser,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUIAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        TokenIsAppContainer,
        TokenCapabilities,
        TokenAppContainerSid,
        TokenAppContainerNumber,
        TokenUserClaimAttributes,
        TokenDeviceClaimAttributes,
        TokenRestrictedUserClaimAttributes,
        TokenRestrictedDeviceClaimAttributes,
        TokenDeviceGroups,
        TokenRestrictedDeviceGroups,
        TokenSecurityAttributes,
        TokenIsRestricted,
        TokenProcessTrustLevel,
        TokenPrivateNameSpace,
        TokenSingletonAttributes,
        TokenBnoIsolation,
        TokenChildProcessFlags,
        TokenIsLessPrivilegedAppContainer,
        TokenIsSandboxed,
        MaxTokenInfoClass
    }

    public enum ProcessInfoClass
    {
        ProcessBasicInformation,
        ProcessQuotaLimits,     // QUOTA_LIMITS 
        ProcessIoCounters,      // IOCOUNTERS 
        ProcessVmCounters,      // VM_COUNTERS 
        ProcessTimes,       // KERNEL_USER_TIMES 
        ProcessBasePriority,    // BASE_PRIORITY_INFORMATION 
        ProcessRaisePriority,
        ProcessDebugPort,
        ProcessExceptionPort,
        ProcessAccessToken,
        ProcessLdtInformation,
        ProcessLdtSize,
        ProcessDefaultHardErrorMode,
        ProcessIoPortHandlers,  // Note: this is kernel mode only 
        ProcessPooledUsageAndLimits,
        ProcessWorkingSetWatch,
        ProcessUserModeIOPL,
        ProcessEnableAlignmentFaultFixup,
        ProcessPriorityClass,
        ProcessWx86Information,
        ProcessHandleCount,
        ProcessAffinityMask,    // AFFINITY_MASK 
        ProcessPriorityBoost,
        ProcessDeviceMap,
        ProcessSessionInformation,
        ProcessForegroundInformation,
        ProcessWow64Information,
        MaxProcessInfoClass
    }

    public readonly struct ProcessBasicInformation
    {
        public IntPtr Reserved1 { get; }
        public IntPtr PebBaseAddress { get; }
        public IntPtr Reserved2 { get; }
        public IntPtr Reserved3 { get; }
        public IntPtr UniqueProcessId { get; }
        public IntPtr ParentProcessId { get; }
    }

    [SupportedOSPlatform("windows")]
    private static class UnsafeNativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetLengthSid([In] byte[] pNativeData);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsValidSid([In] byte[] pNativeData);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetLengthSid(IntPtr pNativeData);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsValidSid(IntPtr pNativeData);

#if NET46_OR_GREATER || NETCOREAPP

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern int NtSuspendProcess(SafeProcessHandle hProcess);

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern int NtResumeProcess(SafeProcessHandle hProcess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(SafeProcessHandle hProcess, TokenAccess TokeAccess, out SafeAccessTokenHandle token);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(SafeAccessTokenHandle token, TokenInformationClass InformationClass, IntPtr Information, int InformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern int NtQueryInformationProcess(SafeProcessHandle hProcess, ProcessInfoClass ProcessInformationClass, IntPtr pProcessInformation, int uProcessInformationLength, out int puReturnLength);
#endif
    }

#if NET46_OR_GREATER || NETCOREAPP

    [SupportedOSPlatform("windows")]
    public static unsafe ProcessBasicInformation QueryBasicInformation(this Process process)
    {
        ProcessBasicInformation info;
        Win32API.ThrowOnNtStatusFailed(UnsafeNativeMethods.NtQueryInformationProcess(process.SafeHandle, ProcessInfoClass.ProcessBasicInformation, new IntPtr(&info), sizeof(ProcessBasicInformation), out _));
        return info;
    }

    [SupportedOSPlatform("windows")]
    public static SafeAccessTokenHandle OpenProcessToken(this Process process, TokenAccess access) =>
        UnsafeNativeMethods.OpenProcessToken(process.SafeHandle, access, out var token) ? token : throw new Win32Exception();

    [SupportedOSPlatform("windows")]
    public static SecurityIdentifier? GetSecurityIdentifier(this Process process)
    {
        using var token = process.OpenProcessToken(TokenAccess.Query);
        using var identity = new WindowsIdentity(token.DangerousGetHandle());
        return identity.User;
    }

    [SupportedOSPlatform("windows")]
    public static void Suspend(this Process process) =>
        Win32API.ThrowOnNtStatusFailed(UnsafeNativeMethods.NtSuspendProcess(process.SafeHandle));

    [SupportedOSPlatform("windows")]
    public static void Resume(this Process process) =>
        Win32API.ThrowOnNtStatusFailed(UnsafeNativeMethods.NtResumeProcess(process.SafeHandle));
#endif

    [SupportedOSPlatform("windows")]
    public static int GetNativeSidLength(IntPtr psid) =>
        UnsafeNativeMethods.IsValidSid(psid) ? UnsafeNativeMethods.GetLengthSid(psid) : 0;

    [SupportedOSPlatform("windows")]
    public static int GetNativeSidLength(byte[] sid)
    {
        if (sid is null || sid.Length < 8 || !UnsafeNativeMethods.IsValidSid(sid))
        {
            return 0;
        }

        var length = UnsafeNativeMethods.GetLengthSid(sid);

        if (length > sid.Length)
        {
            return 0;
        }

        return length;
    }
}
