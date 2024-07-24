using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFileRepository
{
    Task<File?> GetByIdAsync(Guid id);
    Task AddAsync(File file);
}