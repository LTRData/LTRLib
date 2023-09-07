#pragma warning disable

namespace ROOT.CIMV2.Win32
{
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
    // En EarlyBound-klass genererades för WMI-klassen.Win32_Volume
    [GeneratedCode("mgmtclassgen", "")]
    public class Volume : System.ComponentModel.Component {
        
        // En privat egenskap som ska innehålla WMI-namnområdet där klassen finns.
        private static string CreatedWmiNamespace = "Root\\CIMV2";
        
        // En privat egenskap som ska innehålla namnet på den WMI-klass som skapade den här klassen.
        private static string CreatedClassName = "Win32_Volume";
        
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
        public Volume() {
            this.InitializeObject(null, null, null);
        }
        
        public Volume(string keyDeviceID) {
            this.InitializeObject(null, new System.Management.ManagementPath(Volume.ConstructPath(keyDeviceID)), null);
        }
        
        public Volume(System.Management.ManagementScope mgmtScope, string keyDeviceID) {
            this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(Volume.ConstructPath(keyDeviceID)), null);
        }
        
        public Volume(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(null, path, getOptions);
        }
        
        public Volume(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path) {
            this.InitializeObject(mgmtScope, path, null);
        }
        
        public Volume(System.Management.ManagementPath path) {
            this.InitializeObject(null, path, null);
        }
        
        public Volume(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(mgmtScope, path, getOptions);
        }
        
        public Volume(System.Management.ManagementObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                PrivateLateBoundObject = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
                curObj = PrivateLateBoundObject;
            }
            else {
                throw new System.ArgumentException("Klassnamnet matchar inte.");
            }
        }
        
        public Volume(System.Management.ManagementBaseObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                embeddedObj = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(theObject);
                curObj = embeddedObj;
                isEmbedded = true;
            }
            else {
                throw new System.ArgumentException("Klassnamnet matchar inte.");
            }
        }
        
        // En egenskap som returnerar WMI-klassens namnområde.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OriginatingNamespace {
            get {
                return "Root\\CIMV2";
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName {
            get {
                string strRet = CreatedClassName;
                if ((curObj is not null)) {
                    if ((curObj.ClassPath is not null)) {
                        strRet = ((string)(curObj["__CLASS"]));
                        if (((strRet is null) 
                                    || (strRet == string.Empty))) {
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
        public ManagementSystemProperties SystemProperties {
            get {
                return PrivateSystemProperties;
            }
        }
        
        // En egenskap som returnerar det underliggande lateBound-objektet.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementBaseObject LateBoundObject {
            get {
                return curObj;
            }
        }
        
        // Objektets ManagementScope.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementScope Scope {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Scope;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    PrivateLateBoundObject.Scope = value;
                }
            }
        }
        
        // Egenskap som visar aktiverat beteende för WMI-objektet. Om detta är True sparas WMI-objektet automatiskt efter varje egenskapsändring (d.v.s. Put() anropas efter en egenskapsändring).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit {
            get {
                return AutoCommitProp;
            }
            set {
                AutoCommitProp = value;
            }
        }
        
        // ManagementPath för det underliggande WMI-objektet.
        [Browsable(true)]
        public System.Management.ManagementPath Path {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Path;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    if ((CheckIfProperClass(null, value, null) != true)) {
                        throw new System.ArgumentException("Klassnamnet matchar inte.");
                    }
                    PrivateLateBoundObject.Path = value;
                }
            }
        }
        
        // En offentlig och statisk områdesegenskap som används i olika metoder.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static System.Management.ManagementScope StaticScope {
            get {
                return statMgmtScope;
            }
            set {
                statMgmtScope = value;
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAccessNull {
            get {
                if ((curObj["Access"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Access describes whether the media is readable (value=1), writeable (value=2), or" +
            " both (value=3). \"Unknown\" (0) and \"Write Once\" (4) can also be defined.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public AccessValues Access {
            get {
                if ((curObj["Access"] is null)) {
                    return ((AccessValues)(System.Convert.ToInt32(5)));
                }
                return ((AccessValues)(System.Convert.ToInt32(curObj["Access"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAutomountNull {
            get {
                if ((curObj["Automount"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"If TRUE, the Automount property indicates that the volume will be mounted to the file system automatically when the first I/O is issued. Otherwise it will not be mounted until explicitly mounted with the Mount method or by adding a drive letter or mount point.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool Automount {
            get {
                if ((curObj["Automount"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["Automount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAvailabilityNull {
            get {
                if ((curObj["Availability"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The availability and status of the device.  For example, the Availability property indicates that the device is running and has full power (value=3), or is in a warning (4), test (5), degraded (10) or power save state (values 13-15 and 17). Regarding the power saving states, these are defined as follows: Value 13 (""Power Save - Unknown"") indicates that the device is known to be in a power save mode, but its exact status in this mode is unknown; 14 (""Power Save - Low Power Mode"") indicates that the device is in a power save state but still functioning, and may exhibit degraded performance; 15 (""Power Save - Standby"") describes that the device is not functioning but could be brought to full power 'quickly'; and value 17 (""Power Save - Warning"") indicates that the device is in a warning state, though also in a power save mode.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public AvailabilityValues Availability {
            get {
                if ((curObj["Availability"] is null)) {
                    return ((AvailabilityValues)(System.Convert.ToInt32(0)));
                }
                return ((AvailabilityValues)(System.Convert.ToInt32(curObj["Availability"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBlockSizeNull {
            get {
                if ((curObj["BlockSize"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Size in bytes of the blocks which form this StorageExtent. If variable block size, then the maximum block size in bytes should be specified. If the block size is unknown or if a block concept is not valid (for example, for Aggregate Extents, Memory or LogicalDisks), enter a 1.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong BlockSize {
            get {
                if ((curObj["BlockSize"] is null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["BlockSize"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBootVolumeNull {
            get {
                if ((curObj["BootVolume"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The BootVolume property indicates whether the volume contains the currently runni" +
            "ng operating system files.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool BootVolume {
            get {
                if ((curObj["BootVolume"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["BootVolume"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCapacityNull {
            get {
                if ((curObj["Capacity"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Capacity property indicates in bytes, the size of the volume.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong Capacity {
            get {
                if ((curObj["Capacity"] is null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["Capacity"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Caption {
            get {
                return ((string)(curObj["Caption"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCompressedNull {
            get {
                if ((curObj["Compressed"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("If TRUE, the volume exists as a single compressed entity, such as a DoubleSpace v" +
            "olume.If file based compression is supported (such as on NTFS), this property wi" +
            "ll be FALSE.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool Compressed {
            get {
                if ((curObj["Compressed"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["Compressed"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConfigManagerErrorCodeNull {
            get {
                if ((curObj["ConfigManagerErrorCode"] is null)) {
                    return true;
                }
                else {
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
        public ConfigManagerErrorCodeValues ConfigManagerErrorCode {
            get {
                if ((curObj["ConfigManagerErrorCode"] is null)) {
                    return ((ConfigManagerErrorCodeValues)(System.Convert.ToInt32(32)));
                }
                return ((ConfigManagerErrorCodeValues)(System.Convert.ToInt32(curObj["ConfigManagerErrorCode"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConfigManagerUserConfigNull {
            get {
                if ((curObj["ConfigManagerUserConfig"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates whether the device is using a user-defined configuration.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool ConfigManagerUserConfig {
            get {
                if ((curObj["ConfigManagerUserConfig"] is null)) {
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
        public string CreationClassName {
            get {
                return ((string)(curObj["CreationClassName"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Description {
            get {
                return ((string)(curObj["Description"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DeviceID property contains a string uniquely identifying the volume on the sy" +
            "stem.")]
        public string DeviceID {
            get {
                return ((string)(curObj["DeviceID"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDirtyBitSetNull {
            get {
                if ((curObj["DirtyBitSet"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates if the ChkDsk method should be run at next boot up.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool DirtyBitSet {
            get {
                if ((curObj["DirtyBitSet"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["DirtyBitSet"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DriveLetter property indicates the drive letter assigned to the volume. This " +
            "property is NULL for volumes without drive letters.")]
        public string DriveLetter {
            get {
                return ((string)(curObj["DriveLetter"]));
            }
            set {
                curObj["DriveLetter"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDriveTypeNull {
            get {
                if ((curObj["DriveType"] is null)) {
                    return true;
                }
                else {
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
        public DriveTypeValues DriveType {
            get {
                if ((curObj["DriveType"] is null)) {
                    return ((DriveTypeValues)(System.Convert.ToInt32(7)));
                }
                return ((DriveTypeValues)(System.Convert.ToInt32(curObj["DriveType"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsErrorClearedNull {
            get {
                if ((curObj["ErrorCleared"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("ErrorCleared is a boolean property indicating that the error reported in LastErro" +
            "rCode property is now cleared.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool ErrorCleared {
            get {
                if ((curObj["ErrorCleared"] is null)) {
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
        public string ErrorDescription {
            get {
                return ((string)(curObj["ErrorDescription"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("ErrorMethodology is a free-form string describing the type of error detection and" +
            " correction supported by this storage extent.")]
        public string ErrorMethodology {
            get {
                return ((string)(curObj["ErrorMethodology"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The FileSystem property indicates the file system on the logical disk. Example: N" +
            "TFS")]
        public string FileSystem {
            get {
                return ((string)(curObj["FileSystem"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFreeSpaceNull {
            get {
                if ((curObj["FreeSpace"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The FreeSpace property indicates in bytes how much free space is available on the" +
            " volume.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong FreeSpace {
            get {
                if ((curObj["FreeSpace"] is null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["FreeSpace"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsIndexingEnabledNull {
            get {
                if ((curObj["IndexingEnabled"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("If context indexing is enabled this property will be TRUE")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool IndexingEnabled {
            get {
                if ((curObj["IndexingEnabled"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["IndexingEnabled"]));
            }
            set {
                curObj["IndexingEnabled"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInstallDateNull {
            get {
                if ((curObj["InstallDate"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime InstallDate {
            get {
                if ((curObj["InstallDate"] is not null)) {
                    return ToDateTime(((string)(curObj["InstallDate"])));
                }
                else {
                    return System.DateTime.MinValue;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Label property indicates the volume name of the logical disk. Constraints: Ma" +
            "ximum 32 characters. This property is NULL for a volume without a label.")]
        public string Label {
            get {
                return ((string)(curObj["Label"]));
            }
            set {
                curObj["Label"] = value;
                if (((isEmbedded == false) 
                            && (AutoCommitProp == true))) {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLastErrorCodeNull {
            get {
                if ((curObj["LastErrorCode"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("LastErrorCode captures the last error code reported by the logical device.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint LastErrorCode {
            get {
                if ((curObj["LastErrorCode"] is null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["LastErrorCode"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMaximumFileNameLengthNull {
            get {
                if ((curObj["MaximumFileNameLength"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Maximum length of a filename component supported by the Windows drive. A filename component is that portion of a filename between backslashes. The value can be used to indicate that long names are supported by the specified file system. For example, for a FAT file system supporting long names, the function stores the value 255, rather than the previous 8.3 indicator. Long names can also be supported on systems that use the NTFS file system. Example: 255")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MaximumFileNameLength {
            get {
                if ((curObj["MaximumFileNameLength"] is null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MaximumFileNameLength"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Name {
            get {
                return ((string)(curObj["Name"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNumberOfBlocksNull {
            get {
                if ((curObj["NumberOfBlocks"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Total number of consecutive blocks, each block the size of the value contained in the BlockSize property, which form this storage extent. Total size of the storage extent can be calculated by multiplying the value of the BlockSize property by the value of this property. If the value of BlockSize is 1, this property is the total size of the storage extent.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong NumberOfBlocks {
            get {
                if ((curObj["NumberOfBlocks"] is null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["NumberOfBlocks"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPageFilePresentNull {
            get {
                if ((curObj["PageFilePresent"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PageFilePresent property indicates whether the volume contains a system memor" +
            "y paging file.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool PageFilePresent {
            get {
                if ((curObj["PageFilePresent"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["PageFilePresent"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates the Win32 Plug and Play device ID of the logical device.  Example: *PNP" +
            "030b")]
        public string PNPDeviceID {
            get {
                return ((string)(curObj["PNPDeviceID"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Indicates the specific power-related capabilities of the logical device. The array values, 0=""Unknown"", 1=""Not Supported"" and 2=""Disabled"" are self-explanatory. The value, 3=""Enabled"" indicates that the power management features are currently enabled but the exact feature set is unknown or the information is unavailable. ""Power Saving Modes Entered Automatically"" (4) describes that a device can change its power state based on usage or other criteria. ""Power State Settable"" (5) indicates that the SetPowerState method is supported. ""Power Cycling Supported"" (6) indicates that the SetPowerState method can be invoked with the PowerState input variable set to 5 (""Power Cycle""). ""Timed Power On Supported"" (7) indicates that the SetPowerState method can be invoked with the PowerState input variable set to 5 (""Power Cycle"") and the Time parameter set to a specific date and time, or interval, for power-on.")]
        public PowerManagementCapabilitiesValues[] PowerManagementCapabilities {
            get {
                System.Array arrEnumVals = ((System.Array)(curObj["PowerManagementCapabilities"]));
                PowerManagementCapabilitiesValues[] enumToRet = new PowerManagementCapabilitiesValues[arrEnumVals.Length];
                int counter = 0;
                for (counter = 0; (counter < arrEnumVals.Length); counter = (counter + 1)) {
                    enumToRet[counter] = ((PowerManagementCapabilitiesValues)(System.Convert.ToInt32(arrEnumVals.GetValue(counter))));
                }
                return enumToRet;
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPowerManagementSupportedNull {
            get {
                if ((curObj["PowerManagementSupported"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Boolean indicating that the Device can be power managed - ie, put into a power save state. This boolean does not indicate that power management features are currently enabled, or if enabled, what features are supported. Refer to the PowerManagementCapabilities array for this information. If this boolean is false, the integer value 1, for the string, ""Not Supported"", should be the only entry in the PowerManagementCapabilities array.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool PowerManagementSupported {
            get {
                if ((curObj["PowerManagementSupported"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["PowerManagementSupported"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("A free form string describing the media and/or its use.")]
        public string Purpose {
            get {
                return ((string)(curObj["Purpose"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotasEnabledNull {
            get {
                if ((curObj["QuotasEnabled"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotasEnabled property indicates that quota management is enabled for this vo" +
            "lume.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool QuotasEnabled {
            get {
                if ((curObj["QuotasEnabled"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["QuotasEnabled"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotasIncompleteNull {
            get {
                if ((curObj["QuotasIncomplete"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotasIncomplete property indicates that quota management was used but has be" +
            "en disabled. Incomplete refers to the information left in the file system after " +
            "quota management was disabled.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool QuotasIncomplete {
            get {
                if ((curObj["QuotasIncomplete"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["QuotasIncomplete"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotasRebuildingNull {
            get {
                if ((curObj["QuotasRebuilding"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotasRebuilding property indicates that the file system is in the active pro" +
            "cess of compiling information and setting the disk up for quota management.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool QuotasRebuilding {
            get {
                if ((curObj["QuotasRebuilding"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["QuotasRebuilding"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSerialNumberNull {
            get {
                if ((curObj["SerialNumber"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The SerialNumber property indicates the serial number of the volume. Example: A8C" +
            "3D032")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint SerialNumber {
            get {
                if ((curObj["SerialNumber"] is null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["SerialNumber"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Status {
            get {
                return ((string)(curObj["Status"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsStatusInfoNull {
            get {
                if ((curObj["StatusInfo"] is null)) {
                    return true;
                }
                else {
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
        public StatusInfoValues StatusInfo {
            get {
                if ((curObj["StatusInfo"] is null)) {
                    return ((StatusInfoValues)(System.Convert.ToInt32(0)));
                }
                return ((StatusInfoValues)(System.Convert.ToInt32(curObj["StatusInfo"])));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSupportsDiskQuotasNull {
            get {
                if ((curObj["SupportsDiskQuotas"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The SupportsDiskQuotas property indicates if the volume supports disk quotas.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool SupportsDiskQuotas {
            get {
                if ((curObj["SupportsDiskQuotas"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["SupportsDiskQuotas"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSupportsFileBasedCompressionNull {
            get {
                if ((curObj["SupportsFileBasedCompression"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The SupportsFileBasedCompression property indicates if the volume supports file b" +
            "ased compression as is the case with the NTFS file system.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool SupportsFileBasedCompression {
            get {
                if ((curObj["SupportsFileBasedCompression"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["SupportsFileBasedCompression"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The scoping System\'s CreationClassName.")]
        public string SystemCreationClassName {
            get {
                return ((string)(curObj["SystemCreationClassName"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The scoping System\'s Name.")]
        public string SystemName {
            get {
                return ((string)(curObj["SystemName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSystemVolumeNull {
            get {
                if ((curObj["SystemVolume"] is null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The SystemVolume property indicates whether the volume contains the hardware spec" +
            "ific files required to start the operating system.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool SystemVolume {
            get {
                if ((curObj["SystemVolume"] is null)) {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["SystemVolume"]));
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam) {
            if (((path is not null) 
                        && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj) {
            if (((theObj is not null) 
                        && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
                if ((parentClasses is not null)) {
                    int count = 0;
                    for (count = 0; (count < parentClasses.Length); count = (count + 1)) {
                        if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private bool ShouldSerializeAccess() {
            if ((this.IsAccessNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeAutomount() {
            if ((this.IsAutomountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeAvailability() {
            if ((this.IsAvailabilityNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeBlockSize() {
            if ((this.IsBlockSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeBootVolume() {
            if ((this.IsBootVolumeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeCapacity() {
            if ((this.IsCapacityNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeCompressed() {
            if ((this.IsCompressedNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeConfigManagerErrorCode() {
            if ((this.IsConfigManagerErrorCodeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeConfigManagerUserConfig() {
            if ((this.IsConfigManagerUserConfigNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeDirtyBitSet() {
            if ((this.IsDirtyBitSetNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetDriveLetter() {
            curObj["DriveLetter"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeDriveType() {
            if ((this.IsDriveTypeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeErrorCleared() {
            if ((this.IsErrorClearedNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeFreeSpace() {
            if ((this.IsFreeSpaceNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeIndexingEnabled() {
            if ((this.IsIndexingEnabledNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetIndexingEnabled() {
            curObj["IndexingEnabled"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        // Konverterar ett givet datum/tid med DMTF-format till System.DateTime-objekt.
        static System.DateTime ToDateTime(string dmtfDate) {
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
            if ((dmtf is null)) {
                throw new System.ArgumentOutOfRangeException();
            }
            if ((dmtf.Length == 0)) {
                throw new System.ArgumentOutOfRangeException();
            }
            if ((dmtf.Length != 25)) {
                throw new System.ArgumentOutOfRangeException();
            }
            try {
                tempString = dmtf.Substring(0, 4);
                if (("****" != tempString)) {
                    year = int.Parse(tempString);
                }
                tempString = dmtf.Substring(4, 2);
                if (("**" != tempString)) {
                    month = int.Parse(tempString);
                }
                tempString = dmtf.Substring(6, 2);
                if (("**" != tempString)) {
                    day = int.Parse(tempString);
                }
                tempString = dmtf.Substring(8, 2);
                if (("**" != tempString)) {
                    hour = int.Parse(tempString);
                }
                tempString = dmtf.Substring(10, 2);
                if (("**" != tempString)) {
                    minute = int.Parse(tempString);
                }
                tempString = dmtf.Substring(12, 2);
                if (("**" != tempString)) {
                    second = int.Parse(tempString);
                }
                tempString = dmtf.Substring(15, 6);
                if (("******" != tempString)) {
                    ticks = (long.Parse(tempString) * ((long)((System.TimeSpan.TicksPerMillisecond / 1000))));
                }
                if (((((((((year < 0) 
                            || (month < 0)) 
                            || (day < 0)) 
                            || (hour < 0)) 
                            || (minute < 0)) 
                            || (minute < 0)) 
                            || (second < 0)) 
                            || (ticks < 0))) {
                    throw new System.ArgumentOutOfRangeException();
                }
            }
            catch (System.Exception e) {
                throw new System.ArgumentOutOfRangeException(null, e.Message);
            }
            datetime = new System.DateTime(year, month, day, hour, minute, second, 0);
            datetime = datetime.AddTicks(ticks);
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
            int UTCOffset = 0;
            int OffsetToBeAdjusted = 0;
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            tempString = dmtf.Substring(22, 3);
            if ((tempString != "******")) {
                tempString = dmtf.Substring(21, 4);
                try {
                    UTCOffset = int.Parse(tempString);
                }
                catch (System.Exception e) {
                    throw new System.ArgumentOutOfRangeException(null, e.Message);
                }
                OffsetToBeAdjusted = ((int)((OffsetMins - UTCOffset)));
                datetime = datetime.AddMinutes(((double)(OffsetToBeAdjusted)));
            }
            return datetime;
        }
        
        // Konverterar ett givet System.DateTime-objekt till DMTF-datum/tid.
        static string ToDmtfDateTime(System.DateTime date) {
            string utcString = string.Empty;
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(date);
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            if ((System.Math.Abs(OffsetMins) > 999)) {
                date = date.ToUniversalTime();
                utcString = "+000";
            }
            else {
                if ((tickOffset.Ticks >= 0)) {
                    utcString = string.Concat("+", ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute))).ToString().PadLeft(3, '0'));
                }
                else {
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
            if ((strMicrosec.Length > 6)) {
                strMicrosec = strMicrosec.Substring(0, 6);
            }
            dmtfDateTime = string.Concat(dmtfDateTime, strMicrosec.PadLeft(6, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, utcString);
            return dmtfDateTime;
        }
        
        private bool ShouldSerializeInstallDate() {
            if ((this.IsInstallDateNull == false)) {
                return true;
            }
            return false;
        }
        
        private void ResetLabel() {
            curObj["Label"] = null;
            if (((isEmbedded == false) 
                        && (AutoCommitProp == true))) {
                PrivateLateBoundObject.Put();
            }
        }
        
        private bool ShouldSerializeLastErrorCode() {
            if ((this.IsLastErrorCodeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeMaximumFileNameLength() {
            if ((this.IsMaximumFileNameLengthNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeNumberOfBlocks() {
            if ((this.IsNumberOfBlocksNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePageFilePresent() {
            if ((this.IsPageFilePresentNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePowerManagementSupported() {
            if ((this.IsPowerManagementSupportedNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotasEnabled() {
            if ((this.IsQuotasEnabledNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotasIncomplete() {
            if ((this.IsQuotasIncompleteNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotasRebuilding() {
            if ((this.IsQuotasRebuildingNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeSerialNumber() {
            if ((this.IsSerialNumberNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeStatusInfo() {
            if ((this.IsStatusInfoNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeSupportsDiskQuotas() {
            if ((this.IsSupportsDiskQuotasNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeSupportsFileBasedCompression() {
            if ((this.IsSupportsFileBasedCompressionNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeSystemVolume() {
            if ((this.IsSystemVolumeNull == false)) {
                return true;
            }
            return false;
        }
        
        [Browsable(true)]
        public void CommitObject() {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put();
            }
        }
        
        [Browsable(true)]
        public void CommitObject(System.Management.PutOptions putOptions) {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put(putOptions);
            }
        }
        
        private void Initialize() {
            AutoCommitProp = true;
            isEmbedded = false;
        }
        
        private static string ConstructPath(string keyDeviceID) {
            string strPath = "Root\\CIMV2:Win32_Volume";
            strPath = string.Concat(strPath, string.Concat(".DeviceID=", string.Concat("\"", string.Concat(keyDeviceID, "\""))));
            return strPath;
        }
        
        private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            Initialize();
            if ((path is not null)) {
                if ((CheckIfProperClass(mgmtScope, path, getOptions) != true)) {
                    throw new System.ArgumentException("Klassnamnet matchar inte.");
                }
            }
            PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }
        
        // Olika överlagringar av hjälp om GetInstances() i WMI-klassens uppräkningsinstanser.
        public static VolumeCollection GetInstances() {
            return GetInstances(null, null, null);
        }
        
        public static VolumeCollection GetInstances(string condition) {
            return GetInstances(null, condition, null);
        }
        
        public static VolumeCollection GetInstances(string[] selectedProperties) {
            return GetInstances(null, null, selectedProperties);
        }
        
        public static VolumeCollection GetInstances(string condition, string[] selectedProperties) {
            return GetInstances(null, condition, selectedProperties);
        }
        
        public static VolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions) {
            if ((mgmtScope is null)) {
                if ((statMgmtScope is null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "Root\\CIMV2";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
            pathObj.ClassName = "Win32_Volume";
            pathObj.NamespacePath = "Root\\CIMV2";
            System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
            if ((enumOptions is null)) {
                enumOptions = new System.Management.EnumerationOptions();
                enumOptions.EnsureLocatable = true;
            }
            return new VolumeCollection(clsObject.GetInstances(enumOptions));
        }
        
        public static VolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition) {
            return GetInstances(mgmtScope, condition, null);
        }
        
        public static VolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, string[] selectedProperties) {
            return GetInstances(mgmtScope, null, selectedProperties);
        }
        
        public static VolumeCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, string[] selectedProperties) {
            if ((mgmtScope is null)) {
                if ((statMgmtScope is null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "Root\\CIMV2";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_Volume", condition, selectedProperties));
            System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new VolumeCollection(ObjectSearcher.Get());
        }
        
        [Browsable(true)]
        public static Volume CreateInstance() {
            System.Management.ManagementScope mgmtScope = null;
            if ((statMgmtScope is null)) {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
            }
            else {
                mgmtScope = statMgmtScope;
            }
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
            return new Volume(tmpMgmtClass.CreateInstance());
        }
        
        [Browsable(true)]
        public void Delete() {
            PrivateLateBoundObject.Delete();
        }
        
        public uint AddMountPoint(string Directory) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("AddMountPoint");
                inParams["Directory"] = ((string)(Directory));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("AddMountPoint", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Chkdsk(bool FixErrors, bool ForceDismount, bool OkToRunAtBootUp, bool RecoverBadSectors, bool SkipFolderCycle, bool VigorousIndexCheck) {
            if ((isEmbedded == false)) {
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
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Defrag(bool Force, out System.Management.ManagementBaseObject DefragAnalysis) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("Defrag");
                inParams["Force"] = ((bool)(Force));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Defrag", inParams, null);
                DefragAnalysis = ((System.Management.ManagementBaseObject)(outParams.Properties["DefragAnalysis"].Value));
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                DefragAnalysis = null;
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint DefragAnalysis(out System.Management.ManagementBaseObject DefragAnalysis, out bool DefragRecommended) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DefragAnalysis", inParams, null);
                DefragAnalysis = ((System.Management.ManagementBaseObject)(outParams.Properties["DefragAnalysis"].Value));
                DefragRecommended = System.Convert.ToBoolean(outParams.Properties["DefragRecommended"].Value);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                DefragAnalysis = null;
                DefragRecommended = System.Convert.ToBoolean(0);
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Dismount(bool Force, bool Permanent) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("Dismount");
                inParams["Force"] = ((bool)(Force));
                inParams["Permanent"] = ((bool)(Permanent));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Dismount", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public static uint ExcludeFromAutoChk(string[] Volume) {
            bool IsMethodStatic = true;
            if ((IsMethodStatic == true)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
                System.Management.ManagementClass classObj = new System.Management.ManagementClass(statMgmtScope, mgmtPath, null);
                inParams = classObj.GetMethodParameters("ExcludeFromAutoChk");
                inParams["Volume"] = ((string[])(Volume));
                System.Management.ManagementBaseObject outParams = classObj.InvokeMethod("ExcludeFromAutoChk", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Format(uint ClusterSize, bool EnableCompression, string FileSystem, string Label, bool QuickFormat, uint Version) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("Format");
                inParams["ClusterSize"] = ((uint)(ClusterSize));
                inParams["EnableCompression"] = ((bool)(EnableCompression));
                inParams["FileSystem"] = ((string)(FileSystem));
                inParams["Label"] = ((string)(Label));
                inParams["QuickFormat"] = ((bool)(QuickFormat));
                inParams["Version"] = ((uint)(Version));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Format", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Mount() {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Mount", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Reset() {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Reset", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public static uint ScheduleAutoChk(string[] Volume) {
            bool IsMethodStatic = true;
            if ((IsMethodStatic == true)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
                System.Management.ManagementClass classObj = new System.Management.ManagementClass(statMgmtScope, mgmtPath, null);
                inParams = classObj.GetMethodParameters("ScheduleAutoChk");
                inParams["Volume"] = ((string[])(Volume));
                System.Management.ManagementBaseObject outParams = classObj.InvokeMethod("ScheduleAutoChk", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint SetPowerState(ushort PowerState, System.DateTime Time) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("SetPowerState");
                inParams["PowerState"] = ((ushort)(PowerState));
                inParams["Time"] = ToDmtfDateTime(((System.DateTime)(Time)));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetPowerState", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public enum AccessValues {
            
            Unknown0 = 0,
            
            Readable = 1,
            
            Writeable = 2,
            
            Read_Write_Supported = 3,
            
            Write_Once = 4,
            
            NULL_ENUM_VALUE = 5,
        }
        
        public enum AvailabilityValues {
            
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
        
        public enum ConfigManagerErrorCodeValues {
            
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
        
        public enum DriveTypeValues {
            
            Unknown0 = 0,
            
            No_Root_Directory = 1,
            
            Removable_Disk = 2,
            
            Local_Disk = 3,
            
            Network_Drive = 4,
            
            Compact_Disc = 5,
            
            RAM_Disk = 6,
            
            NULL_ENUM_VALUE = 7,
        }
        
        public enum PowerManagementCapabilitiesValues {
            
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
        
        public enum StatusInfoValues {
            
            Other0 = 1,
            
            Unknown0 = 2,
            
            Enabled = 3,
            
            Disabled = 4,
            
            Not_Applicable = 5,
            
            NULL_ENUM_VALUE = 0,
        }
        
        // Implementering av uppräknare för klassens uppräkningsinstanser.
        public class VolumeCollection : object, ICollection {
            
            private ManagementObjectCollection privColObj;
            
            public VolumeCollection(ManagementObjectCollection objCollection) {
                privColObj = objCollection;
            }
            
            public virtual int Count {
                get {
                    return privColObj.Count;
                }
            }
            
            public virtual bool IsSynchronized {
                get {
                    return privColObj.IsSynchronized;
                }
            }
            
            public virtual object SyncRoot {
                get {
                    return this;
                }
            }
            
            public virtual void CopyTo(System.Array array, int index) {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1)) {
                    array.SetValue(new Volume(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }
            
            public virtual System.Collections.IEnumerator GetEnumerator() {
                return new VolumeEnumerator(privColObj.GetEnumerator());
            }
            
            public class VolumeEnumerator : object, System.Collections.IEnumerator {
                
                private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;
                
                public VolumeEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum) {
                    privObjEnum = objEnum;
                }
                
                public virtual object Current {
                    get {
                        return new Volume(((System.Management.ManagementObject)(privObjEnum.Current)));
                    }
                }
                
                public virtual bool MoveNext() {
                    return privObjEnum.MoveNext();
                }
                
                public virtual void Reset() {
                    privObjEnum.Reset();
                }
            }
        }
        
        // TypeConverter som hanterar Null-värden för ValueType-egenskaper
        public class WMIValueTypeConverter : TypeConverter {
            
            private TypeConverter baseConverter;
            
            private System.Type baseType;
            
            public WMIValueTypeConverter(System.Type inBaseType) {
                baseConverter = TypeDescriptor.GetConverter(inBaseType);
                baseType = inBaseType;
            }
            
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType) {
                return baseConverter.CanConvertFrom(context, srcType);
            }
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
                return baseConverter.CanConvertTo(context, destinationType);
            }
            
            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
                return baseConverter.ConvertFrom(context, culture, value);
            }
            
            public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary) {
                return baseConverter.CreateInstance(context, dictionary);
            }
            
            public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetCreateInstanceSupported(context);
            }
            
            public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar) {
                return baseConverter.GetProperties(context, value, attributeVar);
            }
            
            public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetPropertiesSupported(context);
            }
            
            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValues(context);
            }
            
            public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesExclusive(context);
            }
            
            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesSupported(context);
            }
            
            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
                if ((baseType.BaseType == typeof(System.Enum))) {
                    if ((value.GetType() == destinationType)) {
                        return value;
                    }
                    if ((((value is null) 
                                && (context is not null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return  "NULL_ENUM_VALUE" ;
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((baseType == typeof(bool)) 
                            && (baseType.BaseType == typeof(System.ValueType)))) {
                    if ((((value is null) 
                                && (context is not null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return "";
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((context is not null) 
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
        }
        
        // En inbäddad klass som representerar WMI-systemegenskaper.
        [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public class ManagementSystemProperties {
            
            private System.Management.ManagementBaseObject PrivateLateBoundObject;
            
            public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject) {
                PrivateLateBoundObject = ManagedObject;
            }
            
            [Browsable(true)]
            public int GENUS {
                get {
                    return ((int)(PrivateLateBoundObject["__GENUS"]));
                }
            }
            
            [Browsable(true)]
            public string CLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__CLASS"]));
                }
            }
            
            [Browsable(true)]
            public string SUPERCLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
                }
            }
            
            [Browsable(true)]
            public string DYNASTY {
                get {
                    return ((string)(PrivateLateBoundObject["__DYNASTY"]));
                }
            }
            
            [Browsable(true)]
            public string RELPATH {
                get {
                    return ((string)(PrivateLateBoundObject["__RELPATH"]));
                }
            }
            
            [Browsable(true)]
            public int PROPERTY_COUNT {
                get {
                    return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
                }
            }
            
            [Browsable(true)]
            public string[] DERIVATION {
                get {
                    return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
                }
            }
            
            [Browsable(true)]
            public string SERVER {
                get {
                    return ((string)(PrivateLateBoundObject["__SERVER"]));
                }
            }
            
            [Browsable(true)]
            public string NAMESPACE {
                get {
                    return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
                }
            }
            
            [Browsable(true)]
            public string PATH {
                get {
                    return ((string)(PrivateLateBoundObject["__PATH"]));
                }
            }
        }
    }
}
