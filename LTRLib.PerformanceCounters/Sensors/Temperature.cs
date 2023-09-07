using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace LTRLib.LTRGeneric;

public static class Temperature
{
    [SupportedOSPlatform("windows")]
    public static IEnumerable<KeyValuePair<string, float>> EnumerateThermalZone()
    {
        var cat = new PerformanceCounterCategory("Thermal Zone Information");

        foreach (var name in cat.GetInstanceNames())
        {
            if (cat.CounterExists("High Precision Temperature"))
            {
                using var temp = new PerformanceCounter("Thermal Zone Information", "High Precision Temperature", name);
                yield return new(name, (temp.NextValue() / 10f) - 273.2f);
            }
            else if (cat.CounterExists("Temperature"))
            {
                using var temp = new PerformanceCounter("Thermal Zone Information", "Temperature", name);
                yield return new(name, temp.NextValue() - 273.2f);
            }
        }
    }
}
