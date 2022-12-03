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

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'

namespace LTRLib.IO;

public class SeekStreamSegment : Stream, IHasPhysicalPosition
{
    public Stream BaseStream { get; }

    public long StartPosition { get; }

    public override long Length { get; }

    public int AlignmentMask { get; } = 0;

    public byte AlignmentBits { get; } = 0;

    private readonly bool disposeBase;

    public SeekStreamSegment(Stream baseStream, long startPosition, long maxLength, int alignmentRequirement, bool ownsBaseStream)
    {
        BaseStream = baseStream;
        StartPosition = startPosition;

        alignmentRequirement--;
        while (alignmentRequirement > 0)
        {
            AlignmentBits++;
            alignmentRequirement >>= 1;
        }

        AlignmentMask = (1 << AlignmentBits) - 1;

        Length = baseStream.Length - startPosition < maxLength ? baseStream.Length - startPosition : maxLength;

        disposeBase = ownsBaseStream;

        Position = 0;
    }

    public override int Read(byte[] buffer, int index, int count)
    {
        if (Position < 0 || Position >= Length)
        {
            return 0;
        }

        if (Position + count > Length)
        {
            count = (int)(Length - Position);
        }

        if (AlignmentMask == 0 ||
            positionOffset == 0 &&
            (count & AlignmentMask) == 0)
        {
            return BaseStream.Read(buffer, index, count);
        }

        //var aligned_position = BaseStream.Position & ~AlignmentMask;

        var aligned_offset = positionOffset;

        var aligned_count = (count + aligned_offset + AlignmentMask) & ~AlignmentMask;

        var aligned_buffer = new byte[aligned_count];

        var res = BaseStream.Read(aligned_buffer, 0, aligned_count);

        var diff = res >= aligned_offset ? res - aligned_offset - count : 0;

        if (diff < 0)
        {
            count += diff;
        }
        else if (diff > 0)
        {
            Position -= diff;
        }

        Buffer.BlockCopy(aligned_buffer, aligned_offset, buffer, index, count);

        return count;
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override Task<int> ReadAsync(byte[] buffer, int index, int count, CancellationToken cancellationToken)
    {
        if (Position < 0 || Position >= Length)
        {
            return Task.FromResult(0);
        }

        if (Position + count > Length)
        {
            count = (int)(Length - Position);
        }

        if (AlignmentMask == 0 ||
            positionOffset == 0 &&
            (count & AlignmentMask) == 0)
        {
            return BaseStream.ReadAsync(buffer, index, count, cancellationToken);
        }
        else
        {
            return ReadAsyncInternal(buffer, index, count, cancellationToken);
        }
    }

    protected virtual async Task<int> ReadAsyncInternal(byte[] buffer, int index, int count, CancellationToken cancellationToken)
    {
        //var aligned_position = BaseStream.Position;

        var aligned_offset = positionOffset;

        var aligned_count = (count + aligned_offset + AlignmentMask) & ~AlignmentMask;

        var aligned_buffer = new byte[aligned_count];

        var res = await BaseStream.ReadAsync(aligned_buffer, 0, aligned_count, cancellationToken);

        var diff = res >= aligned_offset ? res - aligned_offset - count : 0;

        if (diff < 0)
        {
            count += diff;
        }
        else if (diff > 0)
        {
            Position -= diff;
        }

        Buffer.BlockCopy(aligned_buffer, aligned_offset, buffer, index, count);

        return count;
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var index = 0;
        var count = buffer.Length;

        if (Position < 0 || Position >= Length)
        {
            return 0;
        }

        if (Position + count > Length)
        {
            count = (int)(Length - Position);
        }

        if (AlignmentMask == 0 ||
            positionOffset == 0 &&
            (count & AlignmentMask) == 0)
        {
            return await BaseStream.ReadAsync(buffer.Slice(index, count), cancellationToken);
        }
        else
        {
            return await ReadAsyncInternal(buffer.Slice(index, count), cancellationToken);
        }
    }

    protected virtual async ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        //var aligned_position = BaseStream.Position;

        var count = buffer.Length;

        var aligned_offset = positionOffset;

        var aligned_count = (count + aligned_offset + AlignmentMask) & ~AlignmentMask;

        var aligned_buffer = new byte[aligned_count];

        var res = await BaseStream.ReadAsync(aligned_buffer, 0, aligned_count, cancellationToken);

        var diff = res >= aligned_offset ? res - aligned_offset - count : 0;

        if (diff < 0)
        {
            count += diff;
        }
        else if (diff > 0)
        {
            Position -= diff;
        }

        aligned_buffer.AsMemory(aligned_offset, count).CopyTo(buffer);

        return count;
    }

    public override int Read(Span<byte> buffer)
    {
        var index = 0;
        var count = buffer.Length;

        if (Position < 0 || Position >= Length)
        {
            return 0;
        }

        if (Position + count > Length)
        {
            count = (int)(Length - Position);
        }

        if (AlignmentMask == 0 ||
            positionOffset == 0 &&
            (count & AlignmentMask) == 0)
        {
            return BaseStream.Read(buffer.Slice(index, count));
        }
        else
        {
            return ReadInternal(buffer.Slice(index, count));
        }
    }

    protected virtual int ReadInternal(Span<byte> buffer)
    {
        //var aligned_position = BaseStream.Position;

        var count = buffer.Length;

        var aligned_offset = positionOffset;

        var aligned_count = (count + aligned_offset + AlignmentMask) & ~AlignmentMask;

        var aligned_buffer = new byte[aligned_count];

        var res = BaseStream.Read(aligned_buffer, 0, aligned_count);

        var diff = res >= aligned_offset ? res - aligned_offset - count : 0;

        if (diff < 0)
        {
            count += diff;
        }
        else if (diff > 0)
        {
            Position -= diff;
        }

        aligned_buffer.AsSpan(aligned_offset, count).CopyTo(buffer);

        return count;
    }
#endif

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (Position < 0 || Position >= Length)
        {
            throw new EndOfStreamException();
        }

        BaseStream.Position += positionOffset;

        BaseStream.Write(buffer, offset, count);
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (Position < 0 || Position >= Length)
        {
            throw new EndOfStreamException();
        }

        BaseStream.Position += positionOffset;

        return BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (Position < 0 || Position >= Length)
        {
            throw new EndOfStreamException();
        }

        BaseStream.Position += positionOffset;

        return BaseStream.WriteAsync(buffer, cancellationToken);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (Position < 0 || Position >= Length)
        {
            throw new EndOfStreamException();
        }

        BaseStream.Position += positionOffset;

        BaseStream.Write(buffer);
    }
#endif

    public override void Flush() => BaseStream.Flush();

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override Task FlushAsync(CancellationToken cancellationToken) => BaseStream.FlushAsync(cancellationToken);
#endif

    public override long Position
    {
        get => BaseStream.Position - StartPosition + positionOffset;
        set => Seek(value, SeekOrigin.Begin);
    }

    private int positionOffset;

    public long? PhysicalPosition
    {
        get
        {
            if (BaseStream is IHasPhysicalPosition hasPhysicalPosition)
            {
                return hasPhysicalPosition.PhysicalPosition + positionOffset;
            }
            else
            {
                return BaseStream.Position + positionOffset;
            }
        }
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override bool CanRead => BaseStream.CanRead;

    public override bool CanSeek => BaseStream.CanSeek;

    public override bool CanWrite => BaseStream.CanWrite;

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                offset += StartPosition;
                break;

            case SeekOrigin.Current:
                offset += BaseStream.Position;
                break;

            case SeekOrigin.End:
                offset += StartPosition + Length;
                break;
        }

        if (offset == BaseStream.Position)
        {
            return offset - StartPosition;
        }

        if (offset - StartPosition < 0)
        {
            throw new ArgumentException("Negative stream positions not supported");
        }

        BaseStream.Position = offset & ~AlignmentMask;

        positionOffset = (int)(offset & AlignmentMask);

        return offset - StartPosition;
    }

    public override void Close()
    {
        if (disposeBase)
        {
            BaseStream?.Close();
        }

        base.Close();
    }
}
