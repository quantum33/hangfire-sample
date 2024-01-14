using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace FireApp;

public class FileHandler
{
    static Guid GetFileId(IFileSystem fileSystem, string filePath)
    {
        // FileInfo fileInfo = new FileInfo(filePath);
        IFileInfo fi = fileSystem.FileInfo.New(filePath);
        string filePathCreationComposite = $"{Path.GetFullPath(fi.FullName)}{fi.CreationTime}";

        MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(filePathCreationComposite)); 
        Guid result = new Guid(hash);
        return result;
    }

    static byte[] GetFileContentsAsHash(IFileInfo fileInfo)
    {
        using var sha256 = SHA256.Create();
        using var stream = fileInfo.OpenRead();
        byte[] hash = sha256.ComputeHash(stream);
        return hash;
    }
}