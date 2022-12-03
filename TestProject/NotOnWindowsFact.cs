using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LTRLib;

public class NotOnWindowsFactAttribute : FactAttribute
{
    public NotOnWindowsFactAttribute()
    {
#if NETSTANDARD || NETCOREAPP
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = null;
        }
        else
        {
            Skip = "This test runs on non-Windows platforms only";
        }
#else
        Skip = "This test runs on non-Windows platforms only";
#endif
    }
}
