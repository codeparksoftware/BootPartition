using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BootPartition
{
    class Native
    {
        #region Constants
        /// <summary>
        /// Read, write, and execute access.
        /// </summary>
        public const uint GENERIC_ALL = 0x01U << 28;

        /// <summary>
        /// Execute access.
        /// </summary>
        public const uint GENERIC_EXECUTE = 0x01U << 29;

        /// <summary>
        /// Write access.
        /// </summary>
        public const uint GENERIC_WRITE = 0x01U << 30;

        /// <summary>
        /// Read access.
        /// </summary>
        public const uint GENERIC_READ = 0x01U << 31;

        /// If this flag is not specified, but the file or device has been opened for read access, the function fails.
        /// </summary>
        public const uint FILE_SHARE_READ = 0x00000001U;

        /// If this flag is not specified, but the file or device has been opened for write access or has a file 
        /// mapping with write access, the function fails.
        /// </summary>
        public const uint FILE_SHARE_WRITE = 0x00000002U;

        /// <summary>
        /// Opens a file or device, only if it exists.
        /// 
        /// If the specified file or device does not exist, the function fails and the last-error code is set to 
        /// ERROR_FILE_NOT_FOUND (2).
        /// </summary>
        public const uint OPEN_EXISTING = 3U;


        /// <summary>
        /// The file is read only. Applications can read the file, but cannot write to or delete it.
        /// </summary>
        public const uint FILE_ATTRIBUTE_READONLY = 0x00000001U;

        /// <summary>
        /// The data area passed to a system call is too small.
        /// </summary>
        public const int ERROR_INSUFFICIENT_BUFFER = 0x7A;


        /// <summary>
        /// Retrieves information for each entry in the partition tables for a disk.
        /// </summary>
        public const uint IOCTL_DISK_GET_DRIVE_LAYOUT_EX = 0x00070050U;
        #endregion

        #region Structs

        /// <summary>
        /// Provides information about a drive's master boot record (MBR) partitions.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_MBR
        {
            /// <summary>
            /// The signature of the drive.
            /// </summary>
            [FieldOffset(0)]
            public uint Signature;
        }

        /// <summary>
        /// Contains information about a drive's GUID partition table (GPT) partitions.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_GPT
        {
            /// <summary>
            /// The GUID of the disk.
            /// </summary>
            [FieldOffset(0)]
            public Guid DiskId;

            /// <summary>
            /// The starting byte offset of the first usable block.
            /// </summary>
            [FieldOffset(16)]
            public Int64 StartingUsableOffset;

            /// <summary>
            /// The size of the usable blocks on the disk, in bytes.
            /// </summary>
            [FieldOffset(24)]
            public Int64 UsableLength;

            /// <summary>
            /// The maximum number of partitions that can be defined in the usable block.
            /// </summary>
            [FieldOffset(32)]
            public uint MaxPartitionCount;
        }
        /// <summary>
        /// Contains extended information about a drive's partitions.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_EX
        {
            /// <summary>
            /// The style of the partitions on the drive enumerated by the PARTITION_STYLE enumeration.
            /// </summary>
            [FieldOffset(0)]
            public PARTITION_STYLE PartitionStyle;

            /// <summary>
            /// The number of partitions on a drive.
            /// 
            /// On disks with the MBR layout, this value is always a multiple of 4. Any partitions that are unused have
            /// a partition type of PARTITION_ENTRY_UNUSED.
            /// </summary>
            [FieldOffset(4)]
            public uint PartitionCount;

            /// <summary>
            /// A DRIVE_LAYOUT_INFORMATION_MBR structure containing information about the master boot record type 
            /// partitioning on the drive.
            /// </summary>
            [FieldOffset(8)]
            public DRIVE_LAYOUT_INFORMATION_MBR Mbr;

            /// <summary>
            /// A DRIVE_LAYOUT_INFORMATION_GPT structure containing information about the GUID disk partition type 
            /// partitioning on the drive.
            /// </summary>
            [FieldOffset(8)]
            public DRIVE_LAYOUT_INFORMATION_GPT Gpt;

            /// <summary>
            /// A variable-sized array of PARTITION_INFORMATION_EX structures, one structure for each partition on the 
            /// drive.
            /// </summary>
            [FieldOffset(48)]
            public PARTITION_INFORMATION_EX PartitionEntry;
        }

        /// <summary>
        /// Represents the format of a partition.
        /// </summary>
        public enum PARTITION_STYLE : uint
        {
            /// <summary>
            /// Master boot record (MBR) format.
            /// </summary>
            PARTITION_STYLE_MBR = 0,

            /// <summary>
            /// GUID Partition Table (GPT) format.
            /// </summary>
            PARTITION_STYLE_GPT = 1,

            /// <summary>
            /// Partition not formatted in either of the recognized formats—MBR or GPT.
            /// </summary>
            PARTITION_STYLE_RAW = 2
        }
        /// <summary>
        /// Contains partition information specific to master boot record (MBR) disks.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct PARTITION_INFORMATION_MBR
        {
            #region Constants
            /// <summary>
            /// An unused entry partition.
            /// </summary>
            public const byte PARTITION_ENTRY_UNUSED = 0x00;

            /// <summary>
            /// A FAT12 file system partition.
            /// </summary>
            public const byte PARTITION_FAT_12 = 0x01;

            /// <summary>
            /// A FAT16 file system partition.
            /// </summary>
            public const byte PARTITION_FAT_16 = 0x04;

            /// <summary>
            /// An extended partition.
            /// </summary>
            public const byte PARTITION_EXTENDED = 0x05;

            /// <summary>
            /// An IFS partition.
            /// </summary>
            public const byte PARTITION_IFS = 0x07;

            /// <summary>
            /// A FAT32 file system partition.
            /// </summary>
            public const byte PARTITION_FAT32 = 0x0B;

            /// <summary>
            /// A logical disk manager (LDM) partition.
            /// </summary>
            public const byte PARTITION_LDM = 0x42;

            /// <summary>
            /// An NTFT partition.
            /// </summary>
            public const byte PARTITION_NTFT = 0x80;

            /// <summary>
            /// A valid NTFT partition.
            /// 
            /// The high bit of a partition type code indicates that a partition is part of an NTFT mirror or striped array.
            /// </summary>
            public const byte PARTITION_VALID_NTFT = 0xC0;
            #endregion

            /// <summary>
            /// The type of partition. For a list of values, see Disk Partition Types.
            /// </summary>
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.U1)]
            public byte PartitionType;

            /// <summary>
            /// If this member is TRUE, the partition is bootable.
            /// </summary>
            [FieldOffset(1)]
            [MarshalAs(UnmanagedType.I1)]
            public bool BootIndicator;

            /// <summary>
            /// If this member is TRUE, the partition is of a recognized type.
            /// </summary>
            [FieldOffset(2)]
            [MarshalAs(UnmanagedType.I1)]
            public bool RecognizedPartition;

            /// <summary>
            /// The number of hidden sectors in the partition.
            /// </summary>
            [FieldOffset(4)]
            public uint HiddenSectors;
        }

        /// <summary>
        /// Contains GUID partition table (GPT) partition information.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct PARTITION_INFORMATION_GPT
        {
            /// <summary>
            /// A GUID that identifies the partition type.
            /// 
            /// Each partition type that the EFI specification supports is identified by its own GUID, which is 
            /// published by the developer of the partition.
            /// </summary>
            [FieldOffset(0)]
            public Guid PartitionType;

            /// <summary>
            /// The GUID of the partition.
            /// </summary>
            [FieldOffset(16)]
            public Guid PartitionId;

            /// <summary>
            /// The Extensible Firmware Interface (EFI) attributes of the partition.
            /// 
            /// </summary>
            [FieldOffset(32)]
            public UInt64 Attributes;

            /// <summary>
            /// A wide-character string that describes the partition.
            /// </summary>
            [FieldOffset(40)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
            public string Name;
        }
        /// <summary>
        /// Contains information about a disk partition.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct PARTITION_INFORMATION_EX
        {
            /// <summary>
            /// The format of the partition. For a list of values, see PARTITION_STYLE.
            /// </summary>
            [FieldOffset(0)]
            public PARTITION_STYLE PartitionStyle;

            /// <summary>
            /// The starting offset of the partition.
            /// </summary>
            [FieldOffset(8)]
            public Int64 StartingOffset;

            /// <summary>
            /// The length of the partition, in bytes.
            /// </summary>
            [FieldOffset(16)]
            public Int64 PartitionLength;

            /// <summary>
            /// The number of the partition (1-based).
            /// </summary>
            [FieldOffset(24)]
            public uint PartitionNumber;

            /// <summary>
            /// If this member is TRUE, the partition information has changed. When you change a partition (with 
            /// IOCTL_DISK_SET_DRIVE_LAYOUT), the system uses this member to determine which partitions have changed
            /// and need their information rewritten.
            /// </summary>
            [FieldOffset(28)]
            [MarshalAs(UnmanagedType.I1)]
            public bool RewritePartition;

            /// <summary>
            /// A PARTITION_INFORMATION_MBR structure that specifies partition information specific to master boot 
            /// record (MBR) disks. The MBR partition format is the standard AT-style format.
            /// </summary>
            [FieldOffset(32)]
            public PARTITION_INFORMATION_MBR Mbr;

            /// <summary>
            /// A PARTITION_INFORMATION_GPT structure that specifies partition information specific to GUID partition 
            /// table (GPT) disks. The GPT format corresponds to the EFI partition format.
            /// </summary>
            [FieldOffset(32)]
            public PARTITION_INFORMATION_GPT Gpt;
        } 
        #endregion
    }
}
