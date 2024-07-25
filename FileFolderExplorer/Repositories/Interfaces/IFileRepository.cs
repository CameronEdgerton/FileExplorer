using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFileRepository
{
    Task<File?> GetFileByIdAsync(Guid id);
    Task AddAsync(File file);
    Task<IEnumerable<File>> GetFilesByFolderIdAsync(Guid folderId);
}