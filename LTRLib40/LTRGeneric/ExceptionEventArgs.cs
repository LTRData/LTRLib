// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Threading;

namespace LTRLib.LTRGeneric;

public class QueryExceptionEventArgs : ThreadExceptionEventArgs
{

    public bool Rethrow { get; set; }

    public QueryExceptionEventArgs(Exception ex) : base(ex)
    {

        Rethrow = true;
    }

}