using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
namespace FireApp;

public static class FileIdentifier
{
    /*
     * https://stackoverflow.com/questions/1866454/unique-file-identifier-in-windows
     * 
     * If you call GetFileInformationByHandle, you'll get a file ID in BY_HANDLE_FILE_INFORMATION.nFileIndexHigh/Low.
     * This index is unique within a volume, and stays the same even if you move the file (within the volume) or rename it.
     * If you can assume that NTFS is used, you may also want to consider using Alternate Data Streams to store the metadata.
     */
    public static bool TryGetFileUniqueSystemId(string fileName, out string? fileId)
    {
        fileId = GetFileUniqueSystemId(fileName);
        return !string.IsNullOrEmpty(fileId);
    }
    
    private static string? GetFileUniqueSystemId(string fileName)
    {
        bool fileRead = false;

        while (!fileRead)
        {
            try
            {
                using FileStream stream = File.Open(fileName, FileMode.Open);
                BY_HANDLE_FILE_INFORMATION byHandleFileInformation = new BY_HANDLE_FILE_INFORMATION();
                GetFileInformationByHandle(stream.SafeFileHandle, out byHandleFileInformation);
                string fileId = byHandleFileInformation.FileIndexHigh +
                         byHandleFileInformation.FileIndexLow.ToString() +
                         "|" +
                         byHandleFileInformation.VolumeSerialNumber;
                stream.Close();
                fileRead = true;

                return fileId;
            }
            catch (IOException e)
            {
                if (e is FileNotFoundException or PathTooLongException)
                {
                    fileRead = true;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            catch (UnauthorizedAccessException)
            {
                fileRead = true;
            }
        }

        return null;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandle(
        SafeFileHandle hFile,
        out BY_HANDLE_FILE_INFORMATION lpFileInformation);

    struct BY_HANDLE_FILE_INFORMATION
    {
        public uint FileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }
}