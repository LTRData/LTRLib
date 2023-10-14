using System;
using System.Collections.Generic;
using System.ComponentModel;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
#endif
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Text;
using System.Threading;
using System.IO;

namespace LTRLib.Extensions;

public static class SynchronizationContextExtensions
{
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static SynchronizationContext? GetSynchronizationContext(this ISynchronizeInvoke owner) =>
        owner.InvokeRequired && owner.Invoke(() => SynchronizationContext.Current, null) is SynchronizationContext context
        ? context : SynchronizationContext.Current;
#endif
}
