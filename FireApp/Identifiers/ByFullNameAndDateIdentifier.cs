using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace FireApp.Identifiers;

public class ByFullNameAndDateIdentifier : IdentifierStrategy
{
    public override bool TryValueFileId(IFileInfo file, out IdentifiedFile fileId)
    {
        string filePathCreationComposite = $"{Path.GetFullPath(file.FullName)}{file.CreationTime}";

        MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(filePathCreationComposite)); 
        Guid id = new Guid(hash);

        fileId = new IdentifiedFile(
            id.ToString(),
            file,
            typeof(ByFullNameAndDateIdentifier));
        
        return true;
    }
}