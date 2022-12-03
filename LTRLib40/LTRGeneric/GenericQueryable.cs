// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

/// <summary>
/// Represents a Linq query that uses specified IQueryProvider to execute queries.
/// </summary>
/// <typeparam name="TElement">An entity type supported by the IQueryProvider instance passed when constructing objects of this class.</typeparam>
[ComVisible(false)]
public class GenericQueryable<TElement> : IOrderedQueryable<TElement>
{

    /// <summary>
    /// Creates an instance that returns empty result of type T.
    /// </summary>
    public GenericQueryable()
    {
        var emptyQuery = Enumerable.Empty<TElement>().AsQueryable();
        Provider = emptyQuery.Provider;
        Expression = emptyQuery.Expression;
    }

    /// <summary>
    /// Creates an instance connected to a IQueryProvider that will execute queries
    /// represented by this IQueryable instance. Root of expression tree is set to a
    /// ConstantExpression containing the created GenericQueryable(OF TElement) instance.
    /// </summary>
    /// <param name="context">IQueryProvider that will execute queries
    /// represented by this IQueryable instance.</param>
    public GenericQueryable(IQueryProvider context)
    {
        Provider = context;
        Expression = Expression.Constant(this);
    }

    /// <summary>
    /// Creates an instance connected to a IQueryProvider that will execute queries
    /// represented by this IQueryable instance.
    /// </summary>
    /// <param name="context">IQueryProvider that will execute queries
    /// represented by this IQueryable instance.</param>
    /// <param name="expression">Root of expression tree for this GenericQueryable(OF TElement) instance.</param>
    public GenericQueryable(IQueryProvider context, Expression expression)
    {
        Provider = context;
        Expression = expression;
    }

    /// <summary>
    /// Executes strongly typed query and creates an enumerator for iterating over result.
    /// </summary>
    public virtual IEnumerator<TElement> GetEnumerator() => (Provider.Execute<IEnumerable<TElement>>(Expression) ?? Enumerable.Empty<TElement>()).GetEnumerator();

    /// <summary>
    /// Executes typeless query and creates an enumerator for iterating over result.
    /// </summary>
    protected virtual IEnumerator IEnumerable_GetEnumerator() => Provider.Execute<IEnumerable>(Expression).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => IEnumerable_GetEnumerator();

    /// <summary>
    /// Returns Type of objects returned in results from this query instance.
    /// </summary>
    public virtual Type ElementType => typeof(TElement);

    /// <summary>
    /// Returns root of expression tree in this instance.
    /// </summary>
    public virtual Expression Expression { get; }

    /// <summary>
    /// Returns IQueryProvider instance used to execute queries with this instance.
    /// </summary>
    public virtual IQueryProvider Provider { get; }

    /// <summary>
    /// Return a string describing this query and associated entity type.
    /// </summary>
    public override string? ToString()
    {
        if (Expression.NodeType == ExpressionType.Constant
            && Equals(((ConstantExpression)Expression).Value))
        {
            return $"GenericQuery<{ElementType.Name}>";
        }
        else
        {
            return Expression.ToString();
        }
    }

}

#endif
