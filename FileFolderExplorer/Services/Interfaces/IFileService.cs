using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services.Interfaces;

public interface IFileService
{
    Task<File> UploadFileAsync(IFormFile file, Guid folderId);
    Task<File?> GetFileByIdAsync(Guid fileId);
    Task<IEnumerable<File>> GetFilesByFolderIdAsync(Guid folderId);
}