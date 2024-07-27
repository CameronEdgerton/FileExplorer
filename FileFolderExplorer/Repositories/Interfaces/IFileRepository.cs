using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFileRepository
{
    Task<File?> GetFileByIdAsync(Guid fileId);
    Task AddAsync(File file);
    Task<IEnumerable<File>> GetFilesByFolderIdAsync(Guid folderId);
}