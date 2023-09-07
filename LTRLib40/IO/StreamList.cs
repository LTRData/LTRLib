// LTRLib.IO.StreamList
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if NET35_OR_GREATER || NETCOREAPP || NETSTANDARD
using System.Linq;
#endif
using System.Threading;
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD
using System.Threading.Tasks;
#endif
using LTRLib.LTRGeneric;

namespace LTRLib.IO;

public class StreamList : Stream, IList<Stream>
{
    private readonly DisposableList<Stream> List;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public int Count => List.Count;

    public bool IsReadOnly => false;

    public Stream this[int index]
    {
        get => List[index];
        set => List[index] = value;
    }

    public StreamList()
    {
        List = new DisposableList<Stream>();
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD
    public override void WriteByte(byte value) =>
        Parallel.ForEach(List, item => item.WriteByte(value));

    public override void Write(byte[] buffer, int offset, int count) =>
        Parallel.ForEach(List, item => item.Write(buffer, offset, count));

    public override void Flush() =>
        Parallel.ForEach(List, item => item.Flush());
#else
    public override void WriteByte(byte value) =>
        List.ForEach(item => item.WriteByte(value));

    public override void Write(byte[] buffer, int offset, int count) =>
        List.ForEach(item => item.Write(buffer, offset, count));

    public override void Flush() =>
        List.ForEach(item => item.Flush());
#endif

#if NET45_OR_GREATER || NETCOREAPP || NETSTANDARD
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        Task.WhenAll(List.Select(item => item.WriteAsync(buffer, offset, count, cancellationToken)));

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        WriteAsync(buffer, offset, count, CancellationToken.None).AsAsyncResult(callback, state);

    public override void EndWrite(IAsyncResult asyncResult) =>
        ((Task)asyncResult).Wait();

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        Task.WhenAll(List.Select(item => item.FlushAsync(cancellationToken)));
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        foreach (var item in List)
        {
            item.Write(buffer);
        }
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        new(Task.WhenAll(List.Select(item => item.WriteAsync(buffer, cancellationToken).AsTask())));

    public override async ValueTask DisposeAsync()
    {
        await Task.WhenAll(List.Select(item => item.DisposeAsync().AsTask())).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
#endif

    public void Add(Stream item) => List.Add(item);

    public void Clear() => List.Dispose();

    public bool Contains(Stream item) => List.Contains(item);

    public void CopyTo(Stream[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);

    public bool Remove(Stream item)
    {
        item.Close();
        return List.Remove(item);
    }

    public IEnumerator<Stream> GetEnumerator() => List.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

    public int IndexOf(Stream item) => List.IndexOf(item);

    public void Insert(int index, Stream item) => List.Insert(index, item);

    public void RemoveAt(int index)
    {
        List[index].Close();
        List.RemoveAt(index);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            List.Dispose();
        }

        List.Clear();
        base.Dispose(disposing);
    }
}
