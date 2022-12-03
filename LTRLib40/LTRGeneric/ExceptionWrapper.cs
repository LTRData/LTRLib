// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;

namespace LTRLib.LTRGeneric;

/// <summary>
/// Wraps text in an Exception object so that XML serializer accepts it.
/// </summary>
public class ExceptionWrapper
{
    public ExceptionWrapper()
    {
        Message = "Empty ExceptionWrapper";
    }

    public ExceptionWrapper(Exception value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        Message = value.Message;
        StackTrace = value.StackTrace;
        if (value.InnerException is not null)
        {
            InnerException = new ExceptionWrapper(value.InnerException);
        }
    }

    public string Message { get; set; }

    public string? StackTrace { get; set; }

    public ExceptionWrapper? InnerException { get; set; }

    public IEnumerable<ExceptionWrapper> AsEnumerable()
    {
        var current = this;
        while (current is not null)
        {
            yield return current;
            current = current.InnerException;
        }
    }

    public override string ToString() => Message;
}