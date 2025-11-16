/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Buffers;
#endif
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static LTRLib.IO.NativeConstants;

#pragma warning disable CS0649

namespace LTRLib.IO;

public enum IncludeSubdirectoriesOption
{
    TopLevelOnly,

    ExceptReparsePoints,

    Any
}

[Flags]
public enum InlcudeEntriesOption
{
    Files = 0x1,

    Directories = 0x2,

    Both = Files | Directories
}

[Serializable]
[SupportedOSPlatform("windows")]
public class NativeFileInfo
{
    public DateTime CreationTime { get; }
    public DateTime LastAccessTime { get; }
    public DateTime LastWriteTime { get; }
    public DateTime ChangeTime { get; }

    public FileAttributes Attributes { get; }

    public long AllocationSize { get; }
    public long EndOfFile { get; }
    public uint NumberOfLinks { get; }

    public bool DeletePending { get; }

    public bool IsDirectory { get; }

    public long IndexNumber { get; }

    public uint EaSize { get; }

    public uint AccessFlags { get; }

    public long CurrentByteOffset { get; }

    public uint Mode { get; }

    public uint AlignmentRequirement { get; }

    public string Name { get; }

    bool IsFile => !IsDirectory;

    bool IsReparsePoint => (Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;

    internal unsafe NativeFileInfo(FILE_ALL_INFORMATION* all_info)
    {
        CreationTime = DateTime.FromFileTime(all_info->BasicInformation.CreationTime);
        LastAccessTime = DateTime.FromFileTime(all_info->BasicInformation.LastAccessTime);
        LastWriteTime = DateTime.FromFileTime(all_info->BasicInformation.LastWriteTime);
        ChangeTime = DateTime.FromFileTime(all_info->BasicInformation.ChangeTime);

        Attributes = (FileAttributes)all_info->BasicInformation.FileAttributes;

        AllocationSize = all_info->StandardInformation.AllocationSize;
        EndOfFile = all_info->StandardInformation.EndOfFile;
        NumberOfLinks = all_info->StandardInformation.NumberOfLinks;
        DeletePending = all_info->StandardInformation.DeletePending != 0;

        IsDirectory = all_info->StandardInformation.Directory != 0;

        IndexNumber = all_info->InternalInformation.IndexNumber;
        EaSize = all_info->EaInformation.EaSize;
        AccessFlags = all_info->AccessInformation.AccessFlags;
        CurrentByteOffset = all_info->PositionInformation.CurrentByteOffset;
        Mode = all_info->ModeInformation.Mode;
        AlignmentRequirement = all_info->AlignmentInformation.AlignmentRequirement;

        Name = new string(&all_info->NameInformation.FileName, 0,
            (int)all_info->NameInformation.FileNameLength / sizeof(char));
    }

    [SupportedOSPlatform("windows")]
    public static unsafe NativeFileInfo? FromFile(SafeFileHandle handle, bool throwOnFail)
    {
        if (handle == null || handle.IsClosed || handle.IsInvalid)
        {
            throw new ArgumentNullException(nameof(handle), "Handle parameter cannot be null or a closed or invalid handle.");
        }

        var bytebuffer = new byte[65634];

        fixed (void* all_info = bytebuffer)
        {
            var status = NtIO.NtQueryInformationFile(
                handle,
                out _,
                all_info,
                (uint)bytebuffer.Length,
                FILE_INFORMATION_CLASS.FileAllInformation);

            if (status < 0)
            {
                if (throwOnFail)
                {
                    throw new Win32Exception(Win32API.RtlNtStatusToDosError(status));
                }
                else
                {
                    return null;
                }
            }

            return new NativeFileInfo((FILE_ALL_INFORMATION*)all_info);
        }
    }

    public static unsafe NativeFileInfo? FromFile(string path, bool throwOnFail)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
        }

        using var handle = Win32API.CreateFile(path, 0,
            FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, 0,
            OPEN_EXISTING, (int)FILE_FLAG_BACKUP_SEMANTICS, 0);

        if (handle.IsInvalid)
        {
            if (throwOnFail)
            {
                throw new Win32Exception();
            }
            else
            {
                return null;
            }
        }

        return FromFile(handle, throwOnFail);
    }
}

internal enum FILE_INFORMATION_CLASS
{
    FileDirectoryInformation = 1,
    FileFullDirectoryInformation,   // 2
    FileBothDirectoryInformation,   // 3
    FileBasicInformation,   // 4  wdm
    FileStandardInformation,    // 5  wdm
    FileInternalInformation,    // 6
    FileEaInformation,      // 7
    FileAccessInformation,  // 8
    FileNameInformation,    // 9
    FileRenameInformation,  // 10
    FileLinkInformation,    // 11
    FileNamesInformation,   // 12
    FileDispositionInformation, // 13
    FilePositionInformation,    // 14 wdm
    FileFullEaInformation,  // 15
    FileModeInformation,    // 16
    FileAlignmentInformation,   // 17
    FileAllInformation,     // 18
    FileAllocationInformation,  // 19
    FileEndOfFileInformation,   // 20 wdm
    FileAlternateNameInformation,   // 21
    FileStreamInformation,  // 22
    FilePipeInformation,    // 23
    FilePipeLocalInformation,   // 24
    FilePipeRemoteInformation,  // 25
    FileMailslotQueryInformation,   // 26
    FileMailslotSetInformation, // 27
    FileCompressionInformation, // 28
    FileObjectIdInformation,    // 29
    FileCompletionInformation,  // 30
    FileMoveClusterInformation, // 31
    FileQuotaInformation,   // 32
    FileReparsePointInformation,    // 33
    FileNetworkOpenInformation, // 34
    FileAttributeTagInformation,    // 35
    FileTrackingInformation,    // 36
    FileIdBothDirectoryInformation, // 37
    FileIdFullDirectoryInformation, // 38
    FileValidDataLengthInformation, // 39
    FileShortNameInformation,   // 40
    FileMaximumInformation
}

internal readonly unsafe struct FILE_DIRECTORY_INFORMATION
{
    public uint NextEntryOffset { get; }
    public uint FileIndex { get; }
    public long CreationTime { get; }
    public long LastAccessTime { get; }
    public long LastWriteTime { get; }
    public long ChangeTime { get; }
    public long EndOfFile { get; }
    public long AllocationSize { get; }
    public uint FileAttributes { get; }
    public uint FileNameLength { get; }
    internal readonly char fileName;
}

internal readonly unsafe struct FILE_FULL_DIR_INFORMATION
{
    public uint NextEntryOffset { get; }
    public uint FileIndex { get; }
    public long CreationTime { get; }
    public long LastAccessTime { get; }
    public long LastWriteTime { get; }
    public long ChangeTime { get; }
    public long EndOfFile { get; }
    public long AllocationSize { get; }
    public uint FileAttributes { get; }
    public uint FileNameLength { get; }
    public uint EaSize { get; }
    internal readonly char fileName;
}

internal readonly unsafe struct FILE_BOTH_DIR_INFORMATION
{
    public uint NextEntryOffset { get; }
    public uint FileIndex { get; }
    public long CreationTime { get; }
    public long LastAccessTime { get; }
    public long LastWriteTime { get; }
    public long ChangeTime { get; }
    public long EndOfFile { get; }
    public long AllocationSize { get; }
    public uint FileAttributes { get; }
    public uint FileNameLength { get; }
    public uint EaSize { get; }
    public byte ShortNameLength { get; }
    private readonly ShortName shortName;
    public string ShortName => shortName.ToString();
    internal readonly char fileName;
}

internal unsafe struct ShortName
{
    private fixed char shortName[12];

    public override string ToString()
    {
        fixed (char* ptr = shortName)
        {
            return new(ptr);
        }
    }
}

internal readonly struct FILE_BASIC_INFORMATION
{
    public long CreationTime { get; }
    public long LastAccessTime { get; }
    public long LastWriteTime { get; }
    public long ChangeTime { get; }
    public uint FileAttributes { get; }
}

internal readonly struct FILE_STANDARD_INFORMATION
{
    public long AllocationSize { get; }
    public long EndOfFile { get; }
    public uint NumberOfLinks { get; }
    public byte DeletePending { get; }
    public byte Directory { get; }
}

internal readonly struct FILE_INTERNAL_INFORMATION
{
    public long IndexNumber { get; }
}

internal readonly struct FILE_EA_INFORMATION
{
    public uint EaSize { get; }
}

internal readonly struct FILE_ACCESS_INFORMATION
{
    public uint AccessFlags { get; }
}

internal readonly struct FILE_MODE_INFORMATION
{
    public uint Mode { get; }
}

internal readonly unsafe struct FILE_RENAME_INFORMATION
{
    public byte ReplaceIfExists { get; }
    public nint RootDirectory { get; }
    public uint FileNameLength { get; }
    internal readonly char FileName;
}

internal readonly unsafe struct FILE_LINK_INFORMATION
{
    public byte ReplaceIfExists { get; }
    public nint RootDirectory { get; }
    public uint FileNameLength { get; }
    internal readonly char FileName;
}

internal readonly unsafe struct FILE_NAMES_INFORMATION
{
    public uint NextEntryOffset { get; }
    public uint FileIndex { get; }
    public uint FileNameLength { get; }
    internal readonly char FileName;
}

internal readonly struct FILE_ALLOCATION_INFORMATION
{
    public long AllocationSize { get; }
}

internal unsafe struct FILE_COMPRESSION_INFORMATION
{
    public long CompressedFileSize { get; }
    public ushort CompressionFormat { get; }
    public byte CompressionUnitShift { get; }
    public byte ChunkShift { get; }
    public byte ClusterShift { get; }
    internal fixed byte Reserved[3];
}

internal readonly struct FILE_COMPLETION_INFORMATION
{
    public nint Port { get; }
    public uint Key { get; }
}

internal readonly struct FILE_POSITION_INFORMATION
{
    public long CurrentByteOffset { get; }
}

internal readonly struct FILE_ALIGNMENT_INFORMATION
{
    public uint AlignmentRequirement { get; }
}

internal readonly unsafe struct FILE_NAME_INFORMATION
{
    public uint FileNameLength { get; }
    internal readonly char FileName;
}

internal readonly struct FILE_NETWORK_OPEN_INFORMATION
{
    public long CreationTime { get; }
    public long LastAccessTime { get; }
    public long LastWriteTime { get; }
    public long ChangeTime { get; }
    public long AllocationSize { get; }
    public long EndOfFile { get; }
    public uint FileAttributes { get; }
}

internal readonly struct FILE_ATTRIBUTE_TAG_INFORMATION
{
    public uint FileAttributes { get; }
    public uint ReparseTag { get; }
}

internal readonly struct FILE_DISPOSITION_INFORMATION
{
    public byte DeleteFile { get; }
}

internal readonly struct FILE_END_OF_FILE_INFORMATION
{
    public long EndOfFile { get; }
}

internal readonly struct FILE_VALID_DATA_LENGTH_INFORMATION
{
    public long ValidDataLength { get; }
}

internal readonly struct FILE_ALL_INFORMATION
{
    public readonly FILE_BASIC_INFORMATION BasicInformation;
    public readonly FILE_STANDARD_INFORMATION StandardInformation;
    public readonly FILE_INTERNAL_INFORMATION InternalInformation;
    public readonly FILE_EA_INFORMATION EaInformation;
    public readonly FILE_ACCESS_INFORMATION AccessInformation;
    public readonly FILE_POSITION_INFORMATION PositionInformation;
    public readonly FILE_MODE_INFORMATION ModeInformation;
    public readonly FILE_ALIGNMENT_INFORMATION AlignmentInformation;
    public readonly FILE_NAME_INFORMATION NameInformation;
}

[SupportedOSPlatform("windows")]
public static class NativeDirectory
{
    public static IEnumerable<NativeDirectoryEntryInfo> EnumerateFiles(string path,
                                                                       string pattern,
                                                                       IncludeSubdirectoriesOption subdirectoriesOption,
                                                                       bool returnHardLinkedOnce) =>
        EnumerateEntries(path, pattern, subdirectoriesOption, InlcudeEntriesOption.Files, returnHardLinkedOnce);

    public static IEnumerable<NativeDirectoryEntryInfo> EnumerateDirectories(string path,
                                                                             string pattern,
                                                                             IncludeSubdirectoriesOption subdirectoriesOption,
                                                                             bool returnHardLinkedOnce) =>
        EnumerateEntries(path, pattern, subdirectoriesOption, InlcudeEntriesOption.Directories, returnHardLinkedOnce);

    public static IEnumerable<NativeDirectoryEntryInfo> EnumerateEntries(string path,
                                                                         string pattern,
                                                                         IncludeSubdirectoriesOption subdirectoriesOption,
                                                                         InlcudeEntriesOption entriesOption,
                                                                         bool returnHardLinkedOnce)
    {
        if (!returnHardLinkedOnce &&
            subdirectoriesOption == IncludeSubdirectoriesOption.TopLevelOnly &&
            entriesOption == InlcudeEntriesOption.Both)
        {
            return new NativeFileFinder(path, pattern);
        }

        return EnumerateEntries(path, pattern, subdirectoriesOption, entriesOption, returnHardLinkedOnce, linkList: null);
    }

    internal static IEnumerable<NativeDirectoryEntryInfo> EnumerateEntries(string path,
                                                                           string pattern,
                                                                           IncludeSubdirectoriesOption subdirectoriesOption,
                                                                           InlcudeEntriesOption entriesOption,
                                                                           bool returnHardLinkedOnce,
                                                                           List<long>? linkList)
    {
        if (returnHardLinkedOnce && linkList is null)
        {
            linkList = [];
        }

        foreach (var entry in new NativeFileFinder(path, pattern))
        {
            if (((entry.IsDirectory &&
                (entriesOption & InlcudeEntriesOption.Directories) == InlcudeEntriesOption.Directories) ||
                (entry.IsFile &&
                (entriesOption & InlcudeEntriesOption.Files) == InlcudeEntriesOption.Files)) &&
                (linkList is null || !linkList.Contains(entry.FileReference)))
            {
                if (returnHardLinkedOnce)
                {
                    if (subdirectoriesOption == IncludeSubdirectoriesOption.Any)
                    {
                        linkList?.Add(entry.FileReference);
                    }
                    else
                    {
                        var info = NativeFileInfo.FromFile(entry.FullPath, throwOnFail: false);

                        if (info is null || info.NumberOfLinks > 1)
                        {
                            linkList?.Add(entry.FileReference);
                        }
                    }
                }

                yield return entry;
            }
        }

        if (subdirectoriesOption == IncludeSubdirectoriesOption.TopLevelOnly)
        {
            yield break;
        }

        foreach (var entry in new NativeFileFinder(path))
        {
            if (entry.IsDirectory &&
                (subdirectoriesOption != IncludeSubdirectoriesOption.ExceptReparsePoints ||
                (entry.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) &&
                entry.FileName != "." &&
                entry.FileName != "..")
            {
                foreach (var subEntry in EnumerateEntries(entry.FullPath, pattern, subdirectoriesOption, entriesOption, returnHardLinkedOnce, linkList))
                {
                    yield return subEntry;
                }
            }
        }
    }
}

[SupportedOSPlatform("windows")]
internal class NativeFileFinder : IEnumerable<NativeDirectoryEntryInfo>
{
    private readonly string directory;

    private readonly string? pattern;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public NativeFileFinder(string directory, string pattern)
    {
        this.directory = directory;
        this.pattern = pattern;

        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentNullException(nameof(directory));
        }
    }

    public NativeFileFinder(string directory)
    {
        this.directory = directory;

        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentNullException(nameof(directory));
        }
    }

    public virtual IEnumerator<NativeDirectoryEntryInfo> GetEnumerator() => new NativeFileIterator(directory, pattern);
}

[SupportedOSPlatform("windows")]
internal class NativeFileIterator : IDisposable, IEnumerator<NativeDirectoryEntryInfo>
{
    private readonly string DirectoryPath;

    private readonly string? pattern;

    private readonly bool ownsHandle;

    private byte[] finder;

    private SafeFileHandle handle;

    private bool iterationStarted;

    private NativeDirectoryEntryInfo current = null!;

    public virtual NativeDirectoryEntryInfo Current => current;

    object? IEnumerator.Current => Current;

    private void Cleanup()
    {
        finder = null!;

        if (ownsHandle)
        {
            handle?.Dispose();
        }

        handle = null!;
    }

    public NativeFileIterator(string directory, string? pattern)
        : this(directory, pattern, Win32API.CreateFile(directory, 1u, 7u, 0, 3u, 33554432, 0), ownsHandle: true)
    {
        try
        {
            if (handle.IsInvalid)
            {
                throw new Win32Exception();
            }

            return;
        }
        catch
        {
            //try-fault
            ((IDisposable)this).Dispose();
            throw;
        }
    }

    public NativeFileIterator(string directory)
        : this(directory, null)
    {
    }

    protected NativeFileIterator(string directoryPath, string? pattern, SafeFileHandle handle, [MarshalAs(UnmanagedType.U1)] bool ownsHandle)
    {
        DirectoryPath = directoryPath;
        this.pattern = pattern;
        this.ownsHandle = ownsHandle;
        finder = new byte[65638];
        this.handle = handle;
    }

    public virtual unsafe bool MoveNext()
    {
        fixed (byte* ptr = finder)
        {
            int status;
            if (!iterationStarted && pattern != null)
            {
                if (pattern.Length >= 32767)
                {
                    throw new Win32Exception(123);
                }

                fixed (char* namechars = pattern)
                {
                    var num = checked((ushort)(pattern.Length << 1));
                    var name = new UNICODE_STRING((nint)namechars, num);
                    status = NtIO.NtQueryDirectoryFile(handle, null, null, null, out _, ptr, (uint)finder.Length, FILE_INFORMATION_CLASS.FileIdBothDirectoryInformation, 1, &name, 1);
                }
            }
            else
            {
                status = NtIO.NtQueryDirectoryFile(RestartScan: (!iterationStarted) ? ((byte)1) : ((byte)0),
                                                   FileHandle: handle,
                                                   Event: null,
                                                   ApcRoutine: null,
                                                   ApcContext: null,
                                                   IoStatusBlock: out _,
                                                   FileInformation: ptr,
                                                   Length: (uint)finder.Length,
                                                   FileInformationClass: (FILE_INFORMATION_CLASS)37,
                                                   ReturnSingleEntry: 1,
                                                   FileName: null);
            }

            if (status >= 0)
            {
                current = new NativeDirectoryEntryInfo(DirectoryPath, ptr);
            }
            else
            {
                if (status != -1073741809 && status != -2147483642)
                {
                    throw new Win32Exception(Win32API.RtlNtStatusToDosError(status));
                }

                current = null!;
            }

            iterationStarted = true;
            return (byte)((status >= 0) ? 1u : 0u) != 0;
        }
    }

    public virtual void Reset() => throw new NotImplementedException();

    protected virtual void Dispose(bool A_0) => Cleanup();

    public void Dispose()
    {
        Dispose(A_0: true);
        GC.SuppressFinalize(this);
    }

    ~NativeFileIterator()
    {
        Dispose(A_0: false);
    }
}
