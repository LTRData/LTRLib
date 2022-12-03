
// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NETFRAMEWORK && !NET45_OR_GREATER

using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System;

[ComVisible(false)]
[Serializable]
public partial class WeakReference<T> : WeakReference where T : class
{

    public WeakReference() : base(null)
    {

    }

    public WeakReference(T target) : base(target)
    {

    }

    public WeakReference(T target, bool trackResurrection) : base(target, trackResurrection)
    {

    }

    protected WeakReference(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }

    public new T? Target
    {
        get
        {
            return base.Target as T;
        }
        set
        {
            base.Target = value;
        }
    }

    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);

    public override string? ToString() => Target?.ToString();

}

#endif
