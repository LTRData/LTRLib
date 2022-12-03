/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LTRLib.LTRGeneric;

/// <summary>
/// Provides a generic collection that supports data binding and additionally supports sorting.
/// See http://msdn.microsoft.com/en-us/library/ms993236.aspx
/// If the elements are IComparable it uses that; otherwise compares the ToString()
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public class SortableBindingList<T> : BindingList<T> where T : class
{
    private bool _isSorted;
    private ListSortDirection _sortDirection = ListSortDirection.Ascending;
    private PropertyDescriptor? _sortProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
    /// </summary>
    public SortableBindingList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
    /// </summary>
    /// <param name="list">An <see cref="T:System.Collections.Generic.List`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
    public SortableBindingList(List<T> list)
        : base(list)
    {
    }

    public List<T> InnerList => (List<T>)Items;

    public void AddRange(IEnumerable<T> items)
    {
        InnerList.AddRange(items);

        ResetBindings();
    }

    public void ForEach(Action<T> action) => InnerList.ForEach(action);

    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => InnerList.ConvertAll(converter);

    public void ApplySort(PropertyDescriptor prop, ListSortDirection direction) => ApplySortCore(prop, direction);

    public void RemoveSort() => RemoveSortCore();

    public bool IsSorted => _isSorted;

    public PropertyDescriptor? SortProperty => _sortProperty;

    public ListSortDirection SortDirection => _sortDirection;

    protected override object? AddNewCore()
    {
        var item = base.AddNewCore();

        Sort();

        return item;
    }

    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);

        Sort();
    }

    public void Sort()
    {
        if (_isSorted)
        {
            InnerList.Sort(Compare);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the list supports sorting.
    /// </summary>
    protected override bool SupportsSortingCore => true;

    /// <summary>
    /// Gets a value indicating whether the list is sorted.
    /// </summary>
    protected override bool IsSortedCore => _isSorted;

    /// <summary>
    /// Gets the direction the list is sorted.
    /// </summary>
    protected override ListSortDirection SortDirectionCore => _sortDirection;

    /// <summary>
    /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
    /// </summary>
    protected override PropertyDescriptor? SortPropertyCore => _sortProperty;

    /// <summary>
    /// Removes any sort applied with ApplySortCore if sorting is implemented
    /// </summary>
    protected override void RemoveSortCore()
    {
        _sortDirection = ListSortDirection.Ascending;
        _sortProperty = null;
        _isSorted = false; //thanks Luca
    }

    /// <summary>
    /// Sorts the items if overridden in a derived class
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="direction"></param>
    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
        _sortProperty = prop;
        _sortDirection = direction;

        InnerList.Sort(Compare);

        _isSorted = true;

        //fire an event that the list has been changed.
        ResetBindings();

        OnSortApplied(EventArgs.Empty);
    }

    private int Compare(T lhs, T rhs)
    {
        var result = OnComparison(lhs, rhs);

        //invert if descending
        if (_sortDirection == ListSortDirection.Descending)
        {
            result = -result;
        }

        return result;
    }

    protected virtual int OnComparison(T lhs, T rhs)
    {
        var lhsValue = lhs is null ? null : _sortProperty?.GetValue(lhs);
        var rhsValue = rhs is null ? null : _sortProperty?.GetValue(rhs);
        if (ReferenceEquals(lhsValue, rhsValue))
        {
            return 0; //nulls are equal, same object refs are equal
        }

        if (lhsValue is null)
        {
            return -1; //second has value, first doesn't
        }

        if (rhsValue is null)
        {
            return 1; //first has value, second doesn't
        }

        if (lhsValue is IComparable comparable)
        {
            return comparable.CompareTo(rhsValue);
        }

        if (lhsValue.Equals(rhsValue))
        {
            return 0; //both are the same
        }
        
        //not comparable, compare ToString
        return string.Compare(lhsValue.ToString(), rhsValue.ToString());
    }

    public event EventHandler? SortApplied;

    protected void OnSortApplied(EventArgs e) => SortApplied?.Invoke(this, e);
}
