// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

namespace LTRLib.IO;

/// <summary>
/// ASCII control characters
/// </summary>
/// <remarks></remarks>
public static class AsciiControl
{
    public static readonly char Cr = '\r';
    public static readonly char Lf = '\n';
    public static readonly string CrLf = "\r\n";
    public static readonly char Backspace = '\b';
    public static readonly char Tab = '\t';
    public static readonly char VerticalTab = '\v';
    public static readonly char MultiStringArraySeparator = '\u0002';
}
