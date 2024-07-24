using FileFolderExplorer.Models;

namespace FileFolderExplorer.Repositories.Interfaces;

public interface IFolderRepository
{
    Task<IEnumerable<Folder>> GetAllAsync();
    Task AddAsync(Folder folder);
    Task<bool> FolderExists(Guid id);
}