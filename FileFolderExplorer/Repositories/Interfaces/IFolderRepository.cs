using FileFolderExplorer.Models;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFolderRepository
{
    Task<IEnumerable<Folder>> GetAllAsync();
    Task AddAsync(Folder folder);
    Task<bool> FolderExistsById(Guid id);
    Task<bool> AnyFolderExists();
    Task<Folder?> GetFolderByIdAsync(Guid folderId);
}