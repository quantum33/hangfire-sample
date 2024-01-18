using System.IO.Abstractions;
using System.Text;

namespace FireApp.Identifiers;

public abstract class IdentifierStrategy
{
    public abstract bool TryValueFileId(IFileInfo file, out IdentifiedFile? fileId);
}

public record IdentifiedFile
{
    public IdentifiedFile(string id, IFileInfo file, Type identifierStrategyType)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        Id = id;
        File = file ?? throw new ArgumentNullException(nameof(file));
        IdentifierStrategyType = identifierStrategyType ?? throw new ArgumentNullException(nameof(identifierStrategyType));
    }

    public string Id { get; }

    public IFileInfo File { get; }

    public Type IdentifierStrategyType { get; }

    private byte[]? _fileContentAsHash;
    public byte[] FileHash => _fileContentAsHash ??= File.FileContentAsMd5();

    public string GetFileContentAsString()
    {
        // Convert byte array to a string
        StringBuilder result = new(FileHash.Length*2);

        foreach (byte t in FileHash)
        {
            result.Append(t.ToString("x2"));
        }

        return result.ToString();
    }
}