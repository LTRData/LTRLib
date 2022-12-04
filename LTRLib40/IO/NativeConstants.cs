// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using LTRLib.Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Xml.Serialization;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace LTRLib.IO;

public static class NativeConstants
{
    public const uint GENERIC_READ = 0x80000000U;
    public const uint GENERIC_WRITE = 0x40000000U;
    public const uint GENERIC_EXECUTE = 0x20000000U;
    public const uint GENERIC_ALL = 0x10000000U;
    public const uint FILE_READ_ATTRIBUTES = 0x80U;
    public const uint SYNCHRONIZE = 0x100000U;
    public const uint SC_MANAGER_CREATE_SERVICE = 0x2U;
    public const uint SC_MANAGER_ALL_ACCESS = 0xF003FU;

    public const uint SERVICE_KERNEL_DRIVER = 0x1U;
    public const uint SERVICE_FILE_SYSTEM_DRIVER = 0x2U;
    public const uint SERVICE_WIN32_OWN_PROCESS = 0x10U; // Service that runs in its own process. 
    public const uint SERVICE_WIN32_SHARE_PROCESS = 0x20U;
    public const uint SERVICE_DEMAND_START = 0x3U;
    public const uint SERVICE_ERROR_IGNORE = 0x0U;
    public const uint SERVICE_CONTROL_STOP = 0x1U;

    public const uint ERROR_SERVICE_DOES_NOT_EXIST = 1060U;
    public const uint ERROR_SERVICE_ALREADY_RUNNING = 1056U;

    public const uint FILE_SHARE_READ = 0x1U;
    public const uint FILE_SHARE_WRITE = 0x2U;
    public const uint FILE_SHARE_DELETE = 0x4U;

    public const uint FILE_ATTRIBUTE_NORMAL = 0x80U;

    public const FileOptions FILE_FLAG_BACKUP_SEMANTICS = (FileOptions)0x2000000;
    public const FileOptions FILE_FLAG_OPEN_REPARSE_POINT = (FileOptions)0x200000;
    public const FileOptions FILE_FLAG_POSIX_SEMANTICS = (FileOptions)0x1000000;

    public const uint FILE_BEGIN = 0U;
    public const uint FILE_CURRENT = 1U;
    public const uint FILE_END = 2U;

    public const uint OPEN_ALWAYS = 4U;
    public const uint OPEN_EXISTING = 3U;
    public const uint CREATE_ALWAYS = 2U;
    public const uint CREATE_NEW = 1U;
    public const uint TRUNCATE_EXISTING = 5U;

    public const uint ERROR_SUCCESS = 0U;

    public const uint ERROR_FILE_NOT_FOUND = 2U;
    public const uint ERROR_PATH_NOT_FOUND = 3U;
    public const uint ERROR_ACCESS_DENIED = 5U;
    public const uint ERROR_INVALID_FUNCTION = 1U;
    public const uint ERROR_IO_DEVICE = 0x45DU;
    public const uint ERROR_NO_MORE_FILES = 18U;
    public const uint ERROR_SHARING_VIOLATION = 32U;
    public const uint ERROR_HANDLE_EOF = 38U;
    public const uint ERROR_INVALID_PARAMETER = 87U;
    public const uint ERROR_MORE_DATA = 0x234U;

    public const uint IOCTL_SCSI_MINIPORT = 0x4D008U;
    public const uint IOCTL_SCSI_GET_ADDRESS = 0x41018U;
    public const uint IOCTL_STORAGE_GET_DEVICE_NUMBER = 0x2D1080U;
    public const uint IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x70000U;
    public const uint IOCTL_DISK_GET_LENGTH_INFO = 0x7405CU;
    public const uint IOCTL_DISK_GET_PARTITION_INFO = 0x74004U;
    public const uint IOCTL_DISK_GET_PARTITION_INFO_EX = 0x70048U;
    public const uint IOCTL_DISK_GROW_PARTITION = 0x7C0D0U;
    public const uint IOCTL_DISK_UPDATE_PROPERTIES = 0x70140U;
    public const uint IOCTL_DISK_IS_WRITABLE = 0x70024U;
    public const uint IOCTL_SCSI_RESCAN_BUS = 0x4101CU;

    public const uint IOCTL_DISK_GET_DISK_ATTRIBUTES = 0x700F0U;
    public const uint IOCTL_DISK_SET_DISK_ATTRIBUTES = 0x7C0F4U;
    public const uint IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x560000U;
    public const uint IOCTL_VOLUME_OFFLINE = 0x56C00CU;
    public const uint IOCTL_VOLUME_ONLINE = 0x56C008U;

    public const uint FSCTL_GET_COMPRESSION = 0x9003CU;
    public const uint FSCTL_SET_COMPRESSION = 0x9C040U;

    public const uint FSCTL_SET_SPARSE = 0x900C4U;
    public const uint FSCTL_QUERY_ALLOCATED_RANGES = 0x940CFU;

    public const uint FSCTL_ALLOW_EXTENDED_DASD_IO = 0x90083U;
    public const uint FSCTL_GET_VOLUME_BITMAP = 0x9006FU;

    public const ushort COMPRESSION_FORMAT_NONE = 0;
    public const ushort COMPRESSION_FORMAT_DEFAULT = 1;
    public const ushort COMPRESSION_FORMAT_LZNT1 = 2;

    public const int STATUS_BAD_COMPRESSION_BUFFER = int.MinValue + 0x40000242;

    [Flags]
    public enum PageProtection : uint
    {
        SecReserve = 67108864U,
        SecNoCache = 268435456U,
        SecLargePages = 2147483648U,
        SecImage = 16777216U,
        SecCommit = 134217728U,
        PageExecuteReadWrite = 64U,
        PageExecuteRead = 32U,
        PageWriteCopy = 8U,
        PageReadWrite = 4U,
        PageReadOnly = 2U
    }

    public enum HandleInheritability : uint
    {
        Inheritable = 1U,
        None = 0U
    }

    public enum MemoryMappedFileRights : uint
    {
        ReadWrite = 2U,
        Read = 4U,
        FullControl = 983071U,
        WriteCopy = 1U,
        Execute = 32U
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct UNICODE_STRING
    {
        public ushort Length { get; }

        public ushort MaximumLength { get; }

        private readonly IntPtr Buffer;

        public UNICODE_STRING(IntPtr buffer, ushort length, ushort maximumLength)
        {
            Buffer = buffer;
            Length = length;
            MaximumLength = maximumLength;
        }

        public UNICODE_STRING(IntPtr buffer, ushort length)
        {
            Buffer = buffer;
            Length = length;
            MaximumLength = length;
        }

        public override string ToString() => Marshal.PtrToStringUni(Buffer, Length / 2);
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress { get; }
        public IntPtr AllocationBase { get; }
        public uint AllocationProtect { get; }
        public UIntPtr RegionSize { get; }
        public uint State { get; }
        public uint Protect { get; }
        public uint Type { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    [SecuritySafeCritical]
    public readonly struct LARGE_INTEGER : IEquatable<LARGE_INTEGER>, IEquatable<long>
    {
        public long QuadPart { get; }

        public uint LowPart => (uint)(QuadPart & uint.MaxValue);

        public int HighPart => (int)(QuadPart >> 32);

        public LARGE_INTEGER(long value)
        {
            QuadPart = value;
        }

        public LARGE_INTEGER(uint LowPart, int HighPart)
        {
            QuadPart = LowPart | ((long)HighPart << 32);
        }

        [SecuritySafeCritical]
        public override string ToString() => QuadPart.ToString();

        [SecuritySafeCritical]
        public override int GetHashCode() => QuadPart.GetHashCode();

        [SecuritySafeCritical]
        public static implicit operator LARGE_INTEGER(long value)
        {
            return new LARGE_INTEGER(value);
        }

        [SecuritySafeCritical]
        public static implicit operator long(LARGE_INTEGER value)
        {
            return value.QuadPart;
        }

        [SecuritySafeCritical]
        public override bool Equals(object? obj)
        {
            if (obj is LARGE_INTEGER largeInteger)
            {
                return Equals(largeInteger);
            }
            else if (obj is long @long)
            {
                return Equals(@long);
            }
            return false;
        }

        public static bool operator ==(LARGE_INTEGER @this, LARGE_INTEGER other) => @this.Equals(other);

        public static bool operator !=(LARGE_INTEGER @this, LARGE_INTEGER other) => !@this.Equals(other);

        public static bool operator ==(LARGE_INTEGER @this, long other) => @this.Equals(other);

        public static bool operator !=(LARGE_INTEGER @this, long other) => !@this.Equals(other);

        public static bool operator ==(long @this, LARGE_INTEGER other) => @this.Equals(other);

        public static bool operator !=(long @this, LARGE_INTEGER other) => !@this.Equals(other);

        [SecuritySafeCritical]
        public bool Equals(long other) => QuadPart == other;

        [SecuritySafeCritical]
        public bool Equals(LARGE_INTEGER other) => QuadPart == other.QuadPart;

    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ULARGE_INTEGER : IEquatable<ULARGE_INTEGER>, IEquatable<ulong>
    {
        public ulong QuadPart { get; }

        public uint LowPart => (uint)(QuadPart & uint.MaxValue);

        public uint HighPart => (uint)(QuadPart >> 32);

        public ULARGE_INTEGER(ulong value)
        {
            QuadPart = value;
        }

        public ULARGE_INTEGER(uint LowPart, uint HighPart)
        {
            QuadPart = LowPart | ((ulong)HighPart << 32);
        }

        [SecuritySafeCritical]
        public override string ToString() => QuadPart.ToString();

        [SecuritySafeCritical]
        public override int GetHashCode() => QuadPart.GetHashCode();

        [SecuritySafeCritical]
        public override bool Equals(object? obj)
        {
            if (obj is ULARGE_INTEGER largeInteger)
            {
                return Equals(largeInteger);
            }
            else if (obj is ulong @ulong)
            {
                return Equals(@ulong);
            }
            return false;
        }

        public static bool operator ==(ULARGE_INTEGER @this, ULARGE_INTEGER other) => @this.Equals(other);

        public static bool operator !=(ULARGE_INTEGER @this, ULARGE_INTEGER other) => !@this.Equals(other);

        public static bool operator ==(ULARGE_INTEGER @this, ulong other) => @this.Equals(other);

        public static bool operator !=(ULARGE_INTEGER @this, ulong other) => !@this.Equals(other);

        public static bool operator ==(ulong @this, ULARGE_INTEGER other) => @this.Equals(other);

        public static bool operator !=(ulong @this, ULARGE_INTEGER other) => !@this.Equals(other);

        [SecuritySafeCritical]
        public static implicit operator ULARGE_INTEGER(ulong value)
        {
            return new ULARGE_INTEGER(value);
        }

        [SecuritySafeCritical]
        public static implicit operator ulong(ULARGE_INTEGER value)
        {
            return value.QuadPart;
        }

        [SecuritySafeCritical]
        public bool Equals(ulong other) => QuadPart == other;

        [SecuritySafeCritical]
        public bool Equals(ULARGE_INTEGER other) => QuadPart == other.QuadPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DISK_GEOMETRY
    {
        public enum MEDIA_TYPE : int
        {
            Unknown = 0x0,
            F5_1Pt2_512 = 0x1,
            F3_1Pt44_512 = 0x2,
            F3_2Pt88_512 = 0x3,
            F3_20Pt8_512 = 0x4,
            F3_720_512 = 0x5,
            F5_360_512 = 0x6,
            F5_320_512 = 0x7,
            F5_320_1024 = 0x8,
            F5_180_512 = 0x9,
            F5_160_512 = 0xA,
            RemovableMedia = 0xB,
            FixedMedia = 0xC,
            F3_120M_512 = 0xD,
            F3_640_512 = 0xE,
            F5_640_512 = 0xF,
            F5_720_512 = 0x10,
            F3_1Pt2_512 = 0x11,
            F3_1Pt23_1024 = 0x12,
            F5_1Pt23_1024 = 0x13,
            F3_128Mb_512 = 0x14,
            F3_230Mb_512 = 0x15,
            F8_256_128 = 0x16,
            F3_200Mb_512 = 0x17,
            F3_240M_512 = 0x18,
            F3_32M_512 = 0x19
        }

        public long Cylinders { get; }
        public MEDIA_TYPE MediaType { get; }
        public int TracksPerCylinder { get; }
        public int SectorsPerTrack { get; }
        public int BytesPerSector { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DISK_GROW_PARTITION
    {
        public int PartitionNumber { get; }
        public long BytesToGrow { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct COMMTIMEOUTS
    {
        public uint ReadIntervalTimeout { get; }
        public uint ReadTotalTimeoutMultiplier { get; }
        public uint ReadTotalTimeoutConstant { get; }
        public uint WriteTotalTimeoutMultiplier { get; }
        public uint WriteTotalTimeoutConstant { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct DiskExtents
    {
        private readonly int NumberOfExtents;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private readonly DiskExtent[] Extents;

        public DiskExtent[] CreateExtentsArray()
        {
            var a = Extents;
            Array.Resize(ref a, NumberOfExtents);
            return a;
        }

        public DiskExtents(int maxExtents)
        {
            Array.Resize(ref Extents, maxExtents);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DiskExtent
    {
        public uint DiskNumber { get; }

        public long StartingOffset { get; }

        public long ExtentLength { get; }

        public DiskExtent(uint DiskNumber, long StartingOffset, long ExtentLength)
        {
            this.DiskNumber = DiskNumber;
            this.StartingOffset = StartingOffset;
            this.ExtentLength = ExtentLength;
        }

        public DiskExtent(long StartingOffset, long ExtentLength)
        {
            DiskNumber = uint.MaxValue;
            this.StartingOffset = StartingOffset;
            this.ExtentLength = ExtentLength;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SECURITY_ATTRIBUTES
    {
        public int Length { get; }

        public IntPtr SecurityDescriptor { get; }

        [field: MarshalAs(UnmanagedType.Bool)]
        public readonly bool InheritHandle { get; }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
        public SECURITY_ATTRIBUTES(IntPtr SecurityDescriptor, bool InheritHandle)
            : this()
        {
            this.SecurityDescriptor = SecurityDescriptor;
            this.InheritHandle = InheritHandle;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.AllFlags)]
        public unsafe SECURITY_ATTRIBUTES()
        {
            Length = sizeof(SECURITY_ATTRIBUTES);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SERVICE_STATUS
    {
        public int dwServiceType { get; }
        public int dwCurrentState { get; }
        public int dwControlsAccepted { get; }
        public int dwWin32ExitCode { get; }
        public int dwServiceSpecificExitCode { get; }
        public int dwCheckPoint { get; }
        public int dwWaitHint { get; }
    }

    [Flags]
    public enum DEFINE_DOS_DEVICE_FLAGS : uint
    {
        DDD_EXACT_MATCH_ON_REMOVE = 0x4U,
        DDD_NO_BROADCAST_SYSTEM = 0x8U,
        DDD_RAW_TARGET_PATH = 0x1U,
        DDD_REMOVE_DEFINITION = 0x2U
    }

    public enum Win32FileType : int
    {
        Unknown = 0x0,
        Disk = 0x1,
        Char = 0x2,
        Pipe = 0x3,
        Remote = 0x8000
    }

    public enum StdHandle : int
    {
        Input = -10,
        Output = -11,
        Error = -12
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct COORD
    {
        public short X { get; }
        public short Y { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SMALL_RECT
    {
        public short Left { get; }
        public short Top { get; }
        public short Right { get; }
        public short Bottom { get; }
        public short Width => (short)((short)(Right - Left) + 1);
        public short Height => (short)((short)(Bottom - Top) + 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CONSOLE_SCREEN_BUFFER_INFO
    {
        public COORD Size { get; }
        public COORD CursorPosition { get; }
        public short Attributes { get; }
        public SMALL_RECT Window { get; }
        public COORD MaximumWindowSize { get; }
    }

    internal struct CSDVersion
    {
        private unsafe fixed char version[128];

        public override unsafe string ToString() => BufferExtensions.CreateString(version[0]);
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct OSVERSIONINFO
    {
        public int OSVersionInfoSize { get; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildNumber { get; }
        public PlatformID PlatformId { get; }

        private readonly CSDVersion csdVersion;
        public string CSDVersion => csdVersion.ToString();

        public unsafe OSVERSIONINFO()
        {
            OSVersionInfoSize = sizeof(OSVERSIONINFO);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct OSVERSIONINFOEX
    {
        public int OSVersionInfoSize { get; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildNumber { get; }
        public PlatformID PlatformId { get; }

        private readonly CSDVersion csdVersion;
        public string CSDVersion => csdVersion.ToString();

        public ushort ServicePackMajor { get; }

        public ushort ServicePackMinor { get; }

        public short SuiteMask { get; }

        public byte ProductType { get; }

        public byte Reserved { get; }

        public unsafe OSVERSIONINFOEX()
        {
            OSVersionInfoSize = sizeof(OSVERSIONINFOEX);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public readonly struct PARTITION_INFORMATION
    {
        public enum PARTITION_TYPE : byte
        {
            PARTITION_ENTRY_UNUSED = 0x0,      // Entry unused
            PARTITION_FAT_12 = 0x1,      // 12-bit FAT entries
            PARTITION_XENIX_1 = 0x2,      // Xenix
            PARTITION_XENIX_2 = 0x3,      // Xenix
            PARTITION_FAT_16 = 0x4,      // 16-bit FAT entries
            PARTITION_EXTENDED = 0x5,      // Extended partition entry
            PARTITION_HUGE = 0x6,      // Huge partition MS-DOS V4
            PARTITION_IFS = 0x7,      // IFS Partition
            PARTITION_OS2BOOTMGR = 0xA,      // OS/2 Boot Manager/OPUS/Coherent swap
            PARTITION_FAT32 = 0xB,      // FAT32
            PARTITION_FAT32_XINT13 = 0xC,      // FAT32 using extended int13 services
            PARTITION_XINT13 = 0xE,      // Win95 partition using extended int13 services
            PARTITION_XINT13_EXTENDED = 0xF,      // Same as type 5 but uses extended int13 services
            PARTITION_PREP = 0x41,      // PowerPC Reference Platform (PReP) Boot Partition
            PARTITION_LDM = 0x42,      // Logical Disk Manager partition
            PARTITION_UNIX = 0x63,      // Unix
            PARTITION_NTFT = 0x80      // NTFT partition      
        }

        public long StartingOffset { get; }
        public long PartitionLength { get; }
        public uint HiddenSectors { get; }
        public uint PartitionNumber { get; }
        public PARTITION_TYPE PartitionType { get; }
        public byte BootIndicator { get; }
        public byte RecognizedPartition { get; }
        public byte RewritePartition { get; }

        /// <summary>
        /// Indicates whether this partition entry represents a Windows NT fault tolerant partition,
        /// such as mirror or stripe set.
        /// </summary>
        /// <value>
        /// Indicates whether this partition entry represents a Windows NT fault tolerant partition,
        /// such as mirror or stripe set.
        /// </value>
        /// <returns>True if this partition entry represents a Windows NT fault tolerant partition,
        /// such as mirror or stripe set. False otherwise.</returns>
        public bool IsFTPartition
            => (PartitionType & PARTITION_TYPE.PARTITION_NTFT) == PARTITION_TYPE.PARTITION_NTFT;

        /// <summary>
        /// If this partition entry represents a Windows NT fault tolerant partition, such as mirror or stripe,
        /// set, then this property returns partition subtype, such as PARTITION_IFS for NTFS or HPFS
        /// partitions.
        /// </summary>
        /// <value>
        /// If this partition entry represents a Windows NT fault tolerant partition, such as mirror or stripe,
        /// set, then this property returns partition subtype, such as PARTITION_IFS for NTFS or HPFS
        /// partitions.
        /// </value>
        /// <returns>If this partition entry represents a Windows NT fault tolerant partition, such as mirror or
        /// stripe, set, then this property returns partition subtype, such as PARTITION_IFS for NTFS or HPFS
        /// partitions.</returns>
        public PARTITION_TYPE FTPartitionSubType
            => PartitionType & ~PARTITION_TYPE.PARTITION_NTFT;

        /// <summary>
        /// Indicates whether this partition entry represents a container partition, also known as extended
        /// partition, where an extended partition table can be found in first sector.
        /// </summary>
        /// <value>
        /// Indicates whether this partition entry represents a container partition.
        /// </value>
        /// <returns>True if this partition entry represents a container partition. False otherwise.</returns>
        public bool IsContainerPartition
            => PartitionType == PARTITION_TYPE.PARTITION_EXTENDED
            || PartitionType == PARTITION_TYPE.PARTITION_XINT13_EXTENDED;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct PARTITION_INFORMATION_EX
    {
        public enum PARTITION_STYLE : byte
        {
            PARTITION_STYLE_MBR,
            PARTITION_STYLE_GPT,
            PARTITION_STYLE_RAW
        }

        [field: MarshalAs(UnmanagedType.U1)]
        public PARTITION_STYLE PartitionStyle { get; }
        public long StartingOffset { get; }
        public long PartitionLength { get; }
        public uint PartitionNumber { get; }
        [field: MarshalAs(UnmanagedType.I1)]
        public bool RewritePartition { get; }

        private unsafe fixed byte fields[108];
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct STORAGE_DEVICE_NUMBER
    {
        public uint DeviceType { get; }

        public uint DeviceNumber { get; }

        public int PartitionNumber { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SCSI_ADDRESS : IEquatable<SCSI_ADDRESS>
    {
        public int Length { get; }
        public byte PortNumber { get; }
        public byte PathId { get; }
        public byte TargetId { get; }
        public byte Lun { get; }

        public SCSI_ADDRESS(byte PortNumber, uint DWordDeviceNumber)
            : this(DWordDeviceNumber)
        {
            this.PortNumber = PortNumber;
        }

        public unsafe SCSI_ADDRESS(uint DWordDeviceNumber)
        {
            Length = sizeof(SCSI_ADDRESS);
            PathId = (byte)(DWordDeviceNumber & 0xFFL);
            TargetId = (byte)(DWordDeviceNumber >> 8 & 0xFFL);
            Lun = (byte)(DWordDeviceNumber >> 16 & 0xFFL);
        }

        public uint DWordDeviceNumber => PathId | (uint)TargetId << 8 | (uint)Lun << 16;

        public override string ToString() => $"Port = {PortNumber}, Path = {PathId}, Target = {TargetId}, Lun = {Lun}";

        public bool Equals(SCSI_ADDRESS other) => PortNumber.Equals(other.PortNumber) && PathId.Equals(other.PathId) && TargetId.Equals(other.TargetId) && Lun.Equals(other.Lun);

        public override bool Equals(object? obj) => obj is SCSI_ADDRESS scsiAddress && Equals(scsiAddress);

        public override int GetHashCode() => PathId | TargetId << 8 | Lun << 16;

        public static bool operator ==(SCSI_ADDRESS first, SCSI_ADDRESS second) => first.Equals(second);

        public static bool operator !=(SCSI_ADDRESS first, SCSI_ADDRESS second) => !first.Equals(second);

    }
}
