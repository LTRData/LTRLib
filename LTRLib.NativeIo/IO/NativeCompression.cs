/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET40_OR_GREATER || NETCOREAPP

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace LTRLib.IO;

public enum CompressionFormatAndEngine : ushort
{
    Standard = 0,
    Default = 1,
    LZNT1 = 2,
    Xpress = 3,
    XpressHuff = 4,
    Maximum = 256,
    Hiber = 512
}

[SupportedOSPlatform("windows")]
public static unsafe class NativeCompression
{
    [DllImport("ntdll.dll", SetLastError = false)]
    private static extern int RtlCompressBuffer(
        CompressionFormatAndEngine CompressionFormatAndEngine,
        byte* UncompressedBuffer,
        int UncompressedBufferSize,
        byte* CompressedBuffer,
        int CompressedBufferSize,
        int UncompressedChunkSize,
        out int FinalCompressedSize,
        byte* WorkSpace);

    [DllImport("ntdll.dll", SetLastError = false)]
    private static extern int RtlDecompressBuffer(
        CompressionFormatAndEngine CompressionFormatAndEngine,
        byte* UncompressedBuffer,
        int UncompressedBufferSize,
        byte* CompressedBuffer,
        int CompressedBufferSize,
        out int FinalUncompressedSize);

    [DllImport("ntdll.dll", SetLastError = false)]
    private static extern int RtlDecompressBufferEx(
        CompressionFormatAndEngine CompressionFormatAndEngine,
        byte* UncompressedBuffer,
        int UncompressedBufferSize,
        byte* CompressedBuffer,
        int CompressedBufferSize,
        out int FinalUncompressedSize,
        byte* WorkSpace);

    [DllImport("ntdll.dll", SetLastError = false)]
    private static extern int RtlDecompressBufferEx2(
        CompressionFormatAndEngine CompressionFormatAndEngine,
        byte* UncompressedBuffer,
        int UncompressedBufferSize,
        byte* CompressedBuffer,
        int CompressedBufferSize,
        out int FinalUncompressedSize,
        byte* WorkSpace);

    [DllImport("ntdll.dll", SetLastError = false)]
    private static extern int RtlGetCompressionWorkSpaceSize(
      CompressionFormatAndEngine CompressionFormatAndEngine,
      out int CompressBufferWorkSpaceSize,
      out int CompressFragmentWorkSpaceSize);

    public static int GetCompressBufferWorkSpaceSize(CompressionFormatAndEngine CompressionFormatAndEngine)
    {
        Win32API.ThrowOnNtStatusFailed(RtlGetCompressionWorkSpaceSize(CompressionFormatAndEngine, out var CompressBufferWorkSpaceSize, out var _));

        return CompressBufferWorkSpaceSize;
    }

    public static int GetCompressFragmentWorkSpaceSize(CompressionFormatAndEngine CompressionFormatAndEngine)
    {
        Win32API.ThrowOnNtStatusFailed(RtlGetCompressionWorkSpaceSize(CompressionFormatAndEngine, out var _, out var CompressFragmentWorkSpaceSize));

        return CompressFragmentWorkSpaceSize;
    }

    public static byte[] CreateCompressBufferWorkSpace(CompressionFormatAndEngine CompressionFormatAndEngine) => new byte[GetCompressBufferWorkSpaceSize(CompressionFormatAndEngine)];

    public static byte[] CreateCompressFragmentWorkSpace(CompressionFormatAndEngine CompressionFormatAndEngine) => new byte[GetCompressFragmentWorkSpaceSize(CompressionFormatAndEngine)];

    public static int CompressBuffer(CompressionFormatAndEngine CompressionFormatAndEngine, byte[] uncompressedBuffer, int uncompressedIndex, int uncompressedCount, byte[] compressedBuffer, int compressedIndex, byte[]? workSpace)
    {
        if (uncompressedCount + uncompressedIndex > uncompressedBuffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(uncompressedCount));
        }

        if ((workSpace?.Length ?? 0) < GetCompressBufferWorkSpaceSize(CompressionFormatAndEngine))
        {
            throw new ArgumentException("Invalid buffer size", nameof(workSpace));
        }

        int final_size;

        fixed (byte*
            uncompressedPtr = &uncompressedBuffer[uncompressedIndex],
            compressedPtr = &compressedBuffer[compressedIndex],
            workSpacePtr = workSpace)
        {
            Win32API.ThrowOnNtStatusFailed(RtlCompressBuffer(CompressionFormatAndEngine, uncompressedPtr, uncompressedCount, compressedPtr, compressedBuffer.Length - compressedIndex, 4096, out final_size, workSpacePtr));
        }

        return final_size;
    }

    public static int DecompressBuffer(CompressionFormatAndEngine CompressionFormatAndEngine, byte[] compressedBuffer, int compressedIndex, int compressedCount, byte[] uncompressedBuffer, int uncompressedIndex, int uncompressedCount, byte[] workSpace, bool throwOnDecoderError)
    {
        if (compressedCount + compressedIndex > compressedBuffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(compressedCount));
        }

        if (uncompressedCount + uncompressedIndex > uncompressedBuffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(uncompressedCount));
        }

        if (workSpace is null || workSpace.Length < GetCompressBufferWorkSpaceSize(CompressionFormatAndEngine))
        {
            throw new ArgumentException("Invalid workspace buffer", nameof(workSpace));
        }

        int final_size;

        int status;

        fixed (byte*
            uncompressedPtr = &uncompressedBuffer[uncompressedIndex],
            compressedPtr = &compressedBuffer[compressedIndex],
            workSpacePtr = workSpace)
        {
            status = RtlDecompressBufferEx(CompressionFormatAndEngine, uncompressedPtr, uncompressedCount, compressedPtr, compressedCount, out final_size, workSpacePtr);
        }

        if (throwOnDecoderError)
        {
            Win32API.ThrowOnNtStatusFailed(status);
        }

        if (status < 0)
        {
            return status;
        }
        else
        {
            return final_size;
        }
    }
}

#endif
