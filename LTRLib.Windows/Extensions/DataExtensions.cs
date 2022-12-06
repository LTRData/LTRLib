#if NET35_OR_GREATER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace LTRLib.Extensions;

public static class DataExtensions
{
    /// <summary>
    /// Gets a typed object from an IDataObject instance.
    /// </summary>
    /// <typeparam name="T">Type of object to get.</typeparam>
    /// <param name="sender">IDataObject instance to get object from.</param>
    public static T GetDataAsType<T>(this IDataObject sender)
         => (T)sender.GetData(typeof(T));


    public static bool FindRecord<T>(this BindingSource bindingSource, Func<T, bool> filter)
    {
        var foundRecord = ((IEnumerable<T>)bindingSource.DataSource).FirstOrDefault(filter);
        if (foundRecord is null)
        {
            return false;
        }

        return bindingSource.FindRecord(foundRecord);
    }

    
    public static bool FindRecord(this BindingSource bindingSource, Func<object, bool> filter)
    {
        var foundRecord = ((IEnumerable)bindingSource.DataSource).OfType<object>().FirstOrDefault(filter);
        if (foundRecord is null)
        {
            return false;
        }

        return bindingSource.FindRecord(foundRecord);
    }

    
    public static bool FindRecord(this BindingSource bindingSource, object record)
    {
        var position = bindingSource.IndexOf(record);
        if (position < 0)
        {
            return false;
        }

        bindingSource.Position = position;
        return true;
    }
        
    public static int Count(this ChangeSet ChangeSet) => ChangeSet.Deletes.Count + ChangeSet.Inserts.Count + ChangeSet.Updates.Count;

#if NET40_OR_GREATER

    public static IEnumerable<object> All(this ChangeSet ChangeSet) => ChangeSet.Deletes.Concat(ChangeSet.Inserts).Concat(ChangeSet.Updates);

#endif

    public static void ClearCachedList<TEntity>(this Table<TEntity> LinqDataTable) where TEntity : class
    {
        var binding = BindingFlags.NonPublic | BindingFlags.Instance;

        var dstype = LinqDataTable.GetType();

        var field = dstype.GetField("cachedList", binding);

        field.SetValue(LinqDataTable, default);

        var method = LinqDataTable.Context.GetType().GetMethod("ClearCache", binding);

        method.Invoke(LinqDataTable.Context, default);
    }
}

#endif
