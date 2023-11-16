// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;

namespace LTRLib.LTRGeneric;

[AttributeUsage(AttributeTargets.Field)]
public class EnumDescriptionAttribute(string Description) : Attribute()
{
    public string Description { get; set; } = Description;
}