namespace FileFolderExplorer.Dtos;

public class UploadFileRequest
{
    public IFormFile File { get; set; }
    public string FolderId { get; set; }
}