namespace FileFolderExplorer.Models;

public class Folder
{
    public Guid FolderId { get; set; }
    public Guid? ParentFolderId { get; set; }
    public required string Name { get; set; }
    public Folder ParentFolder { get; set; }
    public ICollection<File> Files { get; set; } = [];
    public ICollection<Folder> Subfolders { get; set; } = [];
}