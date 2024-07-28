using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFileRepository
{
    Task UploadFileAsync(File file);
    Task<File?> GetFileByIdAsync(Guid fileId);
}