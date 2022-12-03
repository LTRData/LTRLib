/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static LTRLib.IO.NativeConstants;

#pragma warning disable CS0649

namespace LTRLib.IO;

[Serializable]
public class NativeDirectoryEntryInfo
{
    public readonly string? DirectoryPath;

    public readonly string FullPath;

    public readonly string FileName;

    public readonly string ShortName;

    public readonly uint FileIndex;

    public readonly DateTime CreationTime;

    public readonly DateTime LastAccessTime;

    public readonly DateTime LastWriteTime;

    public readonly DateTime ChangeTime;

    public readonly long EndOfFile;

    public readonly long AllocationSize;

    public readonly long FileReference;

    public readonly FileAttributes Attributes;

    public readonly uint EaSize;

    public bool IsReparsePoint => (((uint)Attributes >> 10) & 1u) != 0;

    public bool IsFile => (~((uint)Attributes >> 4) & 1u) != 0;

    public bool IsDirectory => (((uint)Attributes >> 4) & 1u) != 0;

    internal unsafe NativeDirectoryEntryInfo(string? basePath, void* finder)
    {
        DirectoryPath = basePath;
        FileName = new string((char*)((byte*)finder + 104), 0, (int)((uint)(*(int*)((byte*)finder + 60)) >> 1));
        ShortName = new string((char*)((byte*)finder + 70), 0, (int)((uint)((byte*)finder)[68] >> 1));
        FileIndex = *(uint*)((byte*)finder + 4);
        CreationTime = DateTime.FromFileTime(*(long*)((byte*)finder + 8));
        LastAccessTime = DateTime.FromFileTime(*(long*)((byte*)finder + 16));
        LastWriteTime = DateTime.FromFileTime(*(long*)((byte*)finder + 24));
        ChangeTime = DateTime.FromFileTime(*(long*)((byte*)finder + 32));
        EndOfFile = *(long*)((byte*)finder + 40);
        AllocationSize = *(long*)((byte*)finder + 48);
        FileReference = *(long*)((byte*)finder + 96);
        Attributes = *(FileAttributes*)((byte*)finder + 56);
        EaSize = *(uint*)((byte*)finder + 64);
        FullPath = DirectoryPath != null ? Path.Combine(DirectoryPath, FileName) : FileName;
    }
}

public enum CreateDisposition : uint
{
    Supersede = 0u,
    Create = 2u,
    Open = 1u,
    OpenIf = 3u,
    Overwrite = 4u,
    OverwriteIf = 5u
}

[Flags]
public enum CreateOptions : uint
{
    DirectoryFile = 0x1u,
    NonDirectoryFile = 0x40u,
    WriteThrough = 0x2u,
    SequentialOnly = 0x4u,
    RandomAccess = 0x800u,
    NoIntermediateBuffering = 0x8u,
    SynchronousIoAlert = 0x10u,
    SynchronousIoNonAlert = 0x20u,
    CreateTreeConnection = 0x80u,
    NoEAKnowledge = 0x200u,
    OpenReparsePoint = 0x200000u,
    DeleteOnClose = 0x1000u,
    OpenByFileId = 0x2000u,
    OpenForBackupIntent = 0x4000u,
    ReserveOpFilter = 0x100000u
}

[SupportedOSPlatform("windows")]
public static class NtIO
{
    [DllImport("ntdll.dll")]
    internal static extern unsafe int NtQueryDirectoryFile(SafeFileHandle FileHandle,
                                                           void* Event,
                                                           delegate*<void*, IoStatusBlock*, uint, void> ApcRoutine,
                                                           void* ApcContext,
                                                           out IoStatusBlock IoStatusBlock,
                                                           void* FileInformation,
                                                           uint Length,
                                                           FILE_INFORMATION_CLASS FileInformationClass,
                                                           byte ReturnSingleEntry,
                                                           UNICODE_STRING* FileName,
                                                           byte RestartScan);

    [DllImport("ntdll.dll")]
    internal static extern unsafe int NtQueryInformationFile(SafeFileHandle FileHandle,
                                                             out IoStatusBlock IoStatusBlock,
                                                             void* FileInformation,
                                                             uint Length,
                                                             FILE_INFORMATION_CLASS FileInformationClass);

    public static FileStream OpenFileStream(string base_path,
                                            string relative_path,
                                            FileAccess file_access,
                                            FileAttributes file_attributes,
                                            FileShare file_share,
                                            CreateDisposition create_disposition,
                                            CreateOptions create_options)
    {
        using var root_dir = NativeFileIO.OpenFileHandle(base_path, FileMode.Open, 0, FileShare.ReadWrite | FileShare.Delete, FILE_FLAG_BACKUP_SEMANTICS);
        return OpenFileStream(root_dir, relative_path, file_access, file_attributes, file_share, create_disposition, create_options | CreateOptions.NonDirectoryFile);
    }

    public static SafeFileHandle OpenFileHandle(string base_path,
                                                string relative_path,
                                                FileAccess file_access,
                                                FileAttributes file_attributes,
                                                FileShare file_share,
                                                CreateDisposition create_disposition,
                                                CreateOptions create_options)
    {
        using var root_dir = NativeFileIO.OpenFileHandle(base_path, FileMode.Open, 0, FileShare.ReadWrite | FileShare.Delete, FILE_FLAG_BACKUP_SEMANTICS);
        return OpenFileHandle(root_dir, relative_path, file_access, file_attributes, file_share, create_disposition, create_options | CreateOptions.NonDirectoryFile);
    }

    public static FileStream OpenFileStream(SafeFileHandle root_dir,
                                            string relative_path,
                                            FileAccess file_access,
                                            FileAttributes file_attributes,
                                            FileShare file_share,
                                            CreateDisposition create_disposition,
                                            CreateOptions create_options)
    {
        var handle = OpenFileHandle(root_dir, relative_path, file_access, file_attributes, file_share, create_disposition, create_options | CreateOptions.NonDirectoryFile);
        return new FileStream(handle, file_access);
    }

    public static unsafe SafeFileHandle OpenFileHandle(SafeFileHandle root_dir,
                                                       string relative_path,
                                                       FileAccess file_access,
                                                       FileAttributes file_attributes,
                                                       FileShare file_share,
                                                       CreateDisposition create_disposition,
                                                       CreateOptions create_options)
    {
        fixed (char* path_ptr = relative_path)
        {
            var us_path = new UNICODE_STRING(new IntPtr(path_ptr), checked((ushort)(relative_path.Length * 2)));

            var ref_success = false;

            root_dir?.DangerousAddRef(ref ref_success);
            if (root_dir is null || !ref_success)
            {
                throw new ArgumentException("Invalid handle", nameof(root_dir));
            }

            try
            {
                var objattr = new ObjectAttributes(&us_path, ObjectAccess.CaseInsensitive | ObjectAccess.OpenIf, root_dir.DangerousGetHandle().ToPointer(), null, null);

                var status = NtCreateFile(out var handle, NativeFileIO.ConvertManagedFileAccess(file_access) | SYNCHRONIZE, objattr, out var io_status, null, file_attributes, file_share, create_disposition, create_options | CreateOptions.SynchronousIoNonAlert, null, 0);

                if (status < 0)
                {
                    throw new Win32Exception(Win32API.RtlNtStatusToDosError(status));
                }

                return handle;
            }
            finally
            {
                root_dir?.DangerousRelease();
            }
        }
    }

    [DllImport("ntdll.dll")]
    internal static extern unsafe int NtCreateFile(out SafeFileHandle FileHandle,
                                                   uint DesiredAccess,
                                                   in ObjectAttributes ObjectAttributes,
                                                   out IoStatusBlock IoStatusBlock,
                                                   [In] long* AllocationSize,
                                                   FileAttributes FileAttributes,
                                                   FileShare ShareAccess,
                                                   CreateDisposition CreateDisposition,
                                                   CreateOptions CreateOptions,
                                                   void* EaBuffer,
                                                   uint EaLength);

    public static unsafe SafeFileHandle OpenFileHandle(string nt_path,
                                                       FileAccess file_access,
                                                       FileAttributes file_attributes,
                                                       FileShare file_share,
                                                       CreateDisposition create_disposition,
                                                       CreateOptions create_options)
    {
        fixed (char* path_ptr = nt_path)
        {
            var us_path = new UNICODE_STRING(new IntPtr(path_ptr), checked((ushort)(nt_path.Length * 2)));

            var objattr = new ObjectAttributes(&us_path, ObjectAccess.CaseInsensitive | ObjectAccess.OpenIf);

            var status = NtCreateFile(out var handle,
                                      NativeFileIO.ConvertManagedFileAccess(file_access) | SYNCHRONIZE,
                                      objattr,
                                      out var io_status,
                                      null,
                                      file_attributes,
                                      file_share,
                                      create_disposition,
                                      create_options | CreateOptions.SynchronousIoNonAlert,
                                      null,
                                      0);

            if (status < 0)
            {
                throw new Win32Exception(Win32API.RtlNtStatusToDosError(status));
            }

            return handle;
        }
    }

    public static FileStream OpenFileStream(string nt_path,
                                            FileAccess file_access,
                                            FileAttributes file_attributes,
                                            FileShare file_share,
                                            CreateDisposition create_disposition,
                                            CreateOptions create_options)
    {
        var handle = OpenFileHandle(nt_path, file_access, file_attributes, file_share, create_disposition, create_options | CreateOptions.NonDirectoryFile);
        return new FileStream(handle, file_access);
    }

    public static unsafe NativeFsFullSizeInformation GetFilesystemSizeInfo(SafeFileHandle volume, [MarshalAs(UnmanagedType.U1)] bool throwOnFail)
    {
        var fs_size_info = default(NativeFsFullSizeInformation);
        var status = NtQueryVolumeInformationFile(volume, out _, &fs_size_info, (uint)sizeof(NativeFsFullSizeInformation), FSINFOCLASS.FileFsFullSizeInformation);
        
        if (status < 0)
        {
            if (throwOnFail)
            {
                throw new Win32Exception(Win32API.RtlNtStatusToDosError(status));
            }
            
            return default;
        }

        return fs_size_info;
    }

    [DllImport("ntdll.dll")]
    internal static extern unsafe int NtQueryVolumeInformationFile(SafeFileHandle FileHandle,
                                                                   out IoStatusBlock IoStatusBlock,
                                                                   void* FsInformation,
                                                                   uint Length,
                                                                   FSINFOCLASS FsInformationClass);
}

public enum FSINFOCLASS
{
    FileFsVolumeInformation = 1,
    FileFsLabelInformation, // 2 
    FileFsSizeInformation,  // 3 
    FileFsDeviceInformation,    // 4 
    FileFsAttributeInformation, // 5 
    FileFsControlInformation,   // 6 
    FileFsFullSizeInformation,  // 7 
    FileFsObjectIdInformation,  // 8 
    FileFsMaximumInformation
}

[Serializable]
public struct IoStatusBlock
{
    public IntPtr Pointer;

    public UIntPtr Information;

    public unsafe int Status
    {
        get => Pointer.ToInt32();
        set => Pointer = new(value);
    }
}

public class FileExtent
{
    public long StartingVcn { get; }
    public long NextVcn { get; }
    public long Lcn { get; }

    public bool IsLastExtent { get; }

    public long Length => NextVcn - StartingVcn;

    public bool IsSparseUnallocated => Lcn == -1L;

    internal unsafe FileExtent(RETRIEVAL_POINTERS_BUFFER* buffer, bool is_last)
    {
        StartingVcn = buffer->StartingVcn;
        NextVcn = buffer->NextVcn;
        Lcn = buffer->Lcn;
        IsLastExtent = is_last;
    }
}

internal struct RETRIEVAL_POINTERS_BUFFER
{
    public uint ExtentCount;
    public long StartingVcn;
    public long NextVcn;
    public long Lcn;
}

[Serializable]
public struct ObjectAttributes
{
    public uint uLength;

    public unsafe void* hRootDirectory;

    public unsafe UNICODE_STRING* pObjectName;

    public ObjectAccess uAttributes;

    public unsafe void* pSecurityDescriptor;

    public unsafe void* pSecurityQualityOfService;

    public unsafe ObjectAttributes(UNICODE_STRING* pObjectName, ObjectAccess uAttributes, void* hRootDirectory, void* pSecurityDescriptor, void* pSecurityQualityOfService)
    {
        uLength = (uint)sizeof(ObjectAttributes);
        this.hRootDirectory = hRootDirectory;
        this.pObjectName = pObjectName;
        this.uAttributes = uAttributes;
        this.pSecurityDescriptor = pSecurityDescriptor;
        this.pSecurityQualityOfService = pSecurityQualityOfService;
    }

    public unsafe ObjectAttributes(UNICODE_STRING* pObjectName, ObjectAccess uAttributes)
        : this(pObjectName, uAttributes, null, null, null)
    {
    }
}

[Flags]
public enum ObjectAccess : uint
{
    Inherit = 0x2u,
    Permanent = 0x10u,
    Exclusive = 0x20u,
    CaseInsensitive = 0x40u,
    OpenIf = 0x80u,
    OpenLink = 0x100u,
    ValidAttributes = 0x1F2u
}

[Serializable]
public readonly struct NativeFsFullSizeInformation
{
    public readonly long TotalAllocationUnits;

    public readonly long CallerAvailableAllocationUnits;

    public readonly long ActualAvailableAllocationUnits;

    public readonly int SectorsPerAllocationUnit;

    public readonly int BytesPerSector;

    public int BytesPerAllocationUnit => SectorsPerAllocationUnit * BytesPerSector;
}
