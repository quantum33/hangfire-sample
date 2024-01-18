using System.Runtime.InteropServices;
using System.Text;

namespace FireApp.Identifiers;

public static class VolumeInfo
{
    [DllImport("kernel32.dll")]
    public static extern long GetVolumeInformationA(
        string pathName,
        StringBuilder volumeNameBuffer,
        UInt32 volumeNameSize,
        ref UInt32 volumeSerialNumber,
        ref UInt32 maximumComponentLength,
        ref UInt32 fileSystemFlags,
        StringBuilder fileSystemNameBuffer,
        UInt32 fileSystemNameSize);
}