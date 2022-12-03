#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRLib.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTRLib.Extensions;

public static class WebExtensions
{
    public static IApplicationBuilder UseApplicationFeatures(this IApplicationBuilder app, IFileProvider FileProvider, HttpShared.DynDocFeatures Features)
    {
        return app.Use(async (context, next) =>
        {
            if (await WebCore.HttpServerSupport.ApplicationAddFeatures(context, context.Request, context.Response, FileProvider, Features).ConfigureAwait(continueOnCapturedContext: false))
            {
                return;
            }

            await next().ConfigureAwait(continueOnCapturedContext: false);
        });
    }
}

#endif
