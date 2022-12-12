#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Collections.Generic;
using System.Linq;

namespace ROOT.CIMV2.Win32;

public partial class Diskdrive
{
    /// <summary>
    /// Gets information about physical and logical devices.
    /// </summary>
    /// <returns>List of physical device objects.</returns>
    public static List<Diskdrive> GetPresentDrives() => GetInstances().OfType<Diskdrive>().Where(disk => disk.MediaLoaded).ToList();

    /// <summary>
    /// Implements ToString method for physical disk drive.
    /// </summary>
    public override string ToString() => $"{Model} (disk {Index})";
}

public partial class CdRomDrive
{
    /// <summary>
    /// Gets information about physical and logical devices.
    /// </summary>
    /// <returns>List of physical device objects.</returns>
    public static List<CdRomDrive> GetPresentDrives() => GetInstances().OfType<CdRomDrive>().Where(disk => disk.MediaLoaded).ToList();

    /// <summary>
    /// Implements ToString method for physical disk drive.
    /// </summary>
    public override string ToString() => $"{VolumeName ?? Description} (cd {Drive})";
}

#endif

