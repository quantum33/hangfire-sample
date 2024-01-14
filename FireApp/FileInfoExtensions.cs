using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace FireApp;

public static class FileInfoExtensions
{
    public static byte[] FileContentAsMd5(this IFileInfo fileInfo)
    {
        using var md5 = MD5.Create();
        using var stream = fileInfo.OpenRead();
        byte[] hash = md5.ComputeHash(stream);

        return hash;
    }
}