#pragma warning disable

namespace ROOT.CIMV2.Win32;

using System;
using System.ComponentModel;
using System.Management;
using System.Collections;
using System.Globalization;
using System.CodeDom.Compiler;


// Funktionerna ShouldSerialize<PropertyName> är funktioner som används av Egenskapsgranskning i VS för att kontrollera om en viss egenskap måste serialiseras. Dessa funktioner läggs till för alla ValueType-egenskaper (egenskaper av typen Int32, BOOL m.fl. som inte kan anges till Null). Dessa funktioner använder funktionen Is<PropertyName>Null. Funktionerna används också vid implementering av TypeConverter när NULL-värde kontrolleras för egenskapen, så att ett tomt värde kan visas i Egenskapsgranskning om Dra och släpp används i Visual Studio.
// Funktionerna Is<PropertyName>Null() används för att kontrollera om en egenskap är NULL.
// Funktionerna Reset<PropertyName> läggs till för Read/Write-egenskaper som kan ha värdet NULL. Dessa funktioner används i Egenskapsgranskning i VS-designer för att ange en egenskap till NULL.
// Varje egenskap som läggs till i klassen för WMI-egenskaper har angivna attribut som definierar dess beteende i Visual Studio-designer, och vilken TypeConverter som ska användas.
// Konverteringsfunktionerna ToDateTime och ToDmtfDateTime för datum och tid läggs till för klassen så att DMTF-datum/tid kan konverteras till System.DateTime och tvärt om.
// En EarlyBound-klass genererades för WMI-klassen.Win32_Logicaldisk
[GeneratedCode("mgmtclassgen", "")]
public class Logicaldisk : System.ComponentModel.Component
{

    // En privat egenskap som ska innehålla WMI-namnområdet där klassen finns.
    private static string CreatedWmiNamespace = "root\\CimV2";

    // En privat egenskap som ska innehålla namnet på den WMI-klass som skapade den här klassen.
    private static string CreatedClassName = "Win32_Logicaldisk";

    // En privat medlemsvariabel som ska innehålla ManagementScope som används i olika metoder.
    private static System.Management.ManagementScope statMgmtScope = null;

    private ManagementSystemProperties PrivateSystemProperties;

    // Ett underliggande lateBound WMI-objekt.
    private System.Management.ManagementObject PrivateLateBoundObject;

    // En medlemsvariabel som lagrar klassens automatiskt aktiverade beteende.
    private bool AutoCommitProp;

    // En privat variabel som ska innehålla de inbäddade egenskaper som representerar instansen.
    private System.Management.ManagementBaseObject embeddedObj;

    // Det aktuella WMI-objekt som används
    private System.Management.ManagementBaseObject curObj;

    // En flagga som indikerar om instansen är ett inbäddat objekt.
    private bool isEmbedded;

    // Nedan visas olika överlagringar för konstruktörer som initierar en instans för klassen med ett WMI-objekt.
    public Logicaldisk()
    {
        this.InitializeObject(null, null, null);
    }

    public Logicaldisk(string keyDeviceID)
    {
        this.InitializeObject(null, new System.Management.ManagementPath(Logicaldisk.ConstructPath(keyDeviceID)), null);
    }

    public Logicaldisk(System.Management.ManagementScope mgmtScope, string keyDeviceID)
    {
        this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(Logicaldisk.ConstructPath(keyDeviceID)), null);
    }

    public Logicaldisk(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions)
    {
        this.InitializeObject(null, path, getOptions);
    }

    public Logicaldisk(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path)
    {
        this.InitializeObject(mgmtScope, path, null);
    }

    public Logicaldisk(System.Management.ManagementPath path)
    {
        this.InitializeObject(null, path, null);
    }

    public Logicaldisk(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions)
    {
        this.InitializeObject(mgmtScope, path, getOptions);
    }

    public Logicaldisk(System.Management.ManagementObject theObject)
    {
        Initialize();
        if ((CheckIfProperClass(theObject) == true))
        {
            PrivateLateBoundObject = theObject;
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }
        else
        {
            throw new System.ArgumentException("Klassnamnet matchar inte.");
        }
    }

    public Logicaldisk(System.Management.ManagementBaseObject theObject)
    {
        Initialize();
        if ((CheckIfProperClass(theObject) == true))
        {
            embeddedObj = theObject;
            PrivateSystemProperties = new ManagementSystemProperties(theObject);
            curObj = embeddedObj;
            isEmbedded = true;
        }
        else
        {
            throw new System.ArgumentException("Klassnamnet matchar inte.");
        }
    }

    // En egenskap som returnerar WMI-klassens namnområde.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string OriginatingNamespace
    {
        get
        {
            return "root\\CimV2";
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ManagementClassName
    {
        get
        {
            string strRet = CreatedClassName;
            if ((curObj is not null))
            {
                if ((curObj.ClassPath is not null))
                {
                    strRet = ((string)(curObj["__CLASS"]));
                    if (((strRet is null)
                                || (strRet == string.Empty)))
                    {
                        strRet = CreatedClassName;
                    }
                }
            }
            return strRet;
        }
    }

    // Egenskaper som pekar till ett inbäddat objekt för att hämta WMI-objektets systemegenskaper.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ManagementSystemProperties SystemProperties
    {
        get
        {
            return PrivateSystemProperties;
        }
    }

    // En egenskap som returnerar det underliggande lateBound-objektet.
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public System.Management.ManagementBaseObject LateBoundObject
    {
        get
        {
            return curObj;
        }
    }

    // Objektets ManagementScope.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public System.Management.ManagementScope Scope
    {
        get
        {
            if ((isEmbedded == false))
            {
                return PrivateLateBoundObject.Scope;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if ((isEmbedded == false))
            {
                PrivateLateBoundObject.Scope = value;
            }
        }
    }

    // Egenskap som visar aktiverat beteende för WMI-objektet. Om detta är True sparas WMI-objektet automatiskt efter varje egenskapsändring (d.v.s. Put() anropas efter en egenskapsändring).
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoCommit
    {
        get
        {
            return AutoCommitProp;
        }
        set
        {
            AutoCommitProp = value;
        }
    }

    // ManagementPath för det underliggande WMI-objektet.
    [Browsable(true)]
    public System.Management.ManagementPath Path
    {
        get
        {
            if ((isEmbedded == false))
            {
                return PrivateLateBoundObject.Path;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if ((isEmbedded == false))
            {
                if ((CheckIfProperClass(null, value, null) != true))
                {
                    throw new System.ArgumentException("Klassnamnet matchar inte.");
                }
                PrivateLateBoundObject.Path = value;
            }
        }
    }

    // En offentlig och statisk områdesegenskap som används i olika metoder.
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static System.Management.ManagementScope StaticScope
    {
        get
        {
            return statMgmtScope;
        }
        set
        {
            statMgmtScope = value;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsAccessNull
    {
        get
        {
            if ((curObj["Access"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Access describes whether the media is readable (value=1), writeable (value=2), or" +
        " both (value=3). \"Unknown\" (0) and \"Write Once\" (4) can also be defined.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public AccessValues Access
    {
        get
        {
            if ((curObj["Access"] is null))
            {
                return ((AccessValues)(System.Convert.ToInt32(5)));
            }
            return ((AccessValues)(System.Convert.ToInt32(curObj["Access"])));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsAvailabilityNull
    {
        get
        {
            if ((curObj["Availability"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"The availability and status of the device.  For example, the Availability property indicates that the device is running and has full power (value=3), or is in a warning (4), test (5), degraded (10) or power save state (values 13-15 and 17). Regarding the power saving states, these are defined as follows: Value 13 (""Power Save - Unknown"") indicates that the device is known to be in a power save mode, but its exact status in this mode is unknown; 14 (""Power Save - Low Power Mode"") indicates that the device is in a power save state but still functioning, and may exhibit degraded performance; 15 (""Power Save - Standby"") describes that the device is not functioning but could be brought to full power 'quickly'; and value 17 (""Power Save - Warning"") indicates that the device is in a warning state, though also in a power save mode.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public AvailabilityValues Availability
    {
        get
        {
            if ((curObj["Availability"] is null))
            {
                return ((AvailabilityValues)(System.Convert.ToInt32(0)));
            }
            return ((AvailabilityValues)(System.Convert.ToInt32(curObj["Availability"])));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsBlockSizeNull
    {
        get
        {
            if ((curObj["BlockSize"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"Size in bytes of the blocks which form this StorageExtent. If variable block size, then the maximum block size in bytes should be specified. If the block size is unknown or if a block concept is not valid (for example, for Aggregate Extents, Memory or LogicalDisks), enter a 1.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public ulong BlockSize
    {
        get
        {
            if ((curObj["BlockSize"] is null))
            {
                return System.Convert.ToUInt64(0);
            }
            return ((ulong)(curObj["BlockSize"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Caption
    {
        get
        {
            return ((string)(curObj["Caption"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsCompressedNull
    {
        get
        {
            if ((curObj["Compressed"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The Compressed property indicates whether the logical volume exists as a single c" +
        "ompressed entity, such as a DoubleSpace volume.  If file based compression is su" +
        "pported (such as on NTFS), this property will be FALSE.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool Compressed
    {
        get
        {
            if ((curObj["Compressed"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["Compressed"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsConfigManagerErrorCodeNull
    {
        get
        {
            if ((curObj["ConfigManagerErrorCode"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates the Win32 Configuration Manager error code.  The following values may b" +
        "e returned: \n0      This device is working properly. \n1      This device is not " +
        "configured correctly. \n2      Windows cannot load the driver for this device. \n3" +
        "      The driver for this device might be corrupted, or your system may be runni" +
        "ng low on memory or other resources. \n4      This device is not working properly" +
        ". One of its drivers or your registry might be corrupted. \n5      The driver for" +
        " this device needs a resource that Windows cannot manage. \n6      The boot confi" +
        "guration for this device conflicts with other devices. \n7      Cannot filter. \n8" +
        "      The driver loader for the device is missing. \n9      This device is not wo" +
        "rking properly because the controlling firmware is reporting the resources for t" +
        "he device incorrectly. \n10     This device cannot start. \n11     This device fai" +
        "led. \n12     This device cannot find enough free resources that it can use. \n13 " +
        "    Windows cannot verify this device\'s resources. \n14     This device cannot wo" +
        "rk properly until you restart your computer. \n15     This device is not working " +
        "properly because there is probably a re-enumeration problem. \n16     Windows can" +
        "not identify all the resources this device uses. \n17     This device is asking f" +
        "or an unknown resource type. \n18     Reinstall the drivers for this device. \n19 " +
        "    Your registry might be corrupted. \n20     Failure using the VxD loader. \n21 " +
        "    System failure: Try changing the driver for this device. If that does not wo" +
        "rk, see your hardware documentation. Windows is removing this device. \n22     Th" +
        "is device is disabled. \n23     System failure: Try changing the driver for this " +
        "device. If that doesn\'t work, see your hardware documentation. \n24     This devi" +
        "ce is not present, is not working properly, or does not have all its drivers ins" +
        "talled. \n25     Windows is still setting up this device. \n26     Windows is stil" +
        "l setting up this device. \n27     This device does not have valid log configurat" +
        "ion. \n28     The drivers for this device are not installed. \n29     This device " +
        "is disabled because the firmware of the device did not give it the required reso" +
        "urces. \n30     This device is using an Interrupt Request (IRQ) resource that ano" +
        "ther device is using. \n31     This device is not working properly because Window" +
        "s cannot load the drivers required for this device.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public ConfigManagerErrorCodeValues ConfigManagerErrorCode
    {
        get
        {
            if ((curObj["ConfigManagerErrorCode"] is null))
            {
                return ((ConfigManagerErrorCodeValues)(System.Convert.ToInt32(32)));
            }
            return ((ConfigManagerErrorCodeValues)(System.Convert.ToInt32(curObj["ConfigManagerErrorCode"])));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsConfigManagerUserConfigNull
    {
        get
        {
            if ((curObj["ConfigManagerUserConfig"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates whether the device is using a user-defined configuration.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool ConfigManagerUserConfig
    {
        get
        {
            if ((curObj["ConfigManagerUserConfig"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["ConfigManagerUserConfig"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("CreationClassName indicates the name of the class or the subclass used in the cre" +
        "ation of an instance. When used with the other key properties of this class, thi" +
        "s property allows all instances of this class and its subclasses to be uniquely " +
        "identified.")]
    public string CreationClassName
    {
        get
        {
            return ((string)(curObj["CreationClassName"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Description
    {
        get
        {
            return ((string)(curObj["Description"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The DeviceID property contains a string uniquely identifying the logical disk fro" +
        "m other devices on the system.")]
    public string DeviceID
    {
        get
        {
            return ((string)(curObj["DeviceID"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsDriveTypeNull
    {
        get
        {
            if ((curObj["DriveType"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The DriveType property contains a numeric value corresponding to the type of disk" +
        " drive this logical disk represents.  Please refer to the Platform SDK documenta" +
        "tion for additional values.\nExample: A CD-ROM drive would return 5.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public DriveTypeValues DriveType
    {
        get
        {
            if ((curObj["DriveType"] is null))
            {
                return ((DriveTypeValues)(System.Convert.ToInt32(7)));
            }
            return ((DriveTypeValues)(System.Convert.ToInt32(curObj["DriveType"])));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsErrorClearedNull
    {
        get
        {
            if ((curObj["ErrorCleared"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("ErrorCleared is a boolean property indicating that the error reported in LastErro" +
        "rCode property is now cleared.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool ErrorCleared
    {
        get
        {
            if ((curObj["ErrorCleared"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["ErrorCleared"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("ErrorDescription is a free-form string supplying more information about the error" +
        " recorded in LastErrorCode property, and information on any corrective actions t" +
        "hat may be taken.")]
    public string ErrorDescription
    {
        get
        {
            return ((string)(curObj["ErrorDescription"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("ErrorMethodology is a free-form string describing the type of error detection and" +
        " correction supported by this storage extent.")]
    public string ErrorMethodology
    {
        get
        {
            return ((string)(curObj["ErrorMethodology"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The FileSystem property indicates the file system on the logical disk.\nExample: N" +
        "TFS")]
    public string FileSystem
    {
        get
        {
            return ((string)(curObj["FileSystem"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsFreeSpaceNull
    {
        get
        {
            if ((curObj["FreeSpace"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The FreeSpace property indicates in bytes how much free space is available on the" +
        " logical disk.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public ulong FreeSpace
    {
        get
        {
            if ((curObj["FreeSpace"] is null))
            {
                return System.Convert.ToUInt64(0);
            }
            return ((ulong)(curObj["FreeSpace"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsInstallDateNull
    {
        get
        {
            if ((curObj["InstallDate"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public System.DateTime InstallDate
    {
        get
        {
            if ((curObj["InstallDate"] is not null))
            {
                return ToDateTime(((string)(curObj["InstallDate"])));
            }
            else
            {
                return System.DateTime.MinValue;
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsLastErrorCodeNull
    {
        get
        {
            if ((curObj["LastErrorCode"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("LastErrorCode captures the last error code reported by the logical device.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint LastErrorCode
    {
        get
        {
            if ((curObj["LastErrorCode"] is null))
            {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["LastErrorCode"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsMaximumComponentLengthNull
    {
        get
        {
            if ((curObj["MaximumComponentLength"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"The MaximumComponentLength property contains the maximum length of a filename component supported by the Win32 drive. A filename component is that portion of a filename between backslashes.  The value can be used to indicate that long names are supported by the specified file system. For example, for a FAT file system supporting long names, the function stores the value 255, rather than the previous 8.3 indicator. Long names can also be supported on systems that use the NTFS file system.
Example: 255")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public uint MaximumComponentLength
    {
        get
        {
            if ((curObj["MaximumComponentLength"] is null))
            {
                return System.Convert.ToUInt32(0);
            }
            return ((uint)(curObj["MaximumComponentLength"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsMediaTypeNull
    {
        get
        {
            if ((curObj["MediaType"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"The MediaType property indicates the type of media currently present in the logical drive. This value will be one of the values of the MEDIA_TYPE enumeration defined in winioctl.h.
<B>Note:</B> The value may not be exact for removable drives if currently there is no media in the drive.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public MediaTypeValues MediaType
    {
        get
        {
            if ((curObj["MediaType"] is null))
            {
                return ((MediaTypeValues)(System.Convert.ToInt32(23)));
            }
            return ((MediaTypeValues)(System.Convert.ToInt32(curObj["MediaType"])));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Name
    {
        get
        {
            return ((string)(curObj["Name"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsNumberOfBlocksNull
    {
        get
        {
            if ((curObj["NumberOfBlocks"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"Total number of consecutive blocks, each block the size of the value contained in the BlockSize property, which form this storage extent. Total size of the storage extent can be calculated by multiplying the value of the BlockSize property by the value of this property. If the value of BlockSize is 1, this property is the total size of the storage extent.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public ulong NumberOfBlocks
    {
        get
        {
            if ((curObj["NumberOfBlocks"] is null))
            {
                return System.Convert.ToUInt64(0);
            }
            return ((ulong)(curObj["NumberOfBlocks"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Indicates the Win32 Plug and Play device ID of the logical device.  Example: *PNP" +
        "030b")]
    public string PNPDeviceID
    {
        get
        {
            return ((string)(curObj["PNPDeviceID"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"Indicates the specific power-related capabilities of the logical device. The array values, 0=""Unknown"", 1=""Not Supported"" and 2=""Disabled"" are self-explanatory. The value, 3=""Enabled"" indicates that the power management features are currently enabled but the exact feature set is unknown or the information is unavailable. ""Power Saving Modes Entered Automatically"" (4) describes that a device can change its power state based on usage or other criteria. ""Power State Settable"" (5) indicates that the SetPowerState method is supported. ""Power Cycling Supported"" (6) indicates that the SetPowerState method can be invoked with the PowerState input variable set to 5 (""Power Cycle""). ""Timed Power On Supported"" (7) indicates that the SetPowerState method can be invoked with the PowerState input variable set to 5 (""Power Cycle"") and the Time parameter set to a specific date and time, or interval, for power-on.")]
    public PowerManagementCapabilitiesValues[] PowerManagementCapabilities
    {
        get
        {
            System.Array arrEnumVals = ((System.Array)(curObj["PowerManagementCapabilities"]));
            PowerManagementCapabilitiesValues[] enumToRet = new PowerManagementCapabilitiesValues[arrEnumVals.Length];
            int counter = 0;
            for (counter = 0; (counter < arrEnumVals.Length); counter = (counter + 1))
            {
                enumToRet[counter] = ((PowerManagementCapabilitiesValues)(System.Convert.ToInt32(arrEnumVals.GetValue(counter))));
            }
            return enumToRet;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsPowerManagementSupportedNull
    {
        get
        {
            if ((curObj["PowerManagementSupported"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"Boolean indicating that the Device can be power managed - ie, put into a power save state. This boolean does not indicate that power management features are currently enabled, or if enabled, what features are supported. Refer to the PowerManagementCapabilities array for this information. If this boolean is false, the integer value 1, for the string, ""Not Supported"", should be the only entry in the PowerManagementCapabilities array.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool PowerManagementSupported
    {
        get
        {
            if ((curObj["PowerManagementSupported"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["PowerManagementSupported"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The ProviderName property indicates the network path name to the logical device.")]
    public string ProviderName
    {
        get
        {
            return ((string)(curObj["ProviderName"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("A free form string describing the media and/or its use.")]
    public string Purpose
    {
        get
        {
            return ((string)(curObj["Purpose"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsQuotasDisabledNull
    {
        get
        {
            if ((curObj["QuotasDisabled"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The QuotasDisabled property indicates that Quota management is not enabled on thi" +
        "s volume.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool QuotasDisabled
    {
        get
        {
            if ((curObj["QuotasDisabled"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["QuotasDisabled"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsQuotasIncompleteNull
    {
        get
        {
            if ((curObj["QuotasIncomplete"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The QuotasIncomplete property indicates that Quota management was used but has be" +
        "en disabled.  Incomplete refers to the information left in the file system  afte" +
        "r quota management has been disabled.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool QuotasIncomplete
    {
        get
        {
            if ((curObj["QuotasIncomplete"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["QuotasIncomplete"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsQuotasRebuildingNull
    {
        get
        {
            if ((curObj["QuotasRebuilding"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The QuotasRebuilding property indicates an active state signifying that the file " +
        "system is in process of compiling information and setting the disk up for quota " +
        "management.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool QuotasRebuilding
    {
        get
        {
            if ((curObj["QuotasRebuilding"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["QuotasRebuilding"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsSizeNull
    {
        get
        {
            if ((curObj["Size"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The Size property indicates in bytes, the size of the logical disk.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public ulong Size
    {
        get
        {
            if ((curObj["Size"] is null))
            {
                return System.Convert.ToUInt64(0);
            }
            return ((ulong)(curObj["Size"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Status
    {
        get
        {
            return ((string)(curObj["Status"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsStatusInfoNull
    {
        get
        {
            if ((curObj["StatusInfo"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("StatusInfo is a string indicating whether the logical device is in an enabled (va" +
        "lue = 3), disabled (value = 4) or some other (1) or unknown (2) state. If this p" +
        "roperty does not apply to the logical device, the value, 5 (\"Not Applicable\"), s" +
        "hould be used.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public StatusInfoValues StatusInfo
    {
        get
        {
            if ((curObj["StatusInfo"] is null))
            {
                return ((StatusInfoValues)(System.Convert.ToInt32(0)));
            }
            return ((StatusInfoValues)(System.Convert.ToInt32(curObj["StatusInfo"])));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsSupportsDiskQuotasNull
    {
        get
        {
            if ((curObj["SupportsDiskQuotas"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The SupportsDiskQuotas property indicates whether this volume supports disk Quota" +
        "s")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool SupportsDiskQuotas
    {
        get
        {
            if ((curObj["SupportsDiskQuotas"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["SupportsDiskQuotas"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsSupportsFileBasedCompressionNull
    {
        get
        {
            if ((curObj["SupportsFileBasedCompression"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"The SupportsFileBasedCompression property indicates whether the logical disk partition supports file based compression, such as is the case with NTFS. This property is FALSE, when the Compressed property is TRUE.
Values: TRUE or FALSE. If TRUE, the logical disk supports file based compression.")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool SupportsFileBasedCompression
    {
        get
        {
            if ((curObj["SupportsFileBasedCompression"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["SupportsFileBasedCompression"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The scoping System\'s CreationClassName.")]
    public string SystemCreationClassName
    {
        get
        {
            return ((string)(curObj["SystemCreationClassName"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The scoping System\'s Name.")]
    public string SystemName
    {
        get
        {
            return ((string)(curObj["SystemName"]));
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsVolumeDirtyNull
    {
        get
        {
            if ((curObj["VolumeDirty"] is null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description(@"The VolumeDirty property indicates whether the disk requires chkdsk to be run at next boot up time. The property is applicable to only those instances of logical disk that represent a physical disk in the machine. It is not applicable to mapped logical drives. ")]
    [TypeConverter(typeof(WMIValueTypeConverter))]
    public bool VolumeDirty
    {
        get
        {
            if ((curObj["VolumeDirty"] is null))
            {
                return System.Convert.ToBoolean(0);
            }
            return ((bool)(curObj["VolumeDirty"]));
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The VolumeName property indicates the volume name of the logical disk.\nConstraint" +
        "s: Maximum 32 characters")]
    public string VolumeName
    {
        get
        {
            return ((string)(curObj["VolumeName"]));
        }
        set
        {
            curObj["VolumeName"] = value;
            if (((isEmbedded == false)
                        && (AutoCommitProp == true)))
            {
                PrivateLateBoundObject.Put();
            }
        }
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("The VolumeSerialNumber property indicates the volume serial number of the logical" +
        " disk.\nConstraints: Maximum 11 characters\nExample: A8C3-D032")]
    public string VolumeSerialNumber
    {
        get
        {
            return ((string)(curObj["VolumeSerialNumber"]));
        }
    }

    private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam)
    {
        if (((path is not null)
                    && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)))
        {
            return true;
        }
        else
        {
            return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
        }
    }

    private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj)
    {
        if (((theObj is not null)
                    && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)))
        {
            return true;
        }
        else
        {
            System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
            if ((parentClasses is not null))
            {
                int count = 0;
                for (count = 0; (count < parentClasses.Length); count = (count + 1))
                {
                    if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ShouldSerializeAccess()
    {
        if ((this.IsAccessNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeAvailability()
    {
        if ((this.IsAvailabilityNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeBlockSize()
    {
        if ((this.IsBlockSizeNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeCompressed()
    {
        if ((this.IsCompressedNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeConfigManagerErrorCode()
    {
        if ((this.IsConfigManagerErrorCodeNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeConfigManagerUserConfig()
    {
        if ((this.IsConfigManagerUserConfigNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeDriveType()
    {
        if ((this.IsDriveTypeNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeErrorCleared()
    {
        if ((this.IsErrorClearedNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeFreeSpace()
    {
        if ((this.IsFreeSpaceNull == false))
        {
            return true;
        }
        return false;
    }

    // Konverterar ett givet datum/tid med DMTF-format till System.DateTime-objekt.
    static System.DateTime ToDateTime(string dmtfDate)
    {
        System.DateTime initializer = System.DateTime.MinValue;
        int year = initializer.Year;
        int month = initializer.Month;
        int day = initializer.Day;
        int hour = initializer.Hour;
        int minute = initializer.Minute;
        int second = initializer.Second;
        long ticks = 0;
        string dmtf = dmtfDate;
        System.DateTime datetime = System.DateTime.MinValue;
        string tempString = string.Empty;
        if ((dmtf is null))
        {
            throw new System.ArgumentOutOfRangeException();
        }
        if ((dmtf.Length == 0))
        {
            throw new System.ArgumentOutOfRangeException();
        }
        if ((dmtf.Length != 25))
        {
            throw new System.ArgumentOutOfRangeException();
        }
        try
        {
            tempString = dmtf.Substring(0, 4);
            if (("****" != tempString))
            {
                year = int.Parse(tempString);
            }
            tempString = dmtf.Substring(4, 2);
            if (("**" != tempString))
            {
                month = int.Parse(tempString);
            }
            tempString = dmtf.Substring(6, 2);
            if (("**" != tempString))
            {
                day = int.Parse(tempString);
            }
            tempString = dmtf.Substring(8, 2);
            if (("**" != tempString))
            {
                hour = int.Parse(tempString);
            }
            tempString = dmtf.Substring(10, 2);
            if (("**" != tempString))
            {
                minute = int.Parse(tempString);
            }
            tempString = dmtf.Substring(12, 2);
            if (("**" != tempString))
            {
                second = int.Parse(tempString);
            }
            tempString = dmtf.Substring(15, 6);
            if (("******" != tempString))
            {
                ticks = (long.Parse(tempString) * ((long)((System.TimeSpan.TicksPerMillisecond / 1000))));
            }
            if (((((((((year < 0)
                        || (month < 0))
                        || (day < 0))
                        || (hour < 0))
                        || (minute < 0))
                        || (minute < 0))
                        || (second < 0))
                        || (ticks < 0)))
            {
                throw new System.ArgumentOutOfRangeException();
            }
        }
        catch (System.Exception e)
        {
            throw new System.ArgumentOutOfRangeException(null, e.Message);
        }
        datetime = new System.DateTime(year, month, day, hour, minute, second, 0);
        datetime = datetime.AddTicks(ticks);
        System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
        int UTCOffset = 0;
        int OffsetToBeAdjusted = 0;
        long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
        tempString = dmtf.Substring(22, 3);
        if ((tempString != "******"))
        {
            tempString = dmtf.Substring(21, 4);
            try
            {
                UTCOffset = int.Parse(tempString);
            }
            catch (System.Exception e)
            {
                throw new System.ArgumentOutOfRangeException(null, e.Message);
            }
            OffsetToBeAdjusted = ((int)((OffsetMins - UTCOffset)));
            datetime = datetime.AddMinutes(((double)(OffsetToBeAdjusted)));
        }
        return datetime;
    }

    // Konverterar ett givet System.DateTime-objekt till DMTF-datum/tid.
    static string ToDmtfDateTime(System.DateTime date)
    {
        string utcString = string.Empty;
        System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(date);
        long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
        if ((System.Math.Abs(OffsetMins) > 999))
        {
            date = date.ToUniversalTime();
            utcString = "+000";
        }
        else
        {
            if ((tickOffset.Ticks >= 0))
            {
                utcString = string.Concat("+", ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute))).ToString().PadLeft(3, '0'));
            }
            else
            {
                string strTemp = ((long)(OffsetMins)).ToString();
                utcString = string.Concat("-", strTemp.Substring(1, (strTemp.Length - 1)).PadLeft(3, '0'));
            }
        }
        string dmtfDateTime = ((int)(date.Year)).ToString().PadLeft(4, '0');
        dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Month)).ToString().PadLeft(2, '0'));
        dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Day)).ToString().PadLeft(2, '0'));
        dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Hour)).ToString().PadLeft(2, '0'));
        dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Minute)).ToString().PadLeft(2, '0'));
        dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Second)).ToString().PadLeft(2, '0'));
        dmtfDateTime = string.Concat(dmtfDateTime, ".");
        System.DateTime dtTemp = new System.DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
        long microsec = ((long)((((date.Ticks - dtTemp.Ticks)
                    * 1000)
                    / System.TimeSpan.TicksPerMillisecond)));
        string strMicrosec = ((long)(microsec)).ToString();
        if ((strMicrosec.Length > 6))
        {
            strMicrosec = strMicrosec.Substring(0, 6);
        }
        dmtfDateTime = string.Concat(dmtfDateTime, strMicrosec.PadLeft(6, '0'));
        dmtfDateTime = string.Concat(dmtfDateTime, utcString);
        return dmtfDateTime;
    }

    private bool ShouldSerializeInstallDate()
    {
        if ((this.IsInstallDateNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeLastErrorCode()
    {
        if ((this.IsLastErrorCodeNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeMaximumComponentLength()
    {
        if ((this.IsMaximumComponentLengthNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeMediaType()
    {
        if ((this.IsMediaTypeNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeNumberOfBlocks()
    {
        if ((this.IsNumberOfBlocksNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializePowerManagementSupported()
    {
        if ((this.IsPowerManagementSupportedNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeQuotasDisabled()
    {
        if ((this.IsQuotasDisabledNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeQuotasIncomplete()
    {
        if ((this.IsQuotasIncompleteNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeQuotasRebuilding()
    {
        if ((this.IsQuotasRebuildingNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeSize()
    {
        if ((this.IsSizeNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeStatusInfo()
    {
        if ((this.IsStatusInfoNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeSupportsDiskQuotas()
    {
        if ((this.IsSupportsDiskQuotasNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeSupportsFileBasedCompression()
    {
        if ((this.IsSupportsFileBasedCompressionNull == false))
        {
            return true;
        }
        return false;
    }

    private bool ShouldSerializeVolumeDirty()
    {
        if ((this.IsVolumeDirtyNull == false))
        {
            return true;
        }
        return false;
    }

    private void ResetVolumeName()
    {
        curObj["VolumeName"] = null;
        if (((isEmbedded == false)
                    && (AutoCommitProp == true)))
        {
            PrivateLateBoundObject.Put();
        }
    }

    [Browsable(true)]
    public void CommitObject()
    {
        if ((isEmbedded == false))
        {
            PrivateLateBoundObject.Put();
        }
    }

    [Browsable(true)]
    public void CommitObject(System.Management.PutOptions putOptions)
    {
        if ((isEmbedded == false))
        {
            PrivateLateBoundObject.Put(putOptions);
        }
    }

    private void Initialize()
    {
        AutoCommitProp = true;
        isEmbedded = false;
    }

    private static string ConstructPath(string keyDeviceID)
    {
        string strPath = "root\\CimV2:Win32_Logicaldisk";
        strPath = string.Concat(strPath, string.Concat(".DeviceID=", string.Concat("\"", string.Concat(keyDeviceID, "\""))));
        return strPath;
    }

    private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions)
    {
        Initialize();
        if ((path is not null))
        {
            if ((CheckIfProperClass(mgmtScope, path, getOptions) != true))
            {
                throw new System.ArgumentException("Klassnamnet matchar inte.");
            }
        }
        PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
        PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
        curObj = PrivateLateBoundObject;
    }

    // Olika överlagringar av hjälp om GetInstances() i WMI-klassens uppräkningsinstanser.
    public static LogicaldiskCollection GetInstances()
    {
        return GetInstances(null, null, null);
    }

    public static LogicaldiskCollection GetInstances(string condition)
    {
        return GetInstances(null, condition, null);
    }

    public static LogicaldiskCollection GetInstances(string[] selectedProperties)
    {
        return GetInstances(null, null, selectedProperties);
    }

    public static LogicaldiskCollection GetInstances(string condition, string[] selectedProperties)
    {
        return GetInstances(null, condition, selectedProperties);
    }

    public static LogicaldiskCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions)
    {
        if ((mgmtScope is null))
        {
            if ((statMgmtScope is null))
            {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = "root\\CimV2";
            }
            else
            {
                mgmtScope = statMgmtScope;
            }
        }
        System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
        pathObj.ClassName = "Win32_Logicaldisk";
        pathObj.NamespacePath = "root\\CimV2";
        System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
        if ((enumOptions is null))
        {
            enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
        }
        return new LogicaldiskCollection(clsObject.GetInstances(enumOptions));
    }

    public static LogicaldiskCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition)
    {
        return GetInstances(mgmtScope, condition, null);
    }

    public static LogicaldiskCollection GetInstances(System.Management.ManagementScope mgmtScope, string[] selectedProperties)
    {
        return GetInstances(mgmtScope, null, selectedProperties);
    }

    public static LogicaldiskCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, string[] selectedProperties)
    {
        if ((mgmtScope is null))
        {
            if ((statMgmtScope is null))
            {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = "root\\CimV2";
            }
            else
            {
                mgmtScope = statMgmtScope;
            }
        }
        System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_Logicaldisk", condition, selectedProperties));
        System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
        enumOptions.EnsureLocatable = true;
        ObjectSearcher.Options = enumOptions;
        return new LogicaldiskCollection(ObjectSearcher.Get());
    }

    [Browsable(true)]
    public static Logicaldisk CreateInstance()
    {
        System.Management.ManagementScope mgmtScope = null;
        if ((statMgmtScope is null))
        {
            mgmtScope = new System.Management.ManagementScope();
            mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
        }
        else
        {
            mgmtScope = statMgmtScope;
        }
        System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
        System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
        return new Logicaldisk(tmpMgmtClass.CreateInstance());
    }

    [Browsable(true)]
    public void Delete()
    {
        PrivateLateBoundObject.Delete();
    }

    public uint Chkdsk(bool FixErrors, bool ForceDismount, bool OkToRunAtBootUp, bool RecoverBadSectors, bool SkipFolderCycle, bool VigorousIndexCheck)
    {
        if ((isEmbedded == false))
        {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("Chkdsk");
            inParams["FixErrors"] = ((bool)(FixErrors));
            inParams["ForceDismount"] = ((bool)(ForceDismount));
            inParams["OkToRunAtBootUp"] = ((bool)(OkToRunAtBootUp));
            inParams["RecoverBadSectors"] = ((bool)(RecoverBadSectors));
            inParams["SkipFolderCycle"] = ((bool)(SkipFolderCycle));
            inParams["VigorousIndexCheck"] = ((bool)(VigorousIndexCheck));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Chkdsk", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else
        {
            return System.Convert.ToUInt32(0);
        }
    }

    public static uint ExcludeFromAutochk(string[] LogicalDisk)
    {
        bool IsMethodStatic = true;
        if ((IsMethodStatic == true))
        {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass classObj = new System.Management.ManagementClass(statMgmtScope, mgmtPath, null);
            inParams = classObj.GetMethodParameters("ExcludeFromAutochk");
            inParams["LogicalDisk"] = ((string[])(LogicalDisk));
            System.Management.ManagementBaseObject outParams = classObj.InvokeMethod("ExcludeFromAutochk", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else
        {
            return System.Convert.ToUInt32(0);
        }
    }

    public uint Reset()
    {
        if ((isEmbedded == false))
        {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Reset", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else
        {
            return System.Convert.ToUInt32(0);
        }
    }

    public static uint ScheduleAutoChk(string[] LogicalDisk)
    {
        bool IsMethodStatic = true;
        if ((IsMethodStatic == true))
        {
            System.Management.ManagementBaseObject inParams = null;
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass classObj = new System.Management.ManagementClass(statMgmtScope, mgmtPath, null);
            inParams = classObj.GetMethodParameters("ScheduleAutoChk");
            inParams["LogicalDisk"] = ((string[])(LogicalDisk));
            System.Management.ManagementBaseObject outParams = classObj.InvokeMethod("ScheduleAutoChk", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else
        {
            return System.Convert.ToUInt32(0);
        }
    }

    public uint SetPowerState(ushort PowerState, System.DateTime Time)
    {
        if ((isEmbedded == false))
        {
            System.Management.ManagementBaseObject inParams = null;
            inParams = PrivateLateBoundObject.GetMethodParameters("SetPowerState");
            inParams["PowerState"] = ((ushort)(PowerState));
            inParams["Time"] = ToDmtfDateTime(((System.DateTime)(Time)));
            System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetPowerState", inParams, null);
            return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
        }
        else
        {
            return System.Convert.ToUInt32(0);
        }
    }

    public enum AccessValues
    {

        Unknown0 = 0,

        Readable = 1,

        Writeable = 2,

        Read_Write_Supported = 3,

        Write_Once = 4,

        NULL_ENUM_VALUE = 5,
    }

    public enum AvailabilityValues
    {

        Other0 = 1,

        Unknown0 = 2,

        Running_Full_Power = 3,

        Warning = 4,

        In_Test = 5,

        Not_Applicable = 6,

        Power_Off = 7,

        Off_Line = 8,

        Off_Duty = 9,

        Degraded = 10,

        Not_Installed = 11,

        Install_Error = 12,

        Power_Save_Unknown = 13,

        Power_Save_Low_Power_Mode = 14,

        Power_Save_Standby = 15,

        Power_Cycle = 16,

        Power_Save_Warning = 17,

        Paused = 18,

        Not_Ready = 19,

        Not_Configured = 20,

        Quiesced = 21,

        NULL_ENUM_VALUE = 0,
    }

    public enum ConfigManagerErrorCodeValues
    {

        This_device_is_working_properly_ = 0,

        This_device_is_not_configured_correctly_ = 1,

        Windows_cannot_load_the_driver_for_this_device_ = 2,

        The_driver_for_this_device_might_be_corrupted_or_your_system_may_be_running_low_on_memory_or_other_resources_ = 3,

        This_device_is_not_working_properly_One_of_its_drivers_or_your_registry_might_be_corrupted_ = 4,

        The_driver_for_this_device_needs_a_resource_that_Windows_cannot_manage_ = 5,

        The_boot_configuration_for_this_device_conflicts_with_other_devices_ = 6,

        Cannot_filter_ = 7,

        The_driver_loader_for_the_device_is_missing_ = 8,

        This_device_is_not_working_properly_because_the_controlling_firmware_is_reporting_the_resources_for_the_device_incorrectly_ = 9,

        This_device_cannot_start_ = 10,

        This_device_failed_ = 11,

        This_device_cannot_find_enough_free_resources_that_it_can_use_ = 12,

        Windows_cannot_verify_this_device_s_resources_ = 13,

        This_device_cannot_work_properly_until_you_restart_your_computer_ = 14,

        This_device_is_not_working_properly_because_there_is_probably_a_re_enumeration_problem_ = 15,

        Windows_cannot_identify_all_the_resources_this_device_uses_ = 16,

        This_device_is_asking_for_an_unknown_resource_type_ = 17,

        Reinstall_the_drivers_for_this_device_ = 18,

        Failure_using_the_VxD_loader_ = 19,

        Your_registry_might_be_corrupted_ = 20,

        System_failure_Try_changing_the_driver_for_this_device_If_that_does_not_work_see_your_hardware_documentation_Windows_is_removing_this_device_ = 21,

        This_device_is_disabled_ = 22,

        System_failure_Try_changing_the_driver_for_this_device_If_that_doesn_t_work_see_your_hardware_documentation_ = 23,

        This_device_is_not_present_is_not_working_properly_or_does_not_have_all_its_drivers_installed_ = 24,

        Windows_is_still_setting_up_this_device_ = 25,

        Windows_is_still_setting_up_this_device_0 = 26,

        This_device_does_not_have_valid_log_configuration_ = 27,

        The_drivers_for_this_device_are_not_installed_ = 28,

        This_device_is_disabled_because_the_firmware_of_the_device_did_not_give_it_the_required_resources_ = 29,

        This_device_is_using_an_Interrupt_Request_IRQ_resource_that_another_device_is_using_ = 30,

        This_device_is_not_working_properly_because_Windows_cannot_load_the_drivers_required_for_this_device_ = 31,

        NULL_ENUM_VALUE = 32,
    }

    public enum DriveTypeValues
    {

        Unknown0 = 0,

        No_Root_Directory = 1,

        Removable_Disk = 2,

        Local_Disk = 3,

        Network_Drive = 4,

        Compact_Disc = 5,

        RAM_Disk = 6,

        NULL_ENUM_VALUE = 7,
    }

    public enum MediaTypeValues
    {

        Format_is_unknown = 0,

        Val_5_Inch_Floppy_Disk = 1,

        Val_3_Inch_Floppy_Disk = 2,

        Val_3_Inch_Floppy_Disk0 = 3,

        Val_3_Inch_Floppy_Disk1 = 4,

        Val_3_Inch_Floppy_Disk2 = 5,

        Val_5_Inch_Floppy_Disk0 = 6,

        Val_5_Inch_Floppy_Disk1 = 7,

        Val_5_Inch_Floppy_Disk2 = 8,

        Val_5_Inch_Floppy_Disk3 = 9,

        Val_5_Inch_Floppy_Disk4 = 10,

        Removable_media_other_than_floppy = 11,

        Fixed_hard_disk_media = 12,

        Val_3_Inch_Floppy_Disk3 = 13,

        Val_3_Inch_Floppy_Disk4 = 14,

        Val_5_Inch_Floppy_Disk5 = 15,

        Val_5_Inch_Floppy_Disk6 = 16,

        Val_3_Inch_Floppy_Disk5 = 17,

        Val_3_Inch_Floppy_Disk6 = 18,

        Val_5_Inch_Floppy_Disk7 = 19,

        Val_3_Inch_Floppy_Disk7 = 20,

        Val_3_Inch_Floppy_Disk8 = 21,

        Val_8_Inch_Floppy_Disk = 22,

        NULL_ENUM_VALUE = 23,
    }

    public enum PowerManagementCapabilitiesValues
    {

        Unknown0 = 0,

        Not_Supported = 1,

        Disabled = 2,

        Enabled = 3,

        Power_Saving_Modes_Entered_Automatically = 4,

        Power_State_Settable = 5,

        Power_Cycling_Supported = 6,

        Timed_Power_On_Supported = 7,

        NULL_ENUM_VALUE = 8,
    }

    public enum StatusInfoValues
    {

        Other0 = 1,

        Unknown0 = 2,

        Enabled = 3,

        Disabled = 4,

        Not_Applicable = 5,

        NULL_ENUM_VALUE = 0,
    }

    // Implementering av uppräknare för klassens uppräkningsinstanser.
    public class LogicaldiskCollection : object, ICollection
    {

        private ManagementObjectCollection privColObj;

        public LogicaldiskCollection(ManagementObjectCollection objCollection)
        {
            privColObj = objCollection;
        }

        public virtual int Count
        {
            get
            {
                return privColObj.Count;
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return privColObj.IsSynchronized;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public virtual void CopyTo(System.Array array, int index)
        {
            privColObj.CopyTo(array, index);
            int nCtr;
            for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1))
            {
                array.SetValue(new Logicaldisk(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
            }
        }

        public virtual System.Collections.IEnumerator GetEnumerator()
        {
            return new LogicaldiskEnumerator(privColObj.GetEnumerator());
        }

        public class LogicaldiskEnumerator : object, System.Collections.IEnumerator
        {

            private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;

            public LogicaldiskEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum)
            {
                privObjEnum = objEnum;
            }

            public virtual object Current
            {
                get
                {
                    return new Logicaldisk(((System.Management.ManagementObject)(privObjEnum.Current)));
                }
            }

            public virtual bool MoveNext()
            {
                return privObjEnum.MoveNext();
            }

            public virtual void Reset()
            {
                privObjEnum.Reset();
            }
        }
    }

    // TypeConverter som hanterar Null-värden för ValueType-egenskaper
    public class WMIValueTypeConverter : TypeConverter
    {

        private TypeConverter baseConverter;

        private System.Type baseType;

        public WMIValueTypeConverter(System.Type inBaseType)
        {
            baseConverter = TypeDescriptor.GetConverter(inBaseType);
            baseType = inBaseType;
        }

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType)
        {
            return baseConverter.CanConvertFrom(context, srcType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            return baseConverter.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return baseConverter.ConvertFrom(context, culture, value);
        }

        public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary)
        {
            return baseConverter.CreateInstance(context, dictionary);
        }

        public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return baseConverter.GetCreateInstanceSupported(context);
        }

        public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar)
        {
            return baseConverter.GetProperties(context, value, attributeVar);
        }

        public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return baseConverter.GetPropertiesSupported(context);
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            return baseConverter.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
        {
            return baseConverter.GetStandardValuesExclusive(context);
        }

        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return baseConverter.GetStandardValuesSupported(context);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            if ((baseType.BaseType == typeof(System.Enum)))
            {
                if ((value.GetType() == destinationType))
                {
                    return value;
                }
                if ((((value is null)
                            && (context is not null))
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                {
                    return "NULL_ENUM_VALUE";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (((baseType == typeof(bool))
                        && (baseType.BaseType == typeof(System.ValueType))))
            {
                if ((((value is null)
                            && (context is not null))
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (((context is not null)
                        && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
            {
                return "";
            }
            return baseConverter.ConvertTo(context, culture, value, destinationType);
        }
    }

    // En inbäddad klass som representerar WMI-systemegenskaper.
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class ManagementSystemProperties
    {

        private System.Management.ManagementBaseObject PrivateLateBoundObject;

        public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject)
        {
            PrivateLateBoundObject = ManagedObject;
        }

        [Browsable(true)]
        public int GENUS
        {
            get
            {
                return ((int)(PrivateLateBoundObject["__GENUS"]));
            }
        }

        [Browsable(true)]
        public string CLASS
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__CLASS"]));
            }
        }

        [Browsable(true)]
        public string SUPERCLASS
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
            }
        }

        [Browsable(true)]
        public string DYNASTY
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__DYNASTY"]));
            }
        }

        [Browsable(true)]
        public string RELPATH
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__RELPATH"]));
            }
        }

        [Browsable(true)]
        public int PROPERTY_COUNT
        {
            get
            {
                return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
            }
        }

        [Browsable(true)]
        public string[] DERIVATION
        {
            get
            {
                return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
            }
        }

        [Browsable(true)]
        public string SERVER
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__SERVER"]));
            }
        }

        [Browsable(true)]
        public string NAMESPACE
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
            }
        }

        [Browsable(true)]
        public string PATH
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__PATH"]));
            }
        }
    }
}
