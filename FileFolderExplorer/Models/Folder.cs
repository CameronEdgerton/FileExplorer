namespace FileFolderExplorer.Models;

public class Folder
{
    public Guid FolderId { get; set; }
    public Guid? ParentId { get; set; }
    public required string Name { get; set; }
    public IList<File> Files { get; set; } = [];
}