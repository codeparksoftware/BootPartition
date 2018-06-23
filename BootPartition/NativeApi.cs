using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static BootPartition.Native;

namespace BootPartition
{

    /// <summary>
    /// Native Api class contains Win Api Pinvokes and ListPartition function
    /// </summary>
    class NativeApi
    {
        /// <summary>
        /// Win NT machines accept max 64 drives
        /// So We define it 64
        /// </summary>
        public const uint MAX_NUMBER_OF_DRIVES = 64;

        /// <summary>
        /// Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
           string lpFileName,
           uint dwDesiredAccess,
           uint dwShareMode,
           IntPtr SecurityAttributes,
           uint dwCreationDisposition,
           uint dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        public void ListPartition()
        {
            for (uint i = 0; i < MAX_NUMBER_OF_DRIVES; i++)
            {
                // try to open the current physical drive
                string volume = string.Format("\\\\.\\PhysicalDrive{0}", i);
                SafeFileHandle hndl = CreateFile(volume,
                                                           Native.GENERIC_READ | Native.GENERIC_WRITE,
                                                           Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE,
                                                           IntPtr.Zero,
                                                           Native.OPEN_EXISTING,
                                                           Native.FILE_ATTRIBUTE_READONLY,
                                                           IntPtr.Zero);
                
                IntPtr driveLayoutPtr = IntPtr.Zero;
                int DRIVE_LAYOUT_BUFFER_SIZE = 1024;
                int error;
                uint dummy = 0;
                do
                {
                    error = 0;
                    driveLayoutPtr = Marshal.AllocHGlobal(DRIVE_LAYOUT_BUFFER_SIZE);
                    if (DeviceIoControl(hndl, Native.IOCTL_DISK_GET_DRIVE_LAYOUT_EX, IntPtr.Zero, 0, driveLayoutPtr, (uint)DRIVE_LAYOUT_BUFFER_SIZE, ref dummy, IntPtr.Zero))
                    {
                        // I/O-control has been invoked successfully, convert to DRIVE_LAYOUT_INFORMATION_EX
                        DRIVE_LAYOUT_INFORMATION_EX driveLayout = (DRIVE_LAYOUT_INFORMATION_EX)Marshal.PtrToStructure(driveLayoutPtr, typeof(DRIVE_LAYOUT_INFORMATION_EX));
                        //Enumerate partitions by using pointer arithmetic
                        for (uint p = 0; p < driveLayout.PartitionCount; p++)
                        {
                             
                            IntPtr ptr = new IntPtr(driveLayoutPtr.ToInt64() + Marshal.OffsetOf(typeof(DRIVE_LAYOUT_INFORMATION_EX), "PartitionEntry").ToInt64() + (p * Marshal.SizeOf(typeof(PARTITION_INFORMATION_EX))));
                            PARTITION_INFORMATION_EX partInfo = (PARTITION_INFORMATION_EX)Marshal.PtrToStructure(ptr, typeof(PARTITION_INFORMATION_EX));
                            
                            //Check partition is recognized or not
                            if ((partInfo.PartitionStyle != PARTITION_STYLE.PARTITION_STYLE_MBR) || (partInfo.Mbr.RecognizedPartition))
                            {
                                if (partInfo.Mbr.BootIndicator == true)
                                {

                                    Console.WriteLine("Drive No: " + i + " Partition Number :" + partInfo.PartitionNumber + " is boot partition");

                                }
                               
                            }
                         
                        }
                    }
                    else
                    {
                        error = Marshal.GetLastWin32Error();

                        DRIVE_LAYOUT_BUFFER_SIZE *= 2;
                    }
                    Marshal.FreeHGlobal(driveLayoutPtr);
                    driveLayoutPtr = IntPtr.Zero;
                } while (error == Native.ERROR_INSUFFICIENT_BUFFER);
            }
        }
    }
}
