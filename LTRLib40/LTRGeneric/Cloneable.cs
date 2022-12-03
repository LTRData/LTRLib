// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;

namespace LTRLib.LTRGeneric;

public abstract class Cloneable : ICloneable
{

    public virtual object Clone() => MemberwiseClone();
}