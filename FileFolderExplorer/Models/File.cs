using System.ComponentModel.DataAnnotations;

namespace FileFolderExplorer.Models;

public class File
{
    public Guid FileId { get; set; }
    public required string Name { get; set; }
    public required string FileType { get; set; }
    public required string Path { get; set; }
    public Guid FolderId { get; set; }
    public Folder Folder { get; set; }
}