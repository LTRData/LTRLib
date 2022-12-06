// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace LTRLib.LTRGeneric;

/// <summary>
/// Provides IDisposable semantics to a COM object.
/// </summary>
[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public abstract class DisposableComWrapper : MarshalByRefObject
{
    /// <summary>
    /// Creates a new DisposableComWrapper(Of T) object around an existing type T COM object.
    /// </summary>
    public static DisposableComWrapper<T> Create<T>(T target) where T : class
    {
        return new DisposableComWrapper<T>(target);
    }
}

/// <summary>
/// Provides IDisposable semantics to a COM object.
/// </summary>
[ComVisible(false)]
[SecurityCritical]
[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
public class DisposableComWrapper<T> : DisposableComWrapper, IDisposable where T : class
{
    private T _target = null!;

    /// <summary>
    /// Creates a new instance without having Target property set to any initial
    /// object reference.
    /// </summary>
    public DisposableComWrapper()
    {
    }

    /// <summary>
    /// Creates a new instance with Target property set to an existing object, if
    /// object is of type T. If conversion fails, an exception is thrown.
    /// </summary>
    public DisposableComWrapper(object Target)
    {
        _target = (T)Target;
    }

    /// <summary>
    /// Creates a new instance with Target property set to an existing object of
    /// type T.
    /// </summary>
    public DisposableComWrapper(T Target)
    {
        _target = Target;
    }

    /// <summary>
    /// Gets or sets object that is encapsulated by this instance. If this property is
    /// set it will release existing object. Set property to Nothing/null to release
    /// object without setting a new object. This is also automatically done by
    /// IDisposable.Dispose implementation, or by finalizer.
    /// </summary>
    /// <value>New object to control.</value>
    public T Target
    {
        get
        {
            return _target;
        }
        [SecurityCritical]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
        set
        {
            if (ReferenceEquals(value, _target))
            {
                return;
            }

            if (_target is not null)
            {
                if (_target is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                else if (Marshal.IsComObject(_target))
                {
                    Marshal.ReleaseComObject(_target);
                }
            }

            _target = value;
        }
    }

    public static implicit operator T(DisposableComWrapper<T> value)
    {
        return value.Target;
    }

    [SecuritySafeCritical]
    ~DisposableComWrapper()
    {
        Dispose(false);
    }

    [SecurityCritical]
    protected virtual void Dispose(bool disposing)
    {
        try
        {
            Target = null!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Sets Target property to Nothing/null and thereby releasing existing
    /// object, if any. This method does this within a try/catch block so that
    /// any exceptions are ignored.
    /// </summary>
    [SecuritySafeCritical]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

