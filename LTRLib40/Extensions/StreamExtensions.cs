using System;
using System.IO;

namespace LTRLib.Extensions;

public static class StreamExtensions
{

    /// <summary>
    /// Reads specified number of bytes from a stream and returns read data as a new byte
    /// array.
    /// </summary>
    /// <param name="Stream">Stream to read from</param>
    /// <param name="count">Number of bytes to read</param>
    public static byte[] Read(this Stream Stream, int count)
    {
        var buffer = new byte[count];
        var readlength = Stream.Read(buffer, 0, count);
        Array.Resize(ref buffer, readlength);
        return buffer;
    }

    /// <summary>
    /// Reads remaining bytes from a stream and returns read data as a new byte array.
    /// </summary>
    /// <param name="Stream">Stream to read from</param>
    public static byte[] ReadToEnd(this Stream Stream) => Stream.Read((int)(Stream.Length - Stream.Position));

    /// <summary>
    /// Writes out a byte array to a stream.
    /// </summary>
    /// <param name="Stream">Stream to write to</param>
    /// <param name="buffer">Bytes to write</param>
    public static void Write(this Stream Stream, byte[] buffer) => Stream.Write(buffer, 0, buffer.Length);

#if NETFRAMEWORK && !NET40_OR_GREATER

    /// <summary>
    /// Copies all remaining data from a stream to another stream using a 1 MB buffer.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    public static void CopyTo(this Stream SourceStream, Stream TargetStream) => IO.IOSupport.CopyStream(SourceStream, TargetStream);

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    /// <param name="BufferSize">Size of buffer to use when reading/writing.</param>
    public static void CopyTo(this Stream SourceStream, Stream TargetStream, int BufferSize) => IO.IOSupport.CopyStream(SourceStream, TargetStream, BufferSize);

    /// <summary>
    /// Copies data from a stream to another stream.
    /// </summary>
    /// <param name="SourceStream">Input stream to copy from.</param>
    /// <param name="TargetStream">Output stream to copy to.</param>
    /// <param name="BufferSize">Size of buffer to use when reading/writing.</param>
    /// <param name="Length">In: Maximum total length to copy or -1 for all
    /// of input stream. Out: Number of bytes actually copied.</param>
    public static void CopyTo(this Stream SourceStream, Stream TargetStream, int BufferSize, ref int Length) => IO.IOSupport.CopyStream(SourceStream, TargetStream, BufferSize, ref Length);

#endif
}
