namespace FileFolderExplorer.Dtos;

public class CreateFolderRequest
{
    public string FolderName { get; set; } = string.Empty;
    public string ParentFolderId { get; set; } = string.Empty;
}