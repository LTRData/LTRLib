#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LTRLib.Extensions;

public static class PipelineExtensions
{
    public static async ValueTask<KeyValuePair<string, string?>> ReadKeyValueLineAsync(this PipeReader pipeReader, CancellationToken cancellationToken)
    {
        ReadResult readresult;
        SequencePosition? line_break_position;
        ReadOnlySequence<byte> buffer;

        for (; ; )
        {
            readresult = await pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

            buffer = readresult.Buffer;

            line_break_position = buffer.PositionOf((byte)'\n');

            if (line_break_position.HasValue)
            {
                break;
            }

            if (readresult.IsCompleted)
            {
                return default;
            }

            pipeReader.AdvanceTo(buffer.Start, buffer.End);
        }

        var endpos = buffer.PositionOf((byte)'\r');
        if (!endpos.HasValue)
        {
            endpos = line_break_position;
        }        

        var slice = buffer.Slice(buffer.Start, endpos.Value);

        string key;
        string? data;

        var delim = slice.PositionOf((byte)':');
        if (delim.HasValue && slice.Slice(0, delim.Value).Length > 0)
        {
#if NET5_0_OR_GREATER
            key = Encoding.UTF8.GetString(slice.Slice(0, delim.Value));
            data = Encoding.UTF8.GetString(slice.Slice(slice.GetPosition(2, delim.Value)));
#else
            key = Encoding.UTF8.GetString(slice.Slice(0, delim.Value).ToArray());
            data = Encoding.UTF8.GetString(slice.Slice(slice.GetPosition(2, delim.Value)).ToArray());
#endif
        }
        else
        {
#if NET5_0_OR_GREATER
            key = Encoding.UTF8.GetString(slice);
            data = null;
#else
            key = Encoding.UTF8.GetString(slice.ToArray());
            data = null;
#endif
        }

        pipeReader.AdvanceTo(buffer.GetPosition(1, line_break_position.Value), buffer.End);

        return new(key, data);
    }

    public static bool SequenceEqual(this ReadOnlySequence<byte> tokenSlice, ReadOnlySpan<byte> token)
    {
        var offset = 0;

        foreach (var memory in tokenSlice)
        {
            foreach (var b in memory.Span)
            {
                if (b != token[offset++])
                {
                    return false;
                }
            }
        }

        return true;
    }
}

#endif
