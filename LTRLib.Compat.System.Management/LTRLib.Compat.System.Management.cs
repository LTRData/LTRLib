﻿using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

namespace LTRLib.Compat.System.Management;

public class ObjectGetOptions
{
    public CimOperationOptions? Options { get; set; }
}

public class PutOptions
{
    public CimOperationOptions? Options { get; set; }
}

public class ManagementPath
{
    public ManagementPath(string path)
    {
        var delimiter = path.IndexOf(':');
        if (delimiter >= 0)
        {
            NamespacePath = path.Substring(0, delimiter);
            var className = path.Substring(delimiter + 1);
            delimiter = className.IndexOf('.');
            string? condition = null;
            if (delimiter >= 0)
            {
                condition = className.Substring(delimiter + 1);
                className = className.Substring(0, delimiter);
            }

            Query = new(className, condition, null);
        }
        else
        {
            Query = new(path, null, null);
        }
    }

    public ManagementPath()
    {
        Query = new();
    }

    public string? ClassName { get => Query.ClassName; set => Query.ClassName = value; }

    public SelectQuery Query { get; set; }

    public string NamespacePath { get; set; } = @"root\cimv2";
}

public class ManagementClass(ManagementScope scope, ManagementPath path, ObjectGetOptions options) : ManagementDisposable
{
    public ManagementScope Scope { get; } = scope ?? new();
    public ManagementPath Path { get; } = path;
    public ObjectGetOptions Options { get; } = options;

    public ManagementObject CreateInstance() => new(Scope, Path, Options);

    public ManagementObjectCollection GetInstances(EnumerationOptions _)
    {
        using var cimSession = CimSession.Create(null);
        var list = new ManagementObjectCollection(cimSession.EnumerateInstances(Path.NamespacePath, Path.ClassName));
        return list;
    }

    public ManagementParameters GetMethodParameters(string methodName)
    {
        using var cimInstance = new CimInstance(Path.ClassName, Path.NamespacePath);
        using var cimSession = CimSession.Create(null);
        using var cim = cimSession.GetInstance(Path.NamespacePath, cimInstance);
        var parameterDeclarations = cim.CimClass.CimClassMethods.First(m => m.Name == methodName).Parameters;
        return new(parameterDeclarations);
    }

    public ManagementParameters InvokeMethod(string methodName, ManagementBaseObject inParams, InvokeMethodOptions _)
    {
        using var cimSession = CimSession.Create(null);
        var result = cimSession.InvokeMethod(Path.NamespacePath, Path.ClassName, methodName, (CimMethodParametersCollection)((ManagementParameters)inParams).Properties);

        return new(result);
    }

    public async Task<ManagementParameters> InvokeMethodAsync(string methodName, ManagementBaseObject inParams, InvokeMethodOptions _)
    {
        using var cimSession = CimSession.Create(null);
        var result = await cimSession.InvokeMethodAsync(Path.NamespacePath, Path.ClassName, methodName, (CimMethodParametersCollection)((ManagementParameters)inParams).Properties)
            .ToAsyncEnumerable()
            .LastAsync()
            .ConfigureAwait(false);

        return new(result);
    }
}

public class ManagementOptions
{
    public bool EnablePrivileges { get; set; }
}

public class ManagementScope
{
    public ManagementScope(string namespacePath)
    {
        Path.NamespacePath = namespacePath;
    }

    public ManagementScope()
    {
    }

    public ManagementPath Path { get; set; } = new();

    public ManagementOptions Options { get; set; } = new();
}

public abstract class ManagementDisposable : IDisposable
{
    public bool IsDisposed { get; private set; }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
        }
    }

    ~ManagementDisposable()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public abstract class ManagementBaseObject : ManagementDisposable
{
    public virtual CimReadOnlyKeyedCollection<CimMethodParameter>? Properties { get; }

    public abstract object? this[string key] { get; set; }

    public virtual string? ClassPath { get; }

    public void Refresh() => (this as ManagementObject)?.Get();
}

public class ManagementParameters : ManagementBaseObject
{
    public override CimReadOnlyKeyedCollection<CimMethodParameter> Properties { get; }

    public override object? this[string key]
    {
        get => Properties[key].Value;
        set => Properties[key].Value = value;
    }

    public ManagementParameters()
    {
        Properties = new CimMethodParametersCollection();
    }

    public ManagementParameters(CimReadOnlyKeyedCollection<CimMethodParameterDeclaration> parameterDeclarations)
    {
        var collection = new CimMethodParametersCollection();

        foreach (var param in parameterDeclarations)
        {
            collection.Add(CimMethodParameter.Create(param.Name, null, param.CimType, CimFlags.In));
        }

        Properties = collection;
    }

    public ManagementParameters(CimMethodResult methodResult)
    {
        Properties = methodResult.OutParameters;
    }
}

public class ManagementObject : ManagementBaseObject
{
    public override object? this[string key]
    {
        get => key switch
        {
            "__CLASS" => CimInstance.CimSystemProperties?.ClassName,
            _ => CimInstance.CimInstanceProperties?[key]?.Value,
        };
        set => CimInstance.CimInstanceProperties[key].Value = value;
    }

    public ManagementObject(ManagementScope scope, ManagementPath path, ObjectGetOptions options)
    {
        CimSession = CimSession.Create(null);

        Scope = scope;
        Path = path;
        Options = options;

        CimInstance = Get();
    }

    public ManagementObject(CimInstance current)
    {
        CimInstance = current;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CimInstance?.Dispose();
            CimSession?.Dispose();
        }
    }

    public CimSession? CimSession { get; }

    public CimInstance CimInstance { get; private set; }

    public ManagementScope? Scope { get; set; }

    public ManagementPath? Path { get; set; }

    public ObjectGetOptions? Options { get; set; }

    public override string? ClassPath => Path?.ClassName;

    public void Delete() => (CimSession ?? throw new InvalidOperationException("CimSession object needed for this operation"))
        .DeleteInstance(CimInstance);

    public CimInstance Get()
    {
        if (Path is null || CimSession is null)
        {
            throw new InvalidOperationException("This operation requires a CimSession object and a query");
        }

        CimInstance?.Dispose();

        if (string.IsNullOrWhiteSpace(Path.Query.Condition))
        {
            CimInstance = new(Path.ClassName, Path.NamespacePath);
            CimInstance = CimSession.GetInstance(Path.NamespacePath, CimInstance);
        }
        else
        {
            var query = CimSession.QueryInstances(Path.NamespacePath,
                                                  "WQL",
                                                  Path.Query.ToString());

            var array = query?.ToList();

            if (array is null || array.Count == 0)
            {
                CimInstance = new(Path.ClassName, Path.NamespacePath);
            }
            else
            {
                foreach (var item in array.Skip(1))
                {
                    item.Dispose();
                }

                CimInstance = array[0];
            }
        }

        return CimInstance;
    }

    public void Put()
        => (CimSession ?? throw new InvalidOperationException("CimSession object needed for this operation"))
        .ModifyInstance(CimInstance);

    public void Put(PutOptions options)
        => CimSession?.ModifyInstance(Path?.NamespacePath, CimInstance, options.Options);

    public ManagementParameters GetMethodParameters(string methodName)
    {
        var methodDeclaration = CimInstance.CimClass.CimClassMethods[methodName]
            ?? throw new MissingMethodException(methodName);

        var parameterDeclarations = methodDeclaration.Parameters;

        return new(parameterDeclarations);
    }

    public new CimKeyedCollection<CimProperty> Properties => CimInstance.CimInstanceProperties;

    public ManagementParameters InvokeMethod(string methodName, ManagementBaseObject inParams, InvokeMethodOptions _)
    {
        if (CimSession is null)
        {
            throw new InvalidOperationException("CimSession object needed for this operation");
        }

        var result = CimSession.InvokeMethod(CimInstance, methodName, (CimMethodParametersCollection)((ManagementParameters)inParams)?.Properties!);

        return new(result);
    }

    public async Task<ManagementParameters> InvokeMethodAsync(string methodName, ManagementBaseObject inParams, InvokeMethodOptions _)
    {
        if (CimSession is null)
        {
            throw new InvalidOperationException("CimSession object needed for this operation");
        }

        var result = await CimSession.InvokeMethodAsync(CimInstance, methodName, (CimMethodParametersCollection)((ManagementParameters)inParams)?.Properties!)
            .ToAsyncEnumerable()
            .LastAsync()
            .ConfigureAwait(false);

        return new(result);
    }
}

public class InvokeMethodOptions
{
}

public class EnumerationOptions
{
    public bool EnsureLocatable { get; set; }
}

public class ManagementObjectCollection : IReadOnlyCollection<ManagementObject>
{
    public List<CimInstance> Collection { get; }

    public bool IsSynchronized => (Collection as ICollection).IsSynchronized;

    public int Count => Collection.Count;

    public object SyncRoot => (Collection as ICollection).SyncRoot;

    public ManagementObjectEnumerator GetEnumerator() => new(Collection);

    internal ManagementObjectCollection(IEnumerable<CimInstance> enumerable)
    {
        Collection = [.. enumerable];
    }

    public void CopyTo(Array array, int index) => (Collection as ICollection).CopyTo(array, index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<ManagementObject> IEnumerable<ManagementObject>.GetEnumerator() => GetEnumerator();

    public struct ManagementObjectEnumerator : IEnumerator<ManagementObject>
    {
        private List<CimInstance>.Enumerator innerEnumerator;

        internal ManagementObjectEnumerator(List<CimInstance> list)
        {
            innerEnumerator = list.GetEnumerator();
        }

        public ManagementObject Current => new(innerEnumerator.Current);

        object IEnumerator.Current => Current;

        public void Dispose() => innerEnumerator.Dispose();

        public bool MoveNext() => innerEnumerator.MoveNext();

        public void Reset() => throw new NotImplementedException();
    }
}

public class ManagementObjectSearcher(ManagementScope mgmtScope, SelectQuery selectQuery) : ManagementDisposable
{
    public EnumerationOptions? Options { get; set; }
    public ManagementScope Scope { get; } = mgmtScope;
    public SelectQuery SelectQuery { get; } = selectQuery;

    public ManagementObjectCollection Get()
    {
        using var cimSession = CimSession.Create(null);
        ManagementObjectCollection list;
        if (SelectQuery.Condition is null)
        {
            list = new(cimSession.EnumerateInstances(Scope.Path.NamespacePath, SelectQuery.ClassName));
        }
        else
        {
            list = new(cimSession.QueryInstances(Scope.Path.NamespacePath, "WQL", SelectQuery.ToString()));
        }

        return list;
    }
}

public class SelectQuery
{
    public SelectQuery(string className, string? condition, string[]? selectedProperties)
    {
        ClassName = className;
        Condition = condition;
        SelectedProperties = selectedProperties;
    }

    public SelectQuery()
    {
    }

    public string? ClassName { get; set; }
    public string? Condition { get; set; }
    public string[]? SelectedProperties { get; set; }

    public override string ToString()
    {
        var fields = "*";
        if (SelectedProperties is not null && SelectedProperties.Length > 0)
        {
            fields = string.Join(", ", SelectedProperties);
        }

        if (string.IsNullOrWhiteSpace(Condition))
        {
            return $"select {fields} from {ClassName}";
        }
        else
        {
            return $"select {fields} from {ClassName} where {Condition}";
        }
    }
}
