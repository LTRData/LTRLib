/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif

namespace LTRLib.IO;

public class CombinedInputStream : Stream, IHasPhysicalPosition
{
    private IEnumerator<Stream>? _enum;

    public CombinedInputStream(params Stream[] inputStreams)
        : this(inputStreams as IEnumerable<Stream>)
    {
    }

    public CombinedInputStream(IEnumerable<Stream> inputStreams)
    {
        _enum = inputStreams?.GetEnumerator();
    }

    public override int Read(byte[] buffer, int index, int count)
    {
        var num = 0;

        while (_enum is not null && count > 0)
        {
            var r = _enum.Current?.Read(buffer, index, count) ?? 0;

            if (r > 0)
            {
                _position += r;

                num += r;
                index += r;
                count -= r;
            }
            else
            {
                _enum.Current?.Close();

                if (!_enum.MoveNext())
                {
                    _enum.Dispose();
                    _enum = null;
                }
            }
        }

        return num;
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override async Task<int> ReadAsync(byte[] buffer, int index, int count, CancellationToken cancellationToken)
    {
        var num = 0;

        while (_enum is not null && count > 0)
        {
            var r = 0;

            if (_enum.Current is not null)
            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
                r = await _enum.Current.ReadAsync(buffer.AsMemory(index, count), cancellationToken).ConfigureAwait(false);
#else
                r = await _enum.Current.ReadAsync(buffer, index, count, cancellationToken).ConfigureAwait(false);
#endif
            }

            if (r > 0)
            {
                _position += r;

                num += r;
                index += r;
                count -= r;
            }
            else
            {
                _enum.Current?.Close();

                if (!_enum.MoveNext())
                {
                    _enum.Dispose();
                    _enum = null;
                }
            }
        }

        return num;
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var num = 0;
        var index = 0;
        var count = buffer.Length;

        while (_enum is not null && count > 0)
        {
            var r = 0;

            if (_enum.Current is not null)
            {
                r = await _enum.Current.ReadAsync(buffer.Slice(index, count), cancellationToken).ConfigureAwait(false);
            }

            if (r > 0)
            {
                _position += r;

                num += r;
                index += r;
                count -= r;
            }
            else
            {
                _enum.Current?.Close();

                if (!_enum.MoveNext())
                {
                    _enum.Dispose();
                    _enum = null;
                }
            }
        }

        return num;
    }

    public override int Read(Span<byte> buffer)
    {
        var num = 0;
        var index = 0;
        var count = buffer.Length;

        while (_enum is not null && count > 0)
        {
            var r = 0;

            if (_enum.Current is not null)
            {
                r = _enum.Current.Read(buffer.Slice(index, count));
            }

            if (r > 0)
            {
                _position += r;

                num += r;
                index += r;
                count -= r;
            }
            else
            {
                _enum.Current?.Close();

                if (!_enum.MoveNext())
                {
                    _enum.Dispose();
                    _enum = null;
                }
            }
        }

        return num;
    }
#endif

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void Flush() => throw new NotSupportedException();

    private long _position;

    public override long Position
    {
        get => _position;
        set
        {
            if (value != _position)
            {
                throw new NotSupportedException();
            }
        }
    }

    public long? PhysicalPosition
    {
        get
        {
            var stream = _enum?.Current;
            return stream switch
            {
                IHasPhysicalPosition posstream => posstream.PhysicalPosition,
                _ => stream?.Position,
            };
        }
    }

    public override long Length => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void Close()
    {
        _enum?.Current?.Close();

        _enum?.Dispose();

        _enum = null;

        base.Close();
    }
}
