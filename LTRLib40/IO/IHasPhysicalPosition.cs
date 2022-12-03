/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
namespace LTRLib.IO;

public interface IHasPhysicalPosition
{
    long? PhysicalPosition { get; }
}
