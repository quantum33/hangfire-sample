using System.IO.Abstractions;

namespace FireApp.Identifiers;

public class ByFullNameIdentifier : IdentifierStrategy
{
    public override bool TryValueFileId(IFileInfo file, out IdentifiedFile fileId)
    {
        fileId = new IdentifiedFile(
            id: file.FullName,
            file,
            typeof(ByFullNameIdentifier));

        return true;
    }
}

public class IdentifyFilesOrchestrator
{
    public HashSet<IdentifierStrategy> Strategies { get; } =
    [
        new ByFullNameIdentifier(),
        new ByLocalWindowsFileIdentifier(),
        new ByFullNameAndDateIdentifier(),
    ];
}