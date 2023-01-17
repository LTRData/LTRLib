// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LTRLib.Net;

public static class HttpExtensions
{
    public static string? Get(this in QueryString query, string key)
    {
        var buffer = HttpUtility.ParseQueryString(query.Value ?? "");

        return buffer[key];
    }

    public static QueryString Remove(this in QueryString query, string key)
    {
        var buffer = HttpUtility.ParseQueryString(query.Value ?? "");

        if (buffer[key] is null)
        {
            return query;
        }

        buffer.Remove(key);

        return QueryString.FromUriComponent($"?{query}");
    }

    public static QueryString AddIfNotNull(this in QueryString query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return query.Add(key, value);
        }

        return query;
    }
}

#endif
