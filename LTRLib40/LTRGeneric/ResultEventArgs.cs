// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Runtime.InteropServices;

namespace LTRLib.LTRGeneric;

[ComVisible(false)]
public class QueryResultEventArgs<TResult> : EventArgs
{
    public QueryResultEventArgs()
    {
    }

    public QueryResultEventArgs(TResult result)
    {
        Result = result;
    }

    public readonly TResult? Result;
}
