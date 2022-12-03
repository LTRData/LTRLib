/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.IO;
using System.Threading;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif

namespace LTRLib.IO;

public class ZeroStream : Stream
{
    public ZeroStream()
    {
    }

    public ZeroStream(long length, bool writable)
    {
        _length = length;
        CanWrite = writable;
    }

    public override bool CanRead => true;

    public override bool CanSeek => Length >= 0;

    public override bool CanWrite
    {
        get;
    } = true;

    private long _length = -1;

    public override long Length => _length;

    public override long Position
    {
        get;
        set;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (Length >= 0)
        {
            if (Position >= Length)
            {
                return count;
            }

            if (Position + count > Length)
            {
                count = (int)(Length - Position);
            }

            Position += count;
        }

        FillBuffer(buffer, offset, count);

        return count;
    }

    public virtual void FillBuffer(byte[] buffer, int offset, int count) => Array.Clear(buffer, offset, count);

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        Task.FromResult(Read(buffer, offset, count));
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override int Read(Span<byte> buffer)
    {
        if (Length >= 0)
        {
            if (Position >= Length)
            {
                return buffer.Length;
            }

            if (Position + buffer.Length > Length)
            {
                buffer = buffer[..(int)(Length - Position)];
            }

            Position += buffer.Length;
        }

        FillBuffer(buffer);

        return buffer.Length;
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        new(Read(buffer.Span));

    public virtual void FillBuffer(Span<byte> buffer) => buffer.Clear();
#endif

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;

            case SeekOrigin.Current:
                Position += offset;
                break;

            case SeekOrigin.End:
                Position = Length + offset;
                break;
        }

        return Position;
    }

    public override void SetLength(long value) => _length = value;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (!CanWrite)
        {
            throw new IOException("Stream is not writable");
        }

        if (Length < 0)
        {
            return;
        }

        Position += count;

        if (Position > Length)
        {
            SetLength(Position);
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        Write(buffer, offset, count);
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
        return Task.CompletedTask;
#else
        return Task.FromResult(0);
#endif
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (!CanWrite)
        {
            throw new IOException("Stream is not writable");
        }

        if (Length < 0)
        {
            return;
        }

        Position += buffer.Length;

        if (Position > Length)
        {
            SetLength(Position);
        }
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        Write(buffer.Span);
        return new();
    }
#endif
}
