using System.IO.Abstractions;
using System.Text;
using FireApp.Identifiers;

public static class FileTests
{
    public static void Run()
    {
        FileSystem fileSystem = new();
// ByLocalWindowsFileIdentifier strategy = new();
        ByFullNameIdentifier strategy = new();
// Console.WriteLine(
//     strategy.TryValueFileId(
//         file: fileSystem.FileInfo.New("Resources/sample1.txt"),
//         out var id1)
//         ? id1
//         : "No id found");
//
// Console.WriteLine(
//     strategy.TryValueFileId(
//         file: fileSystem.FileInfo.New("Resources/subFolder/sample2.txt"),
//         out var id2)
//         ? id2
//         : "No id found 2");

        const string NAS = @"\\192.168.1.135";

        var nasFileInfo = fileSystem.FileInfo.New(@$"{NAS}\Logiciels\shortcut-pop-os.png");

        strategy.TryValueFileId(nasFileInfo, out var nasFile);
        Console.WriteLine(nasFile!.Id == "034819|690572606");

        const string wsl = @"\\wsl.localhost";
        IFileInfo wslFileInfo = fileSystem.FileInfo.New(@$"{wsl}\Ubuntu\home\jamyz\toto.txt");
        strategy.TryValueFileId(wslFileInfo, out var wslFile);
        Console.WriteLine(wslFile?.Id);

        CallWin32Api(nasFileInfo);
    }

    static void CallWin32Api(IFileInfo fileInfo)
    {
        // string? root = Path.GetPathRoot(fileInfo.FullName);
        // FileSystemInfo toto = new FileInfo(fileInfo.FullName);

        string root = "C:\\";
        // const string root = @"\\1.2.3.4";

        try
        {
            // DriveInfo drive = new(root ?? string.Empty);
            // Console.WriteLine($"Format: {drive.DriveFormat}");
        
            uint serialNumber = 0;
            uint maxComponentLength = 0;
            StringBuilder sbVolumeName = new StringBuilder(256);
            UInt32 fileSystemFlags = new UInt32();
            StringBuilder fileSystemName = new StringBuilder(256);
        
            if (VolumeInfo.GetVolumeInformationA(
                    root,
                    sbVolumeName,
                    (UInt32)sbVolumeName.Capacity,
                    ref serialNumber,
                    ref maxComponentLength,
                    ref fileSystemFlags,
                    fileSystemName,
                    (UInt32)fileSystemName.Capacity) != 0)
            {
                Console.WriteLine($"FS ==> {fileSystemName}");
            }
            else
            {
                Console.WriteLine("oups.....");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}