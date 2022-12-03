// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System.IO;

namespace LTRLib.IO;

/// <summary>
/// An implementation of MemoryStream that keeps all methods and properties accessible
/// even after object has been disposed.
/// </summary>
public class CloseSafeMemoryStream : MemoryStream
{
    protected override void Dispose(bool disposing)
    {
        Position = 0L;
        base.Dispose(false);
    }
}
