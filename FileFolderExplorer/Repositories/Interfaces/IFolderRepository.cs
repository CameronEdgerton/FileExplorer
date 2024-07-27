using FileFolderExplorer.Models;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFolderRepository
{
    Task<IList<Folder>> GetAllAsync();
    Task AddAsync(Folder folder);
    Task<bool> FolderExistsById(Guid folderId);
    Task<bool> AnyFolderExists();
    Task<Folder?> GetFolderByIdAsync(Guid folderId);
}