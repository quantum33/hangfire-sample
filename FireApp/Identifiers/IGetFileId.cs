using System.IO.Abstractions;

namespace FireApp.Identifiers;

public interface IGetFileId
{
    public string ValueFileId();
    
    public IFileInfo FileInfo { get; }
}