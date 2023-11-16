#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LTRLib.LTRGeneric;

public class GenericAsyncQueryable<TElement> : GenericQueryable<TElement>, IAsyncEnumerable<TElement?>
{
    public GenericAsyncQueryable()
    {
    }

    public GenericAsyncQueryable(IQueryProvider context) : base(context)
    {
    }

    public GenericAsyncQueryable(IQueryProvider context, Expression expression) : base(context, expression)
    {
    }

    public IAsyncEnumerator<TElement?> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        if (Provider is not IAsyncAggregationQueryProvider asyncAggregationQueryProvider)
        {
            throw new NotSupportedException($"Query provider '{Provider.GetType().Name}' does not implement interface IAsyncAggregationQueryProvider.");
        }

        return new GenericAsyncEnumerator(Expression, asyncAggregationQueryProvider, cancellationToken);
    }

    private sealed class GenericAsyncEnumerator(Expression expression,
                                                IAsyncAggregationQueryProvider provider,
                                                CancellationToken cancellationToken) : IAsyncEnumerator<TElement?>
    {
        private IEnumerator<TElement>? elements;

        public TElement? Current => elements is null ? default : elements.Current;

        public ValueTask DisposeAsync()
        {
            elements?.Dispose();
            return default;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (elements is null)
            {
                var enumerable = await provider.ExecuteAsync<IEnumerable<TElement>>(expression, cancellationToken).ConfigureAwait(false);
                elements = enumerable.GetEnumerator();
            }

            return elements.MoveNext();
        }
    }
}

public interface IAsyncAggregationQueryProvider : IQueryProvider
{
    Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
}

#endif
