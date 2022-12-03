/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Collections.Generic;
using System.IO;
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif

namespace LTRLib.IO;

public class CombinedTextReader : TextReader
{
    private IEnumerator<TextReader>? _enum;

    public CombinedTextReader(params TextReader[] readers)
        : this(readers as IEnumerable<TextReader>)
    {
    }

    public CombinedTextReader(IEnumerable<TextReader> readers)
    {
        try
        {
            _enum = readers?.GetEnumerator();

            if (_enum is not null && !_enum.MoveNext())
            {
                _enum.Dispose();
                _enum = null;
            }
        }
        catch (Exception ex)
        {
            _enum?.Dispose();
            _enum = null;

            throw new Exception("Enumeration of source readers failed", ex);
        }
    }

    public override int Read()
    {
        while (_enum is not null)
        {
            var c = _enum.Current.Read();

            if (c >= 0)
            {
                return c;
            }
            else
            {
                _enum.Current.Dispose();

                if (!_enum.MoveNext())
                {
                    _enum.Dispose();
                    _enum = null;
                }
            }
        }

        return base.Read();
    }

    public override int Read(char[] buffer, int index, int count)
    {
        var num = 0;

        while (_enum is not null && count > 0)
        {
            var r = _enum.Current.Read(buffer, index, count);

            num += r;
            index += r;
            count -= r;

            if (count <= 0)
            {
                break;
            }

            _enum.Current.Close();

            if (!_enum.MoveNext())
            {
                _enum.Dispose();
                _enum = null;
            }
        }

        return num;
    }

    public override int ReadBlock(char[] buffer, int index, int count)
    {
        var num = 0;

        while (_enum is not null && count > 0)
        {
            var r = _enum.Current.ReadBlock(buffer, index, count);

            num += r;
            index += r;
            count -= r;

            if (count <= 0)
            {
                break;
            }

            _enum.Current.Close();

            if (!_enum.MoveNext())
            {
                _enum.Dispose();
                _enum = null;
            }
        }

        return num;
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public override async Task<int> ReadAsync(char[] buffer, int index, int count)
    {
        var num = 0;

        while (_enum is not null && count > 0)
        {
            var r = await _enum.Current.ReadAsync(buffer, index, count).ConfigureAwait(false);

            num += r;
            index += r;
            count -= r;

            if (count <= 0)
            {
                break;
            }

            _enum.Current.Close();

            if (!_enum.MoveNext())
            {
                _enum.Dispose();
                _enum = null;
            }
        }

        return num;
    }

    public override async Task<int> ReadBlockAsync(char[] buffer, int index, int count)
    {
        var num = 0;

        while (_enum is not null && count > 0)
        {
            var r = await _enum.Current.ReadBlockAsync(buffer, index, count).ConfigureAwait(false);

            num += r;
            index += r;
            count -= r;

            if (count <= 0)
            {
                break;
            }

            _enum.Current.Close();

            if (!_enum.MoveNext())
            {
                _enum.Dispose();
                _enum = null;
            }
        }

        return num;
    }
#endif

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _enum?.Current?.Close();
            _enum?.Dispose();
        }

        _enum = null;

        base.Dispose(disposing);
    }
}
