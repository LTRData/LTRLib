/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET40_OR_GREATER || NETCOREAPP

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.Versioning;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0056 // Use index operator
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0290 // Use primary constructor

namespace LTRLib.IO;

[SupportedOSPlatform("windows")]
public static partial class NativeVolume
{
#if NET7_0_OR_GREATER
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static unsafe partial bool DeviceIoControl(SafeFileHandle hDevice,
                                                        uint dwIoControlCode,
                                                        void* lpInBuffer,
                                                        int nInBufferSize,
                                                        void* lpOutBuffer,
                                                        int nOutBufferSize,
                                                        uint* lpBytesReturned,
                                                        NativeOverlapped* lpOverlapped);
#else
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern unsafe bool DeviceIoControl(SafeFileHandle hDevice,
                                                       uint dwIoControlCode,
                                                       void* lpInBuffer,
                                                       int nInBufferSize,
                                                       void* lpOutBuffer,
                                                       int nOutBufferSize,
                                                       uint* lpBytesReturned,
                                                       NativeOverlapped* lpOverlapped);
#endif

    public static IEnumerable<FileExtent> EnumerateFileExtents(string path) => EnumerateFileExtents(path, 0);

    public static IEnumerable<FileExtent> EnumerateFileExtents(string path, long start)
    {
        using var file = NativeFileIO.OpenFileHandle(path, FileMode.Open, 0, FileShare.ReadWrite | FileShare.Delete, NativeConstants.FILE_FLAG_BACKUP_SEMANTICS);
        foreach (var extent in EnumerateFileExtents(file, start))
        {
            yield return extent;
        }
    }

    internal static unsafe FileExtent? GetNextFileExtent(SafeFileHandle file, long start_vcn)
    {
        var input = start_vcn;
        RETRIEVAL_POINTERS_BUFFER output;
        uint stored_bytes;
        var rc = DeviceIoControl(file, 589939u, &input, sizeof(long), &output, sizeof(RETRIEVAL_POINTERS_BUFFER), &stored_bytes, null);
        var is_last = true;

        if (!rc)
        {
            switch (Marshal.GetLastWin32Error())
            {
                case 38:
                    return null;
                default:
                    throw new Win32Exception();
                case 234:
                    break;
            }

            is_last = false;
        }

        if (output.ExtentCount < 1u)
        {
            return null;
        }

        return new FileExtent(&output, is_last);
    }

    public static IEnumerable<FileExtent> EnumerateFileExtents(SafeFileHandle file) => EnumerateFileExtents(file, 0);

    public static IEnumerable<FileExtent> EnumerateFileExtents(SafeFileHandle file, long start)
    {
        for (; ; )
        {
            var result = GetNextFileExtent(file, start);

            if (result is null)
            {
                yield break;
            }

            start = result.NextVcn;

            yield return result;

            if (result.IsLastExtent)
            {
                yield break;
            }
        }
    }

    public static SeekStreamSegment? GetRawFileStream(Stream vol_stream, SafeFileHandle file, long fileOffset)
    {
        var fs_size_info = NtIO.GetFilesystemSizeInfo(file, throwOnFail: true);
        var bytes_per_cluster = fs_size_info.BytesPerAllocationUnit;

        var extents = EnumerateFileExtents(file, fileOffset / bytes_per_cluster).ToArray();

        if (extents.Length == 0)
        {
            return null;
        }

        var offsetFoundExtents = fileOffset - extents[0].StartingVcn * bytes_per_cluster;

        var streams = Array.ConvertAll(
            extents,
            extent => (Stream)(extent.IsSparseUnallocated
                    ? new ZeroStream(extent.Length * bytes_per_cluster, writable: false)
                    : new SeekStreamSegment(vol_stream, extent.Lcn * bytes_per_cluster, extent.Length * bytes_per_cluster, 0, ownsBaseStream: false)));

        var stream = new CombinedSeekStream(streams);

        return new SeekStreamSegment(stream, offsetFoundExtents, stream.Length - offsetFoundExtents, fs_size_info.BytesPerSector, ownsBaseStream: true);
    }

    public readonly struct AllocationExtent
    {
        public long StartPosition { get; }
        public long Length { get; }
        public bool Allocated { get; }

        public AllocationExtent(long StartPosition, long Length, bool Allocated)
        {
            this.StartPosition = StartPosition;
            this.Length = Length;
            this.Allocated = Allocated;
        }
    }

    public static CombinedSeekStream? OpenUnallocatedClustersAsStream(string volname, FileAccess access)
    {
        volname = volname.TrimEnd('\\');

        var vol_stream = new DiskStream(volname, access);

        using var root_dir = NativeFileIO.OpenFileHandle(volname + '\\', FileMode.Open,
            FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, NativeConstants.FILE_FLAG_BACKUP_SEMANTICS);

        var vol_size_info = NtIO.GetFilesystemSizeInfo(root_dir, throwOnFail: true);

        var cluster_size = vol_size_info.BytesPerAllocationUnit;

        var extents = EnumerateVolumeAllocationExtents(root_dir, cluster_size).ToList();

        if (extents.Count == 0)
        {
            return null;
        }

        // There might be volume slack space available after end-of-filesystem
        var last_extent = extents[extents.Count - 1];
        var fs_end_pos = last_extent.StartPosition + last_extent.Length;

        if (fs_end_pos < vol_stream.Length)
        {
            extents.Add(new AllocationExtent(fs_end_pos, vol_stream.Length - fs_end_pos, Allocated: false));
        }

        var unalloc_streams = extents
            .Where(extent => !extent.Allocated)
            .Select(extent => new SeekStreamSegment(vol_stream, extent.StartPosition, extent.Length, vol_size_info.BytesPerSector, ownsBaseStream: true));

        return new CombinedSeekStream([.. unalloc_streams]);
    }

    public static Task<CombinedSeekStream> OpenDiskRangesAsStreamAsync(string diskDevice, IEnumerable<NativeConstants.DiskExtent> except_extents, FileAccess access, CancellationToken cancellationToken) =>
        Task.Factory.StartNew(
            () => OpenDiskRangesAsStream(diskDevice, except_extents, access),
            cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);

    public static CombinedSeekStream OpenDiskRangesAsStream(string diskDevice, IEnumerable<NativeConstants.DiskExtent> except_extents, FileAccess access)
    {
        var unallocated_extents = new List<NativeConstants.DiskExtent>();

        long pos = 0;
        foreach (var extent in except_extents.OrderBy(extent => extent.StartingOffset))
        {
            unallocated_extents.Add(new NativeConstants.DiskExtent(
                StartingOffset: pos,
                ExtentLength: extent.StartingOffset - pos));

            pos = extent.StartingOffset + extent.ExtentLength;
        }

        var rawdisk = new DiskStream(diskDevice, access);

        var sector_size = rawdisk.Geometry?.BytesPerSector ?? 512;

        unallocated_extents.Add(new NativeConstants.DiskExtent(
            StartingOffset: pos,
            ExtentLength: rawdisk.Length - pos));

        unallocated_extents.RemoveAll(extent => extent.ExtentLength <= 0);

        var streams = unallocated_extents
            .ConvertAll(extent => new SeekStreamSegment(rawdisk, extent.StartingOffset, extent.ExtentLength, sector_size, ownsBaseStream: true))
            .ToArray();

        return new CombinedSeekStream(streams);
    }

    public static VolumeBitmap GetVolumeBitmap(SafeFileHandle file, long starting_lcn)
    {
        var bitmap = GetVolumeBitmap(file, starting_lcn, 8);
        var bytes_needed = (int)((7 + bitmap.NumberOfClusters) >> 3);
        return GetVolumeBitmap(file, bitmap.StartingLcn, bytes_needed);
    }

    public static unsafe VolumeBitmap GetVolumeBitmap(SafeFileHandle file, long starting_lcn, int max_bytes)
    {
        long input;
        input = starting_lcn;
        var buffersize = max_bytes + 16;
        var buffer = stackalloc byte[buffersize];
        var output = (VOLUME_BITMAP_BUFFER*)buffer;

        uint bytes_returned;
        if (!DeviceIoControl(file, 589935u, &input, sizeof(long), output, buffersize, &bytes_returned, null) && Marshal.GetLastWin32Error() != 234)
        {
            throw new Win32Exception();
        }

        return new VolumeBitmap(output, bytes_returned);
    }

    public sealed class VolumeBitmap
    {
        public readonly unsafe long StartingLcn;

        public readonly unsafe long NumberOfClusters;

        public readonly byte[] Bitmap;

        public bool this[long lcn]
        {
            [return: MarshalAs(UnmanagedType.U1)]
            get
            {
                var relative_lcn = lcn - StartingLcn;
                return (byte)(((uint)Bitmap[(int)(relative_lcn >> 3)] >> ((byte)relative_lcn & 7)) & 1u) != 0;
            }
        }

        internal unsafe VolumeBitmap(VOLUME_BITMAP_BUFFER* buffer, uint byte_size)
        {
            StartingLcn = buffer->StartingLcn;
            NumberOfClusters = buffer->BitmapSize;
            Bitmap = new byte[byte_size - 16];

            Marshal.Copy((nint)(&buffer->Buffer), Bitmap, 0, Bitmap.Length);
        }
    }

    internal struct VOLUME_BITMAP_BUFFER
    {
        public long StartingLcn;
        public long BitmapSize;
        public byte Buffer;
    }

    public static IEnumerable<AllocationExtent> EnumerateVolumeAllocationExtents(SafeFileHandle volume, int cluster_size)
    {
        var bitmap = GetVolumeBitmap(volume, 0);

        long cluster = 0;

        var allocated = false;

        while (cluster < bitmap.NumberOfClusters)
        {
            var start_cluster = cluster;

            for (;
                cluster < bitmap.NumberOfClusters &&
                bitmap[cluster] == allocated;
                cluster++)
            {
            }

            if (cluster > start_cluster)
            {
                yield return new AllocationExtent(
                    start_cluster * cluster_size,
                    (cluster - start_cluster) * cluster_size,
                    allocated);
            }

            allocated = !allocated;
        }
    }
}

#endif
