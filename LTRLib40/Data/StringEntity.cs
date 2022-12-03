/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
namespace LTRLib.Data;

public sealed class StringEntity
{
    public string? Name { get; set; }

    public static implicit operator StringEntity?(string? name) => FromString(name);

    public static StringEntity? FromString(string? name) => name is null ? null : new StringEntity { Name = name };

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static bool IsNullOrWhiteSpace(StringEntity obj) => obj is null || string.IsNullOrWhiteSpace(obj.Name);

    public static bool NeitherNullNorWhiteSpace(StringEntity obj) => !string.IsNullOrWhiteSpace(obj?.Name);

    public static bool NeitherNullNorWhiteSpace(string obj) => !string.IsNullOrWhiteSpace(obj);
#endif

    public static string? ToString(StringEntity obj) => obj?.Name;

    public static implicit operator string?(StringEntity? name) => name?.Name;

    public override string? ToString() => Name;

    public override int GetHashCode() => Name?.GetHashCode() ?? 0;

    public override bool Equals(object? obj) => Name?.Equals(obj?.ToString()) ??
            obj?.ToString() is null;
}
