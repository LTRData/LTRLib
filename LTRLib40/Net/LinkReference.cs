// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#nullable enable

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.LTRGeneric;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LTRLib.Net;

[ComVisible(false)]
public class LinkReference<T> : MarshalByRefObject where T : class
{
    private readonly T? _obj;

    public LinkReference()
    {
    }

    public LinkReference(T? obj)
    {
        _obj = obj;
    }

    public LinkReference(string @ref)
    {
        this.@ref = @ref;
    }

    private static readonly CacheDictionary<string, T> ObjCache = new();

    [XmlIgnore]
    protected TimeSpan Cachetime { get; set; } = new TimeSpan(0, 0, 20);

    [XmlIgnore]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Used by subclasses in XML serialization")]
    public virtual string? @ref { get; set; }

    [XmlIgnore]
    public virtual T? Target
    {
        get
        {
            if (_obj is not null)
            {
                return _obj;
            }

            if (@ref is null || string.IsNullOrWhiteSpace(@ref))
            {
                return default;
            }

            if (ObjCache.TryGetValue(@ref, out var result))
            {
                return result?.Result;
            }

            if (DownloadObjectFunction is not null)
            {
                result = DownloadObjectFunction(@ref);
            }
            else
            {
                result = DownloadObjectAsync();
            }

            ObjCache.AddOrUpdate(@ref, Cachetime, result);

            return result.Result;
        }
    }

    [XmlIgnore]
    public virtual Func<string, Task<T?>>? DownloadObjectFunction { get; set; }

    public virtual async Task<T?> DownloadObjectAsync() => XmlSupport.XmlDeserialize<T>(await httpClient.GetByteArrayAsync(@ref).ConfigureAwait(false));

    private static readonly HttpClient httpClient = new();

}

#endif
