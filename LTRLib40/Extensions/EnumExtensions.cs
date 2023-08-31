#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LTRLib.Extensions;

public static class EnumDescriptions<TEnum> where TEnum : struct, Enum
{
    public static ImmutableDictionary<TEnum, string> EnumDictionary { get; }

    static EnumDescriptions()
    {
        EnumDictionary = Enum.GetNames<TEnum>()
            .Select(n => (Name: typeof(TEnum)
                .GetField(n, BindingFlags.Public | BindingFlags.Static)?
                .GetCustomAttribute<DescriptionAttribute>() is { } attr
                ? attr.Description : n, Value: Enum.Parse<TEnum>(n)))
            .DistinctBy(record => record.Value)
            .ToImmutableDictionary(record => record.Value, record => record.Name);
    }
}

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
        => EnumDescriptions<TEnum>.EnumDictionary.TryGetValue(value, out var description)
        ? description : value.ToString();
}

#endif
