#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LTRLib.LTRGeneric;

public class QueryHolder<TRecord>
{
    private Func<CancellationToken, Task<IEnumerable<TRecord>?>>? _queryfunc;

    public QueryHolder()
    {
    }

    public QueryHolder(IEnumerable<TRecord>? query)
    {
        _queryfunc = _ => Task.FromResult(query);
    }

    public QueryHolder(Task<IEnumerable<TRecord>?> query)
    {
        _queryfunc = _ => query;
    }

    public QueryHolder(Func<CancellationToken, IEnumerable<TRecord>?>? query)
    {
        _queryfunc = query is not null
            ? cancellationToken => Task.Run(() => query(cancellationToken), cancellationToken)
            : null;
    }

    public QueryHolder(Func<CancellationToken, Task<IEnumerable<TRecord>?>>? query)
    {
        _queryfunc = query;
    }

    public virtual IEnumerable<TRecord>? Query
    {
        set => _queryfunc = _ => Task.FromResult(value);
    }

    public virtual Task<IEnumerable<TRecord>?> QueryTask
    {
        set => _queryfunc = _ => value;
    }

    public virtual Func<CancellationToken, IEnumerable<TRecord>?>? QueryFunc
    {
        set => _queryfunc = value is not null
            ? cancellationToken => Task.Run(() => value(cancellationToken), cancellationToken)
            : null;
    }

    public virtual Func<CancellationToken, Task<IEnumerable<TRecord>?>> QueryTaskFunc
    {
        set => _queryfunc = value;
    }

    public virtual Task<TRecord[]?> ToArrayAsync() => ToArrayAsync(CancellationToken.None);

    public async Task<TRecord[]?> ToArrayAsync(CancellationToken cancellationToken)
    {
        if (_queryfunc is null)
        {
            throw new InvalidOperationException("No query function defined");
        }

        var enumerable = await _queryfunc(cancellationToken).ConfigureAwait(false);
        
        if (enumerable is TRecord[] array)
        {
            return array;
        }
        else if (enumerable is IQueryable<TRecord> query && query.Provider is IAsyncQueryProvider)
        {
            return await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);
        }
        
        return enumerable?.ToArray();
    }

    public virtual Task<List<TRecord>?> ToListAsync() => ToListAsync(CancellationToken.None);

    public async Task<List<TRecord>?> ToListAsync(CancellationToken cancellationToken)
    {
        if (_queryfunc is null)
        {
            throw new InvalidOperationException("No query function defined");
        }

        var enumerable = await _queryfunc(cancellationToken).ConfigureAwait(false);

        if (enumerable is List<TRecord> array)
        {
            return array;
        }
        else if (enumerable is IQueryable<TRecord> query && query.Provider is IAsyncQueryProvider)
        {
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        return enumerable?.ToList();
    }

    public Task<IEnumerable<TRecord>?> ToEnumerableAsync() => ToEnumerableAsync(CancellationToken.None);

    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    public async Task<IEnumerable<TRecord>?> ToEnumerableAsync(CancellationToken cancellationToken)
    {
        if (_queryfunc is null)
        {
            throw new InvalidOperationException("No query function defined");
        }

        var enumerable = await _queryfunc(cancellationToken).ConfigureAwait(false);

        if (enumerable is IQueryable<TRecord> query && query.Provider is IAsyncQueryProvider queryProvider)
        {
            return await queryProvider.ExecuteAsync<Task<IEnumerable<TRecord>>>(query.Expression, cancellationToken).ConfigureAwait(false);
        }

        return enumerable;
    }
    public ValueTask<IAsyncEnumerable<TRecord>> ToAsyncEnumerableAsync()
        => ToAsyncEnumerableAsync(CancellationToken.None);

    public async ValueTask<IAsyncEnumerable<TRecord>> ToAsyncEnumerableAsync(CancellationToken cancellationToken)
    {
        if (_queryfunc is null)
        {
            throw new InvalidOperationException("No query function defined");
        }

        var enumerable = await _queryfunc(cancellationToken).ConfigureAwait(false);

        if (enumerable is IQueryable<TRecord> query && query.Provider is IAsyncQueryProvider)
        {
            return query.AsAsyncEnumerable();
        }

        throw new NotSupportedException($"Query provider does not implement IAsyncQueryProvider");
    }
}

#endif
