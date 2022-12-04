// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete
#pragma warning disable SYSLIB0004 // Type or member is obsolete
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments

namespace LTRLib.IO;

/// <summary>
/// Provides declaration for some Win32 API functions.
/// </summary>
[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static class Win32API
{
    /// <summary>
    /// Encapsulates call to a Win32 API function that returns a BOOL value indicating success
    /// or failure and where an error value is available through a call to GetLastError() in case
    /// of failure. If value True is passed to this method it does nothing. If False is passed,
    /// it calls GetLastError() and throws a managed exception for that error code.
    /// </summary>
    /// <param name="result">Return code from a Win32 API function call.</param>

    public static void Win32Try(bool result)
    {

        if (result == false)
        {
            throw new Win32Exception();
        }

    }

    /// <summary>
    /// Encapsulates call to a NT API function that returns an NTSTATUS value indicating success
    /// or failure. If zero or positive value is passed to this method it does nothing. If negative
    /// value is passed, it throws a managed exception for that status code.
    /// </summary>
    /// <param name="result">Return code from a NT API function call.</param>

    public static int ThrowOnNtStatusFailed(int result)
    {

        if (result < 0)
        {
            throw new Win32Exception(RtlNtStatusToDosError(result));
        }
        else
        {
            return result;
        }

    }

    /// <summary>
    /// Encapsulates call to a Win32 API function that returns a value where failure
    /// is indicated as a NULL return and GetLastError() returns an error code. If
    /// non-zero value is passed to this method it just returns that value. If zero
    /// value is passed, it calls GetLastError() and throws a managed exception for
    /// that error code.
    /// </summary>
    /// <param name="result">Return code from a Win32 API function call.</param>

    public static T Win32Try<T>(T result)
    {

        if (result is null)
        {
            throw new Win32Exception();
        }

        return result;

    }

    /// <summary>
    /// Encapsulates a handle to a file mapping object.
    /// </summary>
    [SecurityCritical]
    public class SafeFileMappingHandle : SafeKernelObjectHandle
    {

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        public SafeFileMappingHandle(IntPtr open_handle, bool owns_handle) : base(open_handle, owns_handle)
        {

        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        protected SafeFileMappingHandle()
        {
        }

    }

    /// <summary>
    /// Encapsulates a kernel object handle that is closed by calling CloseHandle function in kernel32.dll.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class SafeKernelObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
    {

        /// <summary>
        /// Closes contained handle by calling CloseHandle() Win32 API.
        /// </summary>
        /// <returns>Return value from CloseHandle() Win32 API.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => CloseHandle(handle);

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        [SecurityCritical]
        protected SafeKernelObjectHandle() : base(ownsHandle: true)
        {

        }

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeKernelObjectHandle(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);

        }

    }

    /// <summary>
    /// Represents a pointer to unmanaged memory allocated with LocalAlloc that will be freed with LocalFree
    /// when this object is disposed.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class SafeLocalAllocBuffer
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
        : SafeBuffer
    {
#else
        : SafeHandleZeroOrMinusOneIsInvalid
    {

        public ulong ByteLength { get; private set; }

        private void Initialize(ulong size) => ByteLength = size;

#endif
        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeLocalAllocBuffer(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);
            Initialize((ulong)LocalSize(this).ToInt64());
        }

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="numBytes"></param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeLocalAllocBuffer(IntPtr open_handle, ulong numBytes, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);
            Initialize(numBytes);
        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        [SecurityCritical]
        protected SafeLocalAllocBuffer() : base(ownsHandle: true)
        {

        }

        /// <summary>
        /// Closes contained handle by calling CloseServiceHandle() Win32 API.
        /// </summary>
        /// <returns>Return value from CloseServiceHandle() Win32 API.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => LocalFree(handle) == IntPtr.Zero;
    }

    /// <summary>
    /// Encapsulates a Service Control Management object handle that is closed by calling CloseServiceHandle() Win32 API.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class SafeServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeServiceHandle(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);
        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        [SecurityCritical]
        protected SafeServiceHandle() : base(ownsHandle: true)
        {

        }

        /// <summary>
        /// Closes contained handle by calling CloseServiceHandle() Win32 API.
        /// </summary>
        /// <returns>Return value from CloseServiceHandle() Win32 API.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => CloseServiceHandle(handle);
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeServiceHandle OpenSCManager([MarshalAs(UnmanagedType.LPWStr)][In] string lpMachineName, [MarshalAs(UnmanagedType.LPWStr)][In] string lpDatabaseName, int dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeServiceHandle CreateService(SafeServiceHandle hSCManager, [MarshalAs(UnmanagedType.LPWStr)][In] string lpServiceName, [MarshalAs(UnmanagedType.LPWStr)][In] string lpDisplayName, int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, [MarshalAs(UnmanagedType.LPWStr)][In] string lpBinaryPathName, [MarshalAs(UnmanagedType.LPWStr)][In] string lpLoadOrderGroup, IntPtr lpdwTagId, [MarshalAs(UnmanagedType.LPWStr)][In] string lpDependencies, [MarshalAs(UnmanagedType.LPWStr)][In] string lp, [MarshalAs(UnmanagedType.LPWStr)][In] string lpPassword);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeServiceHandle OpenService(SafeServiceHandle hSCManager, [MarshalAs(UnmanagedType.LPWStr)][In] string lpServiceName, int dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool ControlService(SafeServiceHandle hSCManager, int dwControl, out NativeConstants.SERVICE_STATUS lpServiceStatus);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool DeleteService(SafeServiceHandle hSCObject);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CloseServiceHandle(IntPtr hSCObject);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool StartService(SafeServiceHandle hService, int dwNumServiceArgs, IntPtr lpServiceArgVectors);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool DefineDosDevice(NativeConstants.DEFINE_DOS_DEVICE_FLAGS dwFlags, [MarshalAs(UnmanagedType.LPTStr)][In] string lpDeviceName, [MarshalAs(UnmanagedType.LPTStr)][In] string lpTargetPath);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int QueryDosDevice([MarshalAs(UnmanagedType.LPTStr)][In] string? lpDeviceName, [MarshalAs(UnmanagedType.LPArray), Out] char[] lpTargetPath, uint ucchMax);

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetFileSizeEx")]
    public static extern bool GetFileSize(SafeFileHandle hFile, out long liFileSize);

    /// <summary>
    /// Encapsulates a FindVolume handle that is closed by calling FindVolumeClose() Win32 API.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class SafeFindVolumeHandle : SafeHandleMinusOneIsInvalid
    {

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool FindVolumeClose(IntPtr h);

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeFindVolumeHandle(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);
        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        [SecurityCritical]
        protected SafeFindVolumeHandle() : base(ownsHandle: true)
        {

        }

        /// <summary>
        /// Closes contained handle by calling FindVolumeClose() Win32 API.
        /// </summary>
        /// <returns>Return value from FindVolumeClose() Win32 API.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => FindVolumeClose(handle);
    }

    /// <summary>
    /// Encapsulates a FindVolumeMountPoint handle that is closed by calling FindVolumeMountPointClose () Win32 API.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class SafeFindVolumeMountPointHandle : SafeHandleMinusOneIsInvalid
    {

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool FindVolumeMountPointClose(IntPtr h);

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeFindVolumeMountPointHandle(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);
        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        [SecurityCritical]
        protected SafeFindVolumeMountPointHandle() : base(ownsHandle: true)
        {

        }

        /// <summary>
        /// Closes contained handle by calling FindVolumeMountPointClose() Win32 API.
        /// </summary>
        /// <returns>Return value from FindVolumeMountPointClose() Win32 API.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => FindVolumeMountPointClose(handle);
    }

    /// <summary>
    /// Encapsulates a FindVolumeMountPoint handle that is closed by calling FindVolumeMountPointClose () Win32 API.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public class SafeFindHandle : SafeHandleMinusOneIsInvalid
    {

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool FindClose(IntPtr h);

        /// <summary>
        /// Initiates a new instance with an existing open handle.
        /// </summary>
        /// <param name="open_handle">Existing open handle.</param>
        /// <param name="owns_handle">Indicates whether handle should be closed when this
        /// instance is released.</param>
        [SecurityCritical]
        public SafeFindHandle(IntPtr open_handle, bool owns_handle) : base(owns_handle)
        {

            SetHandle(open_handle);
        }

        /// <summary>
        /// Creates a new empty instance. This constructor is used by native to managed
        /// handle marshaller.
        /// </summary>
        [SecurityCritical]
        protected SafeFindHandle() : base(ownsHandle: true)
        {

        }

        /// <summary>
        /// Closes contained handle by calling FindClose() Win32 API.
        /// </summary>
        /// <returns>Return value from FindClose() Win32 API.</returns>
        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => FindClose(handle);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public readonly struct FindStreamData
    {

        public long StreamSize { get; }

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 296)]
        public string StreamName { get; }

    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern uint GetLogicalDrives();

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFindHandle FindFirstStream([MarshalAs(UnmanagedType.LPTStr)][In] string lpFileName, uint InfoLevel, out FindStreamData lpszVolumeMountPoint, uint dwFlags);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool FindNextStream(SafeFindHandle hFindStream, out FindStreamData lpszVolumeMountPoint);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFindVolumeMountPointHandle FindFirstVolumeMountPoint([MarshalAs(UnmanagedType.LPTStr)][In] string lpszRootPathName, [MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder lpszVolumeMountPoint, int cchBufferLength);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool FindNextVolumeMountPoint(SafeFindVolumeMountPointHandle hFindVolumeMountPoint, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszVolumeMountPoint, int cchBufferLength);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFindVolumeHandle FindFirstVolume([MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszVolumeName, int cchBufferLength);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool FindNextVolume(SafeFindVolumeHandle hFindVolumeMountPoint, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszVolumeName, int cchBufferLength);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetVolumeMountPoint([MarshalAs(UnmanagedType.LPTStr)][In] string lpszVolumeMountPoint, [MarshalAs(UnmanagedType.LPTStr)][In] string lpszVolumeName);

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "DeviceIoControl")]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "DeviceIoControl")]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, in ushort lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "DeviceIoControl")]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, [MarshalAs(UnmanagedType.I1)] in bool lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "DeviceIoControl")]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, out long lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, out NativeConstants.PARTITION_INFORMATION lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, out NativeConstants.PARTITION_INFORMATION_EX lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, out NativeConstants.DISK_GEOMETRY lpOutBuffer, int nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, out NativeConstants.SCSI_ADDRESS lpOutBuffer, int nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, out NativeConstants.STORAGE_DEVICE_NUMBER lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CreateDirectory([MarshalAs(UnmanagedType.LPTStr)][In] string lpszPathName, IntPtr lpSecurityAttributes);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CreateDirectory([MarshalAs(UnmanagedType.LPTStr)][In] string lpszPathName, [MarshalAs(UnmanagedType.LPStruct)][In] NativeConstants.SECURITY_ATTRIBUTES lpSecurityAttributes);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern uint GetVolumePathNamesForVolumeName([MarshalAs(UnmanagedType.LPTStr)][In] string lpszVolumeName, [MarshalAs(UnmanagedType.LPArray), Out] char[] lpszVolumePathNames, uint cchBufferLength, out uint lpcchReturnLength);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern uint GetVolumeNameForVolumeMountPoint([MarshalAs(UnmanagedType.LPTStr)][In] string lpszVolumeName, [MarshalAs(UnmanagedType.LPTStr)][In, Out] StringBuilder DestinationInfFileName, int DestinationInfFileNameSize);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetCommTimeouts(SafeFileHandle hFile, out NativeConstants.COMMTIMEOUTS lpCommTimeouts);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetCommTimeouts(SafeFileHandle hFile, in NativeConstants.COMMTIMEOUTS lpCommTimeouts);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetFilePointerEx(SafeFileHandle hFile, long distance_to_move, out long new_file_pointer, uint move_method);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetFilePointerEx(SafeFileHandle hFile, long distance_to_move, IntPtr ptr_new_file_pointer, uint move_method);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFileHandle CreateFile([In][MarshalAs(UnmanagedType.LPTStr)] string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetFileSizeEx(SafeFileHandle hFile, out long liFileSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] byte[] lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, [MarshalAs(UnmanagedType.LPArray)][In()] byte[] lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, [MarshalAs(UnmanagedType.LPArray)][In()] byte[] lpInBuffer, uint nInBufferSize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] byte[] lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, out NativeConstants.DiskExtents lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr GetModuleHandle([In][MarshalAs(UnmanagedType.LPTStr)] string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetFinalPathNameByHandle(SafeFileHandle hFile, [Out] StringBuilder lpszFilePath, int cchFilePath, ByHandlePathFlags dwFlags);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, [In][MarshalAs(UnmanagedType.LPStr)] string lpEntryName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr ordinal);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool WritePrivateProfileString([In][MarshalAs(UnmanagedType.LPTStr)] string SectionName,
                                                        [In][MarshalAs(UnmanagedType.LPTStr)] string SettingName,
                                                        [In][MarshalAs(UnmanagedType.LPTStr)] string? Value,
                                                        [In][MarshalAs(UnmanagedType.LPTStr)] string? FileName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern string GetCommandLine();

    [DllImport("shell32.dll", SetLastError = true, EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
    public static extern IntPtr CommandLineToArgv([MarshalAs(UnmanagedType.LPWStr)][In] string lpCmdLine, out int pNumArgs);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr LocalFree(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr LocalSize([In] SafeHandle hMem);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetOEMCP();

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetACP();

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)][In] string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPTStr)][In] string lpFileName, IntPtr hFile, uint dwFlags);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern NativeConstants.Win32FileType GetFileType(IntPtr handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern NativeConstants.Win32FileType GetFileType(SafeFileHandle handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(NativeConstants.StdHandle nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out NativeConstants.CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfo(SafeFileHandle hConsoleOutput, out NativeConstants.CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFileMappingHandle CreateFileMapping(IntPtr p1, in NativeConstants.SECURITY_ATTRIBUTES p2, NativeConstants.PageProtection p3, uint p4, uint p5, [MarshalAs(UnmanagedType.LPTStr)][In] string p6);

    // [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFileMappingHandle CreateFileMapping(SafeFileHandle p1, in NativeConstants.SECURITY_ATTRIBUTES p2, NativeConstants.PageProtection p3, uint p4, uint p5, [MarshalAs(UnmanagedType.LPTStr)][In] string p6);

    // [DllImport Lib "kernel32" ("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern SafeFileMappingHandle OpenFileMapping(NativeConstants.MemoryMappedFileRights p1, NativeConstants.HandleInheritability p2, [MarshalAs(UnmanagedType.LPTStr)][In] string p3);

    // [DllImport Lib "kernel32" ("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr MapViewOfFile(SafeFileMappingHandle p1, NativeConstants.MemoryMappedFileRights p2, uint p3, uint p4, UIntPtr p5);

    // [DllImport Lib "kernel32" ("psapi.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("psapi", CharSet = CharSet.Auto)]
    public static extern int GetMappedFileName(IntPtr p1, IntPtr p2, [MarshalAs(UnmanagedType.LPTStr)] string p3, int p4);
    [DllImport("psapi", EntryPoint = "GetMappedFileNameW", CharSet = CharSet.Unicode)]
    public static extern int GetMappedFileName(IntPtr p1, IntPtr p2, [MarshalAs(UnmanagedType.LPArray)] char[] p3, int p4);
    [DllImport("psapi", EntryPoint = "GetMappedFileNameW", CharSet = CharSet.Unicode)]
    public static extern int GetMappedFileName(IntPtr p1, IntPtr p2, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder p3, int p4);

    // [DllImport Lib "kernel32" ("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr GetCurrentProcess();

    // [DllImport Lib "kernel32" ("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern UIntPtr VirtualQuery(IntPtr p1, out NativeConstants.MEMORY_BASIC_INFORMATION p2, UIntPtr p3);

    // [DllImport Lib "kernel32" ("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool FlushViewOfFile(IntPtr p1, uint p2);

    // [DllImport Lib "kernel32" ("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool UnmapViewOfFile(IntPtr p1);

    // [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CloseHandle(IntPtr h);

    [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
    public static extern int RtlNtStatusToDosError(int ntstatus);

    [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
    public static extern bool RtlDosPathNameToNtPathName_U([MarshalAs(UnmanagedType.LPTStr)][In] string DosName, out NativeConstants.UNICODE_STRING NtName, IntPtr DosFilePath, IntPtr NtFilePath);

    [DllImport("ntdll.dll")]
    public static extern void RtlFreeUnicodeString(in NativeConstants.UNICODE_STRING UnicodeString);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int SearchPath([MarshalAs(UnmanagedType.LPTStr)][In] string lpPath, [MarshalAs(UnmanagedType.LPTStr)][In] string lpFileName, [MarshalAs(UnmanagedType.LPTStr)][In] string lpExtension, int nBufferLength, [MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder lpBuffer, out IntPtr lpFilePart);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetDiskFreeSpace([In][MarshalAs(UnmanagedType.LPTStr)] string lpRootPathName, out int lpSectorsPerCluster, out int lpBytesPerSector, out int lpNumberOfFreeClusters, out int lpTotalNumberOfClusters);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom([MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(IntPtr buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out sbyte buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out short buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out int buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out long buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out byte buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out ushort buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out uint buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out ulong buffer, int length);

    [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SystemFunction036", CharSet = CharSet.Auto)]
    public static extern byte RtlGenRandom(out Guid buffer, int length);

    [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
    public static extern int RtlGetVersion(ref NativeConstants.OSVERSIONINFO os_version);

    [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
    public static extern int RtlGetVersion(ref NativeConstants.OSVERSIONINFOEX os_version);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SystemParametersInfoA", CharSet = CharSet.Ansi)]
    public static extern int GetSystemParametersDWORD(uint uAction, uint uParam, out uint lpvParam, uint fuWinIni);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SystemParametersInfoA", CharSet = CharSet.Ansi)]
    public static extern int SetSystemParametersDWORD(uint uAction, uint uParam, IntPtr lpvParam, uint fuWinIni);

    [DllImport("oleaut32.dll", SetLastError = true)]
    public static extern int SysStringByteLen(IntPtr strptr);

    [DllImport("oleaut32.dll", SetLastError = true)]
    public static extern IntPtr SysAllocStringByteLen([In][MarshalAs(UnmanagedType.LPArray)] byte[] psz, int len);

}