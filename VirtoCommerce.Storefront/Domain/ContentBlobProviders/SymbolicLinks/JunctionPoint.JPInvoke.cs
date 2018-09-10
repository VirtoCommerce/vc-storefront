using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace VirtoCommerce.Storefront.Domain.ContentBlobProviders.SymbolicLinks
{
    [SecurityCritical]
    public static partial class JunctionPoint
    {
        public class JPInvoke
        {

            /// <summary>
            /// This prefix indicates to NTFS that the path is to be treated as a non-interpreted
            /// path in the virtual file system.
            /// </summary>
            private const string NonInterpretedPathPrefix = @"\??\";

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
                IntPtr InBuffer, int nInBufferSize,
                IntPtr OutBuffer, int nOutBufferSize,
                out int pBytesReturned, IntPtr lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr CreateFile(
                string lpFileName,
                FileAccess dwDesiredAccess,
                FileShare dwShareMode,
                IntPtr lpSecurityAttributes,
                CreationDisposition dwCreationDisposition,
                FileAttributes dwFlagsAndAttributes,
                IntPtr hTemplateFile);

            /// <summary>
            /// Creates a junction point from the specified directory to the specified target directory.
            /// </summary>
            /// <remarks>
            /// Only works on NTFS.
            /// </remarks>
            /// <param name="junctionPoint">The junction point path</param>
            /// <param name="targetDir">The target directory</param>
            /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
            /// <exception cref="IOException">Thrown when the junction point could not be created or when
            /// an existing directory was found and <paramref name="overwrite" /> if false</exception>
            public static void Create(string junctionPoint, string targetDir, bool overwrite)
            {
                targetDir = Path.GetFullPath(targetDir);

                if (!Directory.Exists(targetDir))
                    throw new IOException("Target path does not exist or is not a directory.");

                if (Directory.Exists(junctionPoint))
                {
                    if (!overwrite)
                        throw new IOException("Directory already exists and overwrite parameter is false.");
                }
                else
                {
                    Directory.CreateDirectory(junctionPoint);
                }

                using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, FileAccess.GenericWrite))
                {
                    byte[] targetDirBytes =
                        Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + Path.GetFullPath(targetDir));

                    REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

                    reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                    reparseDataBuffer.ReparseDataLength = (ushort)(targetDirBytes.Length + 12);
                    reparseDataBuffer.SubstituteNameOffset = 0;
                    reparseDataBuffer.SubstituteNameLength = (ushort)targetDirBytes.Length;
                    reparseDataBuffer.PrintNameOffset = (ushort)(targetDirBytes.Length + 2);
                    reparseDataBuffer.PrintNameLength = 0;
                    reparseDataBuffer.PathBuffer = new byte[0x3ff0];
                    Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

                    int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                    IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);

                    try
                    {
                        Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                        int bytesReturned;
                        bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT,
                            inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                        if (!result)
                            ThrowLastWin32Error("Unable to create junction point.");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(inBuffer);
                    }
                }
            }

            /// <summary>
            /// Deletes a junction point at the specified source directory along with the directory itself.
            /// Does nothing if the junction point does not exist.
            /// </summary>
            /// <remarks>
            /// Only works on NTFS.
            /// </remarks>
            /// <param name="junctionPoint">The junction point path</param>
            public static void Delete(string junctionPoint)
            {
                if (!Directory.Exists(junctionPoint))
                {
                    if (File.Exists(junctionPoint))
                        throw new IOException("Path is not a junction point.");

                    return;
                }

                using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, FileAccess.GenericWrite))
                {
                    REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

                    reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
                    reparseDataBuffer.ReparseDataLength = 0;
                    reparseDataBuffer.PathBuffer = new byte[0x3ff0];

                    int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
                    IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);
                    try
                    {
                        Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                        int bytesReturned;
                        bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
                            inBuffer, 8, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

                        if (!result)
                            ThrowLastWin32Error("Unable to delete junction point.");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(inBuffer);
                    }

                    try
                    {
                        Directory.Delete(junctionPoint);
                    }
                    catch (IOException ex)
                    {
                        throw new IOException("Unable to delete junction point.", ex);
                    }
                }
            }

            /// <summary>
            /// Determines whether the specified path exists and refers to a junction point.
            /// </summary>
            /// <param name="path">The junction point path</param>
            /// <returns>True if the specified path represents a junction point</returns>
            /// <exception cref="IOException">Thrown if the specified path is invalid
            /// or some other error occurs</exception>
            public static bool Exists(string path)
            {
                if (!Directory.Exists(path))
                    return false;

                using (SafeFileHandle handle = OpenReparsePoint(path, FileAccess.GenericRead))
                {
                    string target = InternalGetTarget(handle);
                    return target != null;
                }
            }

            /// <summary>
            /// Gets the target of the specified junction point.
            /// </summary>
            /// <remarks>
            /// Only works on NTFS.
            /// </remarks>
            /// <param name="junctionPoint">The junction point path</param>
            /// <returns>The target of the junction point</returns>
            /// <exception cref="IOException">Thrown when the specified path does not
            /// exist, is invalid, is not a junction point, or some other error occurs</exception>
            public static string GetTarget(string junctionPoint)
            {
                using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, FileAccess.GenericRead))
                {
                    string target = InternalGetTarget(handle);
                    if (target == null)
                        throw new IOException("Path is not a junction point.");

                    return target;
                }
            }

            private static string InternalGetTarget(SafeFileHandle handle)
            {
                int outBufferSize = Marshal.SizeOf(typeof(REPARSE_DATA_BUFFER));
                IntPtr outBuffer = Marshal.AllocHGlobal(outBufferSize);

                try
                {
                    int bytesReturned;
                    bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT,
                        IntPtr.Zero, 0, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

                    if (!result)
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error == ERROR_NOT_A_REPARSE_POINT)
                            return null;

                        ThrowLastWin32Error("Unable to get information about junction point.");
                    }

                    REPARSE_DATA_BUFFER reparseDataBuffer = (REPARSE_DATA_BUFFER)
                        Marshal.PtrToStructure(outBuffer, typeof(REPARSE_DATA_BUFFER));

                    if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT &&
                        reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_SYMLINK)
                        return null;

                    if (reparseDataBuffer.ReparseTag == IO_REPARSE_TAG_SYMLINK)
                        reparseDataBuffer.SubstituteNameOffset += 4; // sizeof(ULONG)

                    string targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                        reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);

                    if (targetDir.StartsWith(NonInterpretedPathPrefix))
                        targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);



                    return targetDir;
                }
                finally
                {
                    Marshal.FreeHGlobal(outBuffer);
                }
            }

            private static SafeFileHandle OpenReparsePoint(string reparsePoint, FileAccess accessMode)
            {
                SafeFileHandle reparsePointHandle = new SafeFileHandle(CreateFile(reparsePoint, accessMode,
                    FileShare.Read | FileShare.Write | FileShare.Delete,
                    IntPtr.Zero, CreationDisposition.OpenExisting,
                    FileAttributes.BackupSemantics | FileAttributes.OpenReparsePoint, IntPtr.Zero), true);

                if (Marshal.GetLastWin32Error() != 0)
                    ThrowLastWin32Error("Unable to open reparse point.");

                return reparsePointHandle;
            }

            private static void ThrowLastWin32Error(string message)
            {
                throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            }

        }
    }
}
