// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;

namespace LTRLib.LTRGeneric;

public static class EnumTools
{
    /// <summary>
    /// Gets the value that corresponds to a name in an Enum type.
    /// </summary>
    /// <typeparam name="E">Any Enum Type.</typeparam>
    /// <param name="name">Name of Enum member.</param>
    /// <param name="ignorecase"></param>
    public static E ParseEnumName<E>(string name, bool ignorecase) where E : struct, Enum
#if NET5_0_OR_GREATER
        => Enum.Parse<E>(name, ignorecase);
#else
        => (E)Enum.Parse(typeof(E), name, ignorecase);
#endif

    /// <summary>
    /// Gets the value that corresponds to a name in an Enum type.
    /// </summary>
    /// <typeparam name="E">Any Enum Type.</typeparam>
    /// <param name="name">Name of Enum member.</param>
    public static E ParseEnumName<E>(string name) where E : struct, Enum
#if NET5_0_OR_GREATER
        => Enum.Parse<E>(name);
#else
        => (E)Enum.Parse(typeof(E), name);
#endif

    /// <summary>
    /// Gets the value that corresponds to a name in the Enum type 
    /// </summary>
    /// <typeparam name="E">Any Enum type.</typeparam>
    /// <param name="enumVar">Variable of Enum type.</param>
    /// <param name="Name">Name of Enum member.</param>
    /// <param name="ignorecase"></param>
    public static void ParseEnumName<E>(ref E enumVar, string Name, bool ignorecase) where E : struct, Enum => enumVar = ParseEnumName<E>(Name, ignorecase);

    /// <summary>
    /// Gets the value that corresponds to a name in the Enum type 
    /// </summary>
    /// <typeparam name="E">Any Enum type.</typeparam>
    /// <param name="enumVar">Variable of Enum type.</param>
    /// <param name="Name">Name of Enum member.</param>
    public static void ParseEnumName<E>(ref E enumVar, string Name) where E : struct, Enum => enumVar = ParseEnumName<E>(Name);

}