// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;

namespace LTRLib.LTRGeneric;

[AttributeUsage(AttributeTargets.Field)]
public class EnumDescriptionAttribute : Attribute
{

    public string Description;

    public EnumDescriptionAttribute(string Description) : base()
    {

        this.Description = Description;
    }
}