using System;
using System.IO;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading;
using System.Threading.Tasks;
#else
using LTRLib.Extensions;
#endif

namespace LTRLib.IO;

/// <summary>
/// Buffers an entire stream in memory. A readable base stream is read into memory by constructor.
/// For a writable base stream, buffers all writes in memory and write out everything when this
/// object is closed or disposed.
/// </summary>
/// <remarks></remarks>
public class BufferedStreamEx : MemoryStream
{
    private readonly Stream _baseStream;

    public BufferedStreamEx(Stream BaseStream)
        : base(capacity: BaseStream is not null
            && BaseStream.CanSeek && BaseStream.CanRead
            ? (int)BaseStream.Length : 0)
    {
        if (BaseStream is null)
        {
            throw new ArgumentNullException(nameof(BaseStream));
        }

        _baseStream = BaseStream;

        if (_baseStream.CanRead)
        {
            if (_baseStream.CanSeek)
            {
                _baseStream.Position = 0L;
            }

            _baseStream.CopyTo(this);
        }

        if (!_baseStream.CanWrite)
        {
            _baseStream.Dispose();
            _baseStream = null!;
        }
    }

    public Stream BaseStream => _baseStream;

    public override bool CanWrite => _baseStream is not null;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_baseStream is null)
        {
            throw new IOException("Base stream is not writable");
        }

        base.Write(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
        if (_baseStream is null)
        {
            throw new IOException("Base stream is not writable");
        }

        base.WriteByte(value);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        if (_baseStream is null)
        {
            throw new IOException("Base stream is not writable");
        }

        return base.BeginWrite(buffer, offset, count, callback, state);
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (_baseStream is null)
        {
            throw new IOException("Base stream is not writable");
        }

        return base.WriteAsync(buffer, offset, count, cancellationToken);
    }
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_baseStream is null)
        {
            throw new IOException("Base stream is not writable");
        }

        return base.WriteAsync(buffer, cancellationToken);
    }
#endif

    protected override void Dispose(bool disposing)
    {
        if (disposing && _baseStream is not null)
        {
            if (_baseStream.CanSeek)
            {
                _baseStream.Position = 0L;
            }

            WriteTo(_baseStream);
            if (_baseStream.CanSeek)
            {
                _baseStream.SetLength(_baseStream.Position);
            }

            _baseStream.Close();
        }

        base.Dispose(disposing);
    }

}