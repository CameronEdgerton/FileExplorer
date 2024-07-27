using FileFolderExplorer.Models;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFolderRepository
{
    Task<List<Folder>> GetFolderTreeAsync();
    Task AddAsync(Folder folder);
    Task<bool> FolderExistsById(Guid folderId);
    Task<bool> AnyFolderExists();
    Task<Folder?> GetFolderByIdAsync(Guid folderId);
}