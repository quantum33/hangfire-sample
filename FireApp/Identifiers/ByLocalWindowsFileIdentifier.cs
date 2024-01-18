using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace FireApp.Identifiers;

public class ByLocalWindowsFileIdentifier : IdentifierStrategy
{
    public record FileId(IFileInfo FileInfo) : IGetFileId
    {
        public string ValueFileId() => FileIndexHigh +
                                       FileIndexLow.ToString() +
                                       "|" +
                                       VolumeSerialNumber;
        public uint FileAttributes { get; init; }
        public DateTime CreationTime { get; init; }
        public DateTime LastAccessTime { get; init; }
        public DateTime LastWriteTime { get; init; }
        public uint VolumeSerialNumber { get; init; }
        public uint FileSizeHigh { get; init; }
        public uint FileSizeLow { get; init; }
        public uint NumberOfLinks { get; init; }
        public uint FileIndexHigh { get; init; }
        public uint FileIndexLow { get; init; }
    }

    public override bool TryValueFileId(IFileInfo file, out IdentifiedFile? fileId)
    {
        fileId = GetFileUniqueSystemId(file);
        return fileId != null;
    }

    /*
     * https://stackoverflow.com/questions/1866454/unique-file-identifier-in-windows
     *
     * If you call GetFileInformationByHandle, you'll get a file ID in BY_HANDLE_FILE_INFORMATION.nFileIndexHigh/Low.
     * This index is unique within a volume, and stays the same even if you move the file (within the volume) or rename it.
     * If you can assume that NTFS is used, you may also want to consider using Alternate Data Streams to store the metadata.
     */

    private static IdentifiedFile? GetFileUniqueSystemId(IFileInfo file)
    {
        bool fileRead = false;

        while (!fileRead)
        {
            try
            {
                using FileStream stream = File.Open(file.FullName, FileMode.Open);
                BY_HANDLE_FILE_INFORMATION byHandleFileInformation = new BY_HANDLE_FILE_INFORMATION();
                GetFileInformationByHandle(stream.SafeFileHandle, out byHandleFileInformation);
                FileId intermediate = ToWindowsFileId(byHandleFileInformation, file);

                var result = new IdentifiedFile(
                    intermediate.ValueFileId(),
                    file,
                    typeof(ByLocalWindowsFileIdentifier));
                
                stream.Close();
                fileRead = true;

                return result;
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
        public FILETIME CreationTime;
        public FILETIME LastAccessTime;
        public FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    private static FileId ToWindowsFileId(BY_HANDLE_FILE_INFORMATION fileInformation, IFileInfo file) => new(file)
    {
        FileAttributes = fileInformation.FileAttributes,
        CreationTime = ToDateTime(fileInformation.CreationTime),
        LastAccessTime = ToDateTime(fileInformation.LastAccessTime),
        LastWriteTime = ToDateTime(fileInformation.LastWriteTime),
        VolumeSerialNumber = fileInformation.VolumeSerialNumber,
        FileSizeHigh = fileInformation.FileSizeHigh,
        FileSizeLow = fileInformation.FileSizeLow,
        NumberOfLinks = fileInformation.NumberOfLinks,
        FileIndexHigh = fileInformation.FileIndexHigh,
        FileIndexLow = fileInformation.FileIndexLow
    };

    private static DateTime ToDateTime(FILETIME fileTime)
    {
        long highBits = fileTime.dwHighDateTime;
        highBits = highBits << 32;

        return DateTime.FromFileTimeUtc(highBits + (uint)fileTime.dwLowDateTime);
    }
}