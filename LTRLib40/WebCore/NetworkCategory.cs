// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

#if NETCOREAPP || NETSTANDARD || NET461_OR_GREATER


namespace LTRLib.WebCore;

public enum NetworkCategory
{

    None,

    Loopback,

    LoopbackAndPrivate,

    All

}

#endif
