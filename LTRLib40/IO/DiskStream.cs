// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace LTRLib.IO;

[SecurityCritical]
[SupportedOSPlatform("windows")]
public class DiskStream : FileStream
{

    public string DevicePath { get; }

#if NETFRAMEWORK && !NET462_OR_GREATER
    public DiskStream(string path, FileAccess access) : base(NativeFileIO.OpenFileHandle(path, FileMode.Open, access, FileShare.ReadWrite | FileShare.Delete, 0), access: NativeFileIO.GetFileStreamLegalAccessValue(access), bufferSize: 512)
    {

        DevicePath = path;
        if (!Win32API.DeviceIoControl(SafeFileHandle, NativeConstants.FSCTL_ALLOW_EXTENDED_DASD_IO, IntPtr.Zero, 0U, IntPtr.Zero, 0U, out _, IntPtr.Zero))
        {
            var errcode = Marshal.GetLastWin32Error();
            if (errcode != NativeConstants.ERROR_INVALID_PARAMETER)
            {
                Trace.WriteLine($"FSCTL_ALLOW_EXTENDED_DASD_IO failed for '{path}': {errcode}");
            }

        }

    }

#else

    public DiskStream(string path, FileAccess access) : base(path, FileMode.Open, access, FileShare.ReadWrite | FileShare.Delete, 512)
    {

        DevicePath = path;

        if (!Win32API.DeviceIoControl(SafeFileHandle, NativeConstants.FSCTL_ALLOW_EXTENDED_DASD_IO, IntPtr.Zero, 0U, IntPtr.Zero, 0U, out _, IntPtr.Zero))
        {
            var errcode = Marshal.GetLastWin32Error();
            if (errcode != NativeConstants.ERROR_INVALID_PARAMETER)
            {
                Trace.WriteLine($"FSCTL_ALLOW_EXTENDED_DASD_IO failed for '{path}': {errcode}");
            }

        }

    }

#endif

    public override long Length
    {
        [SecuritySafeCritical]
        get
        {
            return NativeFileIO.GetVolumeSize(SafeFileHandle);
        }
    }

    [SecuritySafeCritical]
    public override void SetLength(long value) => throw new NotSupportedException();

    public NativeConstants.DISK_GEOMETRY? Geometry
    {
        [SecuritySafeCritical]
        get
        {
            return NativeFileIO.GetDiskGeometry(SafeFileHandle);
        }
    }

    public NativeConstants.DiskExtent[] VolumeDiskExtents
    {
        [SecuritySafeCritical]
        get
        {
            return NativeFileIO.GetVolumeDiskExtents(SafeFileHandle);
        }
    }

    public bool VolumeOffline
    {
        set
        {
            NativeFileIO.SetVolumeOffline(SafeFileHandle, value);
        }
    }

}