using System.IO.Abstractions;

namespace FireApp.Identifiers;

public interface IIdentifierStrategy<TFileId>
{
    bool TryValueFileId(IFileInfo file, out TFileId? fileId);
}

public record IdentifiedFile<TFileId>(
    TFileId Id,
    IFileInfo File);