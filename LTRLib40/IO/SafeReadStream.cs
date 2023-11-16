/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */

using LTRLib.Extensions;
using LTRLib.LTRGeneric;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
#if NET45_OR_GREATER || NETCOREAPP || NETSTANDARD
using LTRData.Extensions.Async;
using System.Threading.Tasks;
#endif

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'

namespace LTRLib.IO;

public class SafeReadStream(Stream baseStream) : Stream
{
    public Stream BaseStream { get; } = baseStream ?? throw new ArgumentNullException(nameof(baseStream));

    public override bool CanRead => BaseStream.CanRead;

    public override bool CanSeek => BaseStream.CanSeek;

    public override bool CanWrite => BaseStream.CanWrite;

    public override long Length => BaseStream.Length;

    public override long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    public override int ReadTimeout
    {
        get => BaseStream.ReadTimeout;
        set => BaseStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => BaseStream.WriteTimeout;
        set => BaseStream.WriteTimeout = value;
    }

    public override bool CanTimeout => BaseStream.CanTimeout;

    public override int Read(byte[] buffer, int offset, int count)
    {
        var totalSize = 0;
        while (count > 0)
        {
            var blockSize = BaseStream.Read(buffer, offset, count);
            if (blockSize == 0)
            {
                break;
            }

            count -= blockSize;
            offset += blockSize;
            totalSize += blockSize;
        }

        return totalSize;
    }

#if NET45_OR_GREATER || NETCOREAPP || NETSTANDARD
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var totalSize = 0;
        while (count > 0)
        {
            var blockSize = await BaseStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            if (blockSize == 0)
            {
                break;
            }

            count -= blockSize;
            offset += blockSize;
            totalSize += blockSize;
        }

        return totalSize;
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        BaseStream.ReadAsync(buffer, offset, count, CancellationToken.None).AsAsyncResult(callback, state);

    public override int EndRead(IAsyncResult asyncResult) =>
        ((Task<int>)asyncResult).Result;
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override int Read(Span<byte> buffer)
    {
        var totalSize = 0;
        while (buffer.Length > 0)
        {
            var blockSize = BaseStream.Read(buffer);
            if (blockSize == 0)
            {
                break;
            }

            buffer = buffer[blockSize..];
            totalSize += blockSize;
        }

        return totalSize;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var totalSize = 0;
        while (buffer.Length > 0)
        {
            var blockSize = await BaseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (blockSize == 0)
            {
                break;
            }

            buffer = buffer[blockSize..];
            totalSize += blockSize;
        }

        return totalSize;
    }
#endif

    public override void Flush() =>
        BaseStream.Flush();

    public override long Seek(long offset, SeekOrigin origin) =>
        BaseStream.Seek(offset, origin);

    public override void SetLength(long value) =>
        BaseStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) =>
        BaseStream.Write(buffer, offset, count);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        BaseStream.BeginWrite(buffer, offset, count, callback, RuntimeHelpers.GetObjectValue(state));

    public override void EndWrite(IAsyncResult asyncResult) =>
        BaseStream.EndWrite(asyncResult);

#if NET45_OR_GREATER || NETCOREAPP || NETSTANDARD
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        BaseStream.WriteAsync(buffer, offset, count, cancellationToken);

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        BaseStream.FlushAsync(cancellationToken);
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override void Write(ReadOnlySpan<byte> buffer) =>
        BaseStream.Write(buffer);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        BaseStream.WriteAsync(buffer, cancellationToken);

    public override async ValueTask DisposeAsync()
    {
        await BaseStream.DisposeAsync();
        GC.SuppressFinalize(this);
    }
#endif

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BaseStream.Close();
        }
    }

    public override int ReadByte() =>
        BaseStream.ReadByte();

    public override void WriteByte(byte value) =>
        BaseStream.WriteByte(value);
}
