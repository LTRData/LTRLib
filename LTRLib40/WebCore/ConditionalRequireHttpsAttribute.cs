
// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator

#if NETCOREAPP || NETSTANDARD || NET461_OR_GREATER

using System.Runtime.InteropServices;
using LTRLib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static LTRLib.Extensions.NetExtensions;

namespace LTRLib.WebCore;

[ComVisible(false)]
public partial class ConditionalRequireHttpsAttribute : RequireHttpsAttribute
{

    public static NetworkCategory InsecureAllowedNetworks { get; set; }

    public override void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        if (filterContext.HttpContext.Request.IsHttpsClient())
        {
            return;
        }

        switch (InsecureAllowedNetworks)
        {
            case NetworkCategory.All:
                {
                    return;
                }

            case NetworkCategory.LoopbackAndPrivate:
                {
                    if (filterContext.HttpContext.Connection.LocalIpAddress is null)
                    {
                        var remoteIpAddress = filterContext.HttpContext.GetClientTrueIPAddress();

                        if (remoteIpAddress is null || remoteIpAddress.IsLoopbackOrPrivate())
                        {
                            return;
                        }
                    }
                    else if (filterContext.HttpContext.Connection.LocalIpAddress.IsLoopbackOrPrivate())
                    {
                        return;
                    }

                    break;
                }

            case NetworkCategory.Loopback:
                {
                    var remoteIpAddress = filterContext.HttpContext.GetClientTrueIPAddress();

                    if (remoteIpAddress is null || remoteIpAddress.IsLoopback())
                    {
                        return;
                    }

                    break;
                }
        }

        base.OnAuthorization(filterContext);
    }
}

#endif
