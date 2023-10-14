// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using static System.Runtime.InteropServices.Marshal;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;
using LTRLib.Extensions;
using static LTRLib.IO.NativeConstants;
using System.Diagnostics;
using static LTRLib.IO.Win32API;
using System.Runtime.Versioning;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Formatting;
using LTRData.Extensions.Buffers;
using System.Linq;
#endif


#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1069 // Enums values should not be duplicated
#pragma warning disable SYSLIB0003 // Type or member is obsolete
#pragma warning disable IDE0057 // Use range operator

namespace LTRLib.IO;

/// <summary>
/// Provides wrappers for Win32 file API. This makes it possible to open anything that
/// CreateFile() can open and get a FileStream based .NET wrapper around the file handle.
/// </summary>
[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
[SupportedOSPlatform("windows")]
public static class NativeFileIO
{
    public static bool TestFileOpen(string path)
    {

        using var handle = CreateFile(path, FILE_READ_ATTRIBUTES, 0U, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        return !handle.IsInvalid;

    }

    public static long GetVolumeSize(SafeFileHandle volume)
    {
        if (!DeviceIoControl(volume, IOCTL_DISK_GET_LENGTH_INFO, IntPtr.Zero, 0U, out long length, 8U, out _, IntPtr.Zero))
        {
            throw new Win32Exception();
        }

        return length;
    }

    public static uint ConvertManagedFileAccess(FileAccess DesiredAccess)
    {

        var NativeDesiredAccess = FILE_READ_ATTRIBUTES;
        if ((DesiredAccess & FileAccess.Read) == FileAccess.Read)
        {
            NativeDesiredAccess |= GENERIC_READ;
        }

        if ((DesiredAccess & FileAccess.Write) == FileAccess.Write)
        {
            NativeDesiredAccess |= GENERIC_WRITE;
        }

        return NativeDesiredAccess;

    }

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle in a SafeFileHandle object.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="Overlapped">Specifies whether to request overlapped I/O.</param>
    [SecurityCritical]
    public static SafeFileHandle OpenFileHandle(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, bool Overlapped) => OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, Overlapped ? FileOptions.Asynchronous : default);

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle in a SafeFileHandle object.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="Options">Specifies whether to request overlapped I/O.</param>
    [SecurityCritical]
    public static SafeFileHandle OpenFileHandle(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, FileOptions Options)
    {

        if (string.IsNullOrEmpty(FileName))
        {
            throw new ArgumentNullException(nameof(FileName));
        }

        var NativeDesiredAccess = ConvertManagedFileAccess(DesiredAccess);

        var NativeShareMode = 0U;
        if ((ShareMode & FileShare.Read) == FileShare.Read)
        {
            NativeShareMode |= FILE_SHARE_READ;
        }

        if ((ShareMode & FileShare.Write) == FileShare.Write)
        {
            NativeShareMode |= FILE_SHARE_WRITE;
        }

        if ((ShareMode & FileShare.Delete) == FileShare.Delete)
        {
            NativeShareMode |= FILE_SHARE_DELETE;
        }

        uint NativeCreationDisposition;
        switch (CreationDisposition)
        {
            case FileMode.Create:
                {
                    NativeCreationDisposition = CREATE_ALWAYS;
                    break;
                }
            case FileMode.CreateNew:
                {
                    NativeCreationDisposition = CREATE_NEW;
                    break;
                }
            case FileMode.Open:
                {
                    NativeCreationDisposition = OPEN_EXISTING;
                    break;
                }
            case FileMode.OpenOrCreate:
                {
                    NativeCreationDisposition = OPEN_ALWAYS;
                    break;
                }
            case FileMode.Truncate:
                {
                    NativeCreationDisposition = TRUNCATE_EXISTING;
                    break;
                }

            default:
                {
                    throw new NotImplementedException();
                }
        }

        var NativeFlagsAndAttributes = (int)FILE_ATTRIBUTE_NORMAL;

        NativeFlagsAndAttributes += (int)Options;

        var Handle = CreateFile(FileName, NativeDesiredAccess, NativeShareMode, IntPtr.Zero, NativeCreationDisposition, NativeFlagsAndAttributes, IntPtr.Zero);
        if (Handle.IsInvalid)
        {
            throw new Win32Exception();
        }

        return Handle;
    }

    /// <summary>
    /// Converts FileAccess flags to values legal in constructor call to FileStream class.
    /// </summary>
    /// <param name="Value">FileAccess values.</param>
    internal static FileAccess GetFileStreamLegalAccessValue(FileAccess Value)
    {
        if (Value == 0)
        {
            return FileAccess.Read;
        }
        else
        {
            return Value;
        }
    }

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    public static FileStream OpenFileStream(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode) => new(OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, 0), GetFileStreamLegalAccessValue(DesiredAccess));

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="BufferSize">Buffer size to specify in constructor call to FileStream class.</param>
    public static FileStream OpenFileStream(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, int BufferSize) => new(OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, 0), GetFileStreamLegalAccessValue(DesiredAccess), BufferSize);

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="BufferSize">Buffer size to specify in constructor call to FileStream class.</param>
    /// <param name="Overlapped">Specifies whether to request overlapped I/O.</param>
    public static FileStream OpenFileStream(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, int BufferSize, bool Overlapped) => new(OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, Overlapped), GetFileStreamLegalAccessValue(DesiredAccess), BufferSize, Overlapped);

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="BufferSize">Buffer size to specify in constructor call to FileStream class.</param>
    /// <param name="Options">Specifies whether to request overlapped I/O.</param>
    public static FileStream OpenFileStream(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, int BufferSize, FileOptions Options) => new(OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, Options), GetFileStreamLegalAccessValue(DesiredAccess), BufferSize, (Options & FileOptions.Asynchronous) == FileOptions.Asynchronous);

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="Options">Specifies whether to request overlapped I/O.</param>
    public static FileStream OpenFileStream(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, FileOptions Options) =>
        new(OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, Options), GetFileStreamLegalAccessValue(DesiredAccess), 1, (Options & FileOptions.Asynchronous) == FileOptions.Asynchronous);

    /// <summary>
    /// Calls Win32 API CreateFile() function and encapsulates returned handle in a
    /// managed FileStream object with buffer size set to 1 byte.
    /// </summary>
    /// <param name="FileName">Name of file to open.</param>
    /// <param name="DesiredAccess">File access to request.</param>
    /// <param name="ShareMode">Share mode to request.</param>
    /// <param name="CreationDisposition">Open/creation mode.</param>
    /// <param name="Overlapped">Specifies whether to request overlapped I/O.</param>
    public static FileStream OpenFileStream(string FileName, FileMode CreationDisposition, FileAccess DesiredAccess, FileShare ShareMode, bool Overlapped) =>
        new(OpenFileHandle(FileName, CreationDisposition, DesiredAccess, ShareMode, Overlapped), GetFileStreamLegalAccessValue(DesiredAccess), 1, Overlapped);

    public static string GetFinalPathName(SafeFileHandle file, ByHandlePathFlags flags)
    {

        var buffer = new StringBuilder(32768);

        if (GetFinalPathNameByHandle(file, buffer, buffer.Capacity, flags) == 0)
        {
            throw new Win32Exception();
        }

        return buffer.ToString();

    }

    public static string GetFinalPathName(string file, ByHandlePathFlags flags)
    {

        using var handle = OpenFileHandle(file, FileMode.Open, 0, FileShare.ReadWrite | FileShare.Delete, FILE_FLAG_BACKUP_SEMANTICS);

        return GetFinalPathName(handle, flags);

    }

    public static void DisableCommTimeouts(SafeFileHandle SafeFileHandle)
    {

        var CommTimeouts = new COMMTIMEOUTS();

        Win32Try(SetCommTimeouts(SafeFileHandle, in CommTimeouts));

    }

    private static void SetFileCompressionState(SafeFileHandle SafeFileHandle, ushort State)
        => Win32Try(DeviceIoControl(SafeFileHandle, FSCTL_SET_COMPRESSION, State, 2U, IntPtr.Zero, 0U, out _, IntPtr.Zero));

    public static void CreateDirectory(string path) =>

#if NETFRAMEWORK && !NET462_OR_GREATER
        Win32Try(Win32API.CreateDirectory(path, IntPtr.Zero));
#else
        Directory.CreateDirectory(path);
#endif

    public static long GetFileSize(string path)
    {

        using var handle = OpenFileHandle(path, FileMode.Open, 0, FileShare.Delete | FileShare.ReadWrite, 0);

        return GetFileSize(handle);

    }

    public static long GetFileSize(SafeFileHandle SafeFileHandle)
    {

        Win32Try(GetFileSizeEx(SafeFileHandle, out var FileSize));

        return FileSize;

    }

    public static void CompressFile(SafeFileHandle SafeFileHandle) => SetFileCompressionState(SafeFileHandle, COMPRESSION_FORMAT_DEFAULT);

    public static void UncompressFile(SafeFileHandle SafeFileHandle) => SetFileCompressionState(SafeFileHandle, COMPRESSION_FORMAT_NONE);

    public static string GetModuleFullPath(string ModuleName, bool Load)
    {

        var hModule = Load ? LoadLibrary(ModuleName) : GetModuleHandle(ModuleName);
        if (hModule == IntPtr.Zero)
        {
            throw new Win32Exception();
        }

        try
        {
            return GetModuleFullPath(hModule);
        }

        finally
        {
            if (Load)
            {
                FreeLibrary(hModule);
            }

        }

    }

    [SecuritySafeCritical]
    public static string[] CommandLineToArgumentArray(string CommandLine)
    {
        var ArgsPtr = CommandLineToArgv(CommandLine ?? GetCommandLine(), out var NumArgs);
        
        if (ArgsPtr == default)
        {
            throw new Win32Exception();
        }

        try
        {
            var ArgsArray = new string[NumArgs];
            for (int i = 0, loopTo = NumArgs - 1; i <= loopTo; i++)
            {
                var ParamPtr = ReadIntPtr(ArgsPtr, IntPtr.Size * i);
                ArgsArray[i] = PtrToStringUni(ParamPtr)!;
            }

            return ArgsArray;
        }
        finally
        {
            LocalFree(ArgsPtr);
        }
    }

    [SecuritySafeCritical]
    public static string GetLongFullPath(string path)
    {
        path = GetNtPath(path);

        if (path.StartsWith(@"\??\"))
        {
#if NET6_0_OR_GREATER
            path = $@"\\?\{path.AsSpan(4)}";
#elif NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
            path = $@"\\?\{path.AsMemory(4)}";
#else
            path = $@"\\?\{path.Substring(4)}";
#endif
        }

        return path;
    }

    [SecuritySafeCritical]
    public static string GetNtPath(string Win32Path)
    {
        var RC = RtlDosPathNameToNtPathName_U(Win32Path, out var UnicodeString, default, default);
        if (!RC)
        {
            throw new IOException($"Invalid path: '{Win32Path}'");
        }

        try
        {
            return UnicodeString.ToString();
        }
        finally
        {
            RtlFreeUnicodeString(UnicodeString);
        }
    }

#if NETCOREAPP && !NETCOREAPP3_0_OR_GREATER

    [Obsolete]
    [SecuritySafeCritical]
    public static Delegate GetProcAddress(IntPtr hModule, string procedureName, Type delegateType)
    {

        return GetDelegateForFunctionPointer(Win32Try(Win32API.GetProcAddress(hModule, procedureName)), delegateType);

    }

    [Obsolete]
    [SecuritySafeCritical]
    public static Delegate GetProcAddress(string moduleName, string procedureName, Type delegateType)
    {

        var hModule = Win32Try(LoadLibrary(moduleName));
        return GetDelegateForFunctionPointer(Win32Try(Win32API.GetProcAddress(hModule, procedureName)), delegateType);

    }

    [SecuritySafeCritical]
    public static T GetProcAddress<T>(IntPtr hModule, string procedureName)
    {

        return GetDelegateForFunctionPointer<T>(Win32Try(Win32API.GetProcAddress(hModule, procedureName)));

    }

    [SecuritySafeCritical]
    public static T GetProcAddress<T>(string moduleName, string procedureName)
    {

        var hModule = Win32Try(LoadLibrary(moduleName));
        return GetDelegateForFunctionPointer<T>(Win32Try(Win32API.GetProcAddress(hModule, procedureName)));

    }

#else
    [SecuritySafeCritical]
    public static Delegate GetProcAddress(IntPtr hModule, string procedureName, Type delegateType) =>
        GetDelegateForFunctionPointer(Win32Try(Win32API.GetProcAddress(hModule, procedureName)), delegateType);

    [SecuritySafeCritical]
    public static Delegate GetProcAddress(string moduleName, string procedureName, Type delegateType)
    {

        var hModule = Win32Try(LoadLibrary(moduleName));
        return GetDelegateForFunctionPointer(Win32Try(Win32API.GetProcAddress(hModule, procedureName)), delegateType);

    }

#endif

    /// <summary>
    /// Retrieves disk geometry.
    /// </summary>
    /// <param name="hDevice">Handle to device.</param>
    public static DISK_GEOMETRY? GetDiskGeometry(SafeFileHandle hDevice)
    {
        if (DeviceIoControl(hDevice, IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0U, out var DiskGeometry, MarshalSupport<DISK_GEOMETRY>.Size, out _, default))
        {

            return DiskGeometry;

        }

        return default;

    }

    public static string SearchPath(string path, string filename, string extension)
    {

        var str = new StringBuilder(32768);
        var PathLength = Win32API.SearchPath(path, filename, extension, str.Capacity, str, out _);
        if (PathLength == 0)
        {
            throw new Win32Exception();
        }

        return str.ToString();

    }

    public static int GetVolumeClusterSize(string rootPath)
    {
        if (!GetDiskFreeSpace(rootPath, out var SectorsPerCluster, out var BytesPerSector, out _, out _))
        {
            throw new Win32Exception();
        }

        return SectorsPerCluster * BytesPerSector;

    }

    public static void GenerateRandom(byte[] buffer) => Win32Try(RtlGenRandom(buffer, buffer.Length));

    public static sbyte GenerateRandomSByte()
    {
        Win32Try(RtlGenRandom(out sbyte b, 1));
        return b;
    }

    public static byte GenerateRandomByte()
    {
        Win32Try(RtlGenRandom(out byte b, 1));
        return b;
    }

    public static short GenerateRandomInt16()
    {
        Win32Try(RtlGenRandom(out short b, 2));
        return b;
    }

    public static int GenerateRandomInt32()
    {
        Win32Try(RtlGenRandom(out int b, 4));
        return b;
    }

    public static long GenerateRandomInt64()
    {
        Win32Try(RtlGenRandom(out long b, 8));
        return b;
    }

    public static Guid GenerateRandomGuid()
    {
        Win32Try(RtlGenRandom(out Guid b, 16));
        return b;
    }

    public static unsafe void GenerateRandom<T>(T[] values) where T : unmanaged
    {
        fixed (T* pinptr = values)
        {
            Win32Try(RtlGenRandom(new IntPtr(pinptr), Buffer.ByteLength(values)));
        }
    }

    public static unsafe void GenerateRandom<T>(T[] values, int startIndex, int count) where T : unmanaged
    {
        if (startIndex < 0 || count < 0 || startIndex + count > values.Length)
        {
            throw new IndexOutOfRangeException();
        }

        fixed (T* ptr = values)
        {
            Win32Try(RtlGenRandom(new IntPtr(ptr + startIndex), count * sizeof(T)));
        }
    }

    public static string[] QueryDosDevice() => QueryDosDevice(null)
        ?? throw new InvalidOperationException("QueryDosDevice not supported");

    public static string[]? QueryDosDevice(string? DosDevice)
    {
        var TargetPath = new char[65537];

        var length = Win32API.QueryDosDevice(DosDevice, TargetPath, (uint)TargetPath.Length);

        if (length < 2)
        {
            return null;
        }

        var Target = new string(TargetPath, 0, length - 2);

        return Target.Split('\0', StringSplitOptions.RemoveEmptyEntries);
    }

    public static void SetVolumeMountPoint(string VolumeMountPoint, string VolumeName) => Win32Try(Win32API.SetVolumeMountPoint(VolumeMountPoint, VolumeName));

    public static char FindFirstFreeDriveLetter() => FindFirstFreeDriveLetter('D');

    public static char FindFirstFreeDriveLetter(char start)
    {
        start = char.ToUpperInvariant(start);
        if (start < 'A' || start > 'Z')
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        var logical_drives = GetLogicalDrives();

        for (ushort s = Convert.ToUInt16(start), loopTo = Convert.ToUInt16('Z'); s <= loopTo; s++)
        {
            if ((logical_drives & 1 << s - Convert.ToUInt16('A')) == 0L)
            {
                return Convert.ToChar(s);
            }
        }

        return default;

    }

    public static DiskExtent[] GetVolumeDiskExtents(SafeFileHandle volume)
    {

        var extents = new DiskExtents();

#if NETCOREAPP && !NETCOREAPP3_0_OR_GREATER
        var struct_size = SizeOf<DiskExtents>();
#else
        var struct_size = SizeOf(typeof(DiskExtents));
#endif

        var rc = DeviceIoControl(volume, IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, default, 0, out extents, struct_size, out var arglpBytesReturned, IntPtr.Zero);

        if (!rc)
        {
            throw new Win32Exception();
        }

        return extents.CreateExtentsArray();

    }

    public static string GetModuleFullPath(IntPtr hModule)
    {
        var str = new StringBuilder(32768);

        var PathLength = GetModuleFileName(hModule, str, str.Capacity);
        if (PathLength == 0)
        {
            throw new Win32Exception();
        }

        return str.ToString();
    }

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static IEnumerable<string> GetDiskVolumesMountPoints(uint DiskNumber) => GetDiskVolumes(DiskNumber).SelectMany(GetVolumeMountPoints);

    public static string GetVolumeNameForVolumeMountPoint(string MountPoint)
    {

        var str = new StringBuilder(65536);

        Win32Try(Win32API.GetVolumeNameForVolumeMountPoint(MountPoint, str, str.Capacity));

        return str.ToString();

    }

    private static readonly int _GetDevicesScsiAddresses_SizeOfScsiAddress = MarshalSupport<SCSI_ADDRESS>.Size;

    public static Dictionary<uint, string> GetDevicesScsiAddresses(byte portnumber)
    {
        static SCSI_ADDRESS? GetScsiAddress(string drv)
        {
            try
            {
                using var disk = OpenFileHandle(drv, FileMode.Open, 0, FileShare.ReadWrite, 0);
                var rc = DeviceIoControl(disk, IOCTL_SCSI_GET_ADDRESS, IntPtr.Zero, 0, out SCSI_ADDRESS ScsiAddress, _GetDevicesScsiAddresses_SizeOfScsiAddress, out _, default);
                var errcode = GetLastWin32Error();
                if (rc)
                {
                    return (SCSI_ADDRESS?)ScsiAddress;
                }
                else
                {
                    Trace.WriteLine($"IOCTL_SCSI_GET_ADDRESS failed for device {drv}: Error 0x{errcode:X}");
                    return default;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Exception attempting to find SCSI address for device {drv}: {ex}");
                return default;
            }
        };

        var q = from drv in QueryDosDevice()
                where drv.StartsWith("PhysicalDrive", StringComparison.Ordinal) || drv.StartsWith("CdRom", StringComparison.Ordinal)
                let address = GetScsiAddress(string.Concat(@"\\?\", drv))
                where address.HasValue
                where address.Value.PortNumber == portnumber
                select new KeyValuePair<uint, string>(address.Value.DWordDeviceNumber, drv);

        return q.ToDictionary(o => o.Key, o => o.Value);

    }

    public static string[] GetVolumeMountPoints(string VolumeName)
    {

        var TargetPath = new char[65537];

        Win32Try(GetVolumePathNamesForVolumeName(VolumeName, TargetPath, (uint)TargetPath.Length, out var length));

        if (length <= 2L)
        {
            return ReflectionExtensions.Empty<string>();
        }

        var Target = new string(TargetPath, 0, (int)(length - 2L));

        return Target.Split('\0', StringSplitOptions.RemoveEmptyEntries);

    }

    public static IEnumerable<string> GetDiskVolumes(string DevicePath)
    {

        if (DevicePath.StartsWith(@"\\?\PhysicalDrive", StringComparison.Ordinal))
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            return GetDiskVolumes(uint.Parse(DevicePath.AsSpan(@"\\?\PhysicalDrive".Length)));
#else
            return GetDiskVolumes(uint.Parse(DevicePath.Substring(@"\\?\PhysicalDrive".Length)));
#endif
        }
        else
        {
            return GetVolumeNamesForDeviceObject(QueryDosDevice(DevicePath.Substring(@"\\?\".Length))?.FirstOrDefault()
                ?? throw new DriveNotFoundException($"Volume '{DevicePath}' not found"));
        }
    }

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static IEnumerable<string> GetDiskVolumes(uint DiskNumber)
    {
        return new VolumeEnumerator()
            .Where(volumeGuid =>
            {
                try
                {
                    return VolumeUsesDisk(volumeGuid, DiskNumber);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{volumeGuid}: {ex.JoinMessages()}");
                    return false;
                }
            });
    }

    public static IEnumerable<string> GetVolumeNamesForDeviceObject(string DeviceObject)
    {
        return new VolumeEnumerator()
            .Where(volumeGuid =>
            {
                try
                {
                    if (volumeGuid.StartsWith(@"\\?\", StringComparison.Ordinal))
                    {
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
                        volumeGuid = volumeGuid.AsSpan(4).TrimEnd('\\').ToString();
#else
                        volumeGuid = volumeGuid.Substring(4).TrimEnd('\\');
#endif
                    }
                    else
                    {
                        volumeGuid = volumeGuid.TrimEnd('\\');
                    }

                    return QueryDosDevice(volumeGuid)?
                        .Any(target => target.Equals(DeviceObject, StringComparison.OrdinalIgnoreCase))
                        ?? false;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{volumeGuid}: {ex.JoinMessages()}");
                    return false;
                }
            });
    }
#endif

    public static bool VolumeUsesDisk(string VolumeGuid, uint DiskNumber)
    {

        using var volume = OpenFileHandle(VolumeGuid.TrimEnd('\\'), FileMode.Open, 0, FileShare.ReadWrite, 0);

        try
        {
            var extents = GetVolumeDiskExtents(volume);

            return extents.Any(extent => extent.DiskNumber.Equals((object)DiskNumber));
        }

        catch (Win32Exception ex) when (ex.NativeErrorCode == ERROR_INVALID_FUNCTION)
        {

            return false;

        }

    }

#endif

    public static PARTITION_INFORMATION? GetPartitionInformation(SafeFileHandle disk)
    {
        if (DeviceIoControl(disk,
                            IOCTL_DISK_GET_PARTITION_INFO_EX,
                            IntPtr.Zero,
                            0U,
                            out PARTITION_INFORMATION partition_info,
                            (uint)MarshalSupport<PARTITION_INFORMATION>.Size,
                            out _,
                            IntPtr.Zero))
        {
            return partition_info;
        }
        else
        {
            return default;
        }
    }

    public static PARTITION_INFORMATION_EX? GetPartitionInformationEx(SafeFileHandle disk)
    {
        if (DeviceIoControl(disk,
                            IOCTL_DISK_GET_PARTITION_INFO_EX,
                            IntPtr.Zero,
                            0U,
                            out PARTITION_INFORMATION_EX partition_info,
                            (uint)MarshalSupport<PARTITION_INFORMATION_EX>.Size,
                            out _,
                            IntPtr.Zero))
        {
            return partition_info;
        }
        else
        {
            return default;
        }
    }

    public static bool? GetDiskOffline(SafeFileHandle disk)
    {
        var attribs_size = (byte)16;
        var attribs = new byte[attribs_size];
        if (DeviceIoControl(disk,
                            IOCTL_DISK_GET_DISK_ATTRIBUTES,
                            IntPtr.Zero,
                            0U,
                            attribs,
                            attribs_size,
                            out _,
                            IntPtr.Zero))
        {
            return (attribs[8] & 1) != 0;
        }
        else
        {
            return default;
        }
    }

    public static void SetDiskOffline(SafeFileHandle disk, bool offline)
    {
        var attribs_size = (byte)40;
        var attribs = new byte[attribs_size];
        attribs[0] = attribs_size;
        attribs[16] = 1;
        if (offline)
        {
            attribs[8] = 1;
        }

        Win32Try(DeviceIoControl(disk, IOCTL_DISK_SET_DISK_ATTRIBUTES, attribs, attribs_size, IntPtr.Zero, 0U, out _, IntPtr.Zero));
    }

    public static bool? GetDiskReadOnly(SafeFileHandle disk)
    {
        var attribs_size = (byte)16;
        var attribs = new byte[attribs_size];
        if (DeviceIoControl(disk,
                            IOCTL_DISK_GET_DISK_ATTRIBUTES,
                            IntPtr.Zero,
                            0U,
                            attribs,
                            attribs_size,
                            out _,
                            IntPtr.Zero))
        {
            return (attribs[8] & 2) != 0;
        }
        else
        {
            return default;
        }
    }

    public static void SetDiskReadOnly(SafeFileHandle disk, bool read_only)
    {
        var attribs_size = (byte)40;
        var attribs = new byte[attribs_size];
        attribs[0] = attribs_size;
        attribs[16] = 2;
        if (read_only)
        {
            attribs[8] = 2;
        }

        Win32Try(DeviceIoControl(disk, IOCTL_DISK_SET_DISK_ATTRIBUTES, attribs, attribs_size, IntPtr.Zero, 0U, out _, IntPtr.Zero));
    }

    public static void SetVolumeOffline(SafeFileHandle disk, bool offline)
        => Win32Try(DeviceIoControl(disk,
                                    offline ? IOCTL_VOLUME_OFFLINE : IOCTL_VOLUME_ONLINE,
                                    IntPtr.Zero,
                                    0U,
                                    IntPtr.Zero,
                                    0U,
                                    out _,
                                    IntPtr.Zero));

    private static class MarshalSupport<T>
    {
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
        public static readonly int Size = SizeOf<T>();
#else
        public static readonly int Size = SizeOf(typeof(T));
#endif
    }

#if NETCOREAPP3_0_OR_GREATER || !NETCOREAPP
    public static OperatingSystem GetOSVersion()
    {
        var os_version = new OSVERSIONINFOEX();

        var status = RtlGetVersion(ref os_version);

        if (status < 0)
        {
            throw new Win32Exception(RtlNtStatusToDosError(status));
        }

        return new OperatingSystem(os_version.PlatformId,
                                   new Version(os_version.MajorVersion,
                                               os_version.MinorVersion,
                                               os_version.BuildNumber,
                                               os_version.ServicePackMajor << 16 | os_version.ServicePackMinor));
    }
#endif

    public static void SetFileSparseFlag(SafeFileHandle file, bool flag)
        => Win32Try(DeviceIoControl(file, FSCTL_SET_SPARSE, flag, 1U, default, 0U, out _, IntPtr.Zero));

#if NETCOREAPP3_0_OR_GREATER || !NETCOREAPP
    /// <summary>
    /// Returns character encoding object for current OEM codepage.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
    public static Encoding GetOemEncoding() => Encoding.GetEncoding(GetOEMCP());
#endif

    /// <summary>
    /// Saves a value to an INI file by calling Win32 API function WritePrivateProfileString. If call fails and exception
    /// is thrown.
    /// </summary>
    /// <param name="FileName">Name and path of INI file where to save value</param>
    /// <param name="SectionName">Name of INI file section where to save value</param>
    /// <param name="SettingName">Name of value to save</param>
    /// <param name="Value">Value to save</param>
    [SecurityCritical]
    public static void SaveIniFileValue(string? FileName, string SectionName, string SettingName, string? Value)
        => Win32Try(WritePrivateProfileString(SectionName, SettingName, Value, FileName));

}

[Flags]
public enum ByHandlePathFlags
{

    FileNameNormalized = 0x0,
    FileNameOpened = 0x8,

    VolumeNameDOS = 0x0,
    VolumeNameGUID = 0x1,
    VolumeNameNT = 0x2,
    VolumeNameNone = 0x4

}