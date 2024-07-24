using FileFolderExplorer.Models;

namespace FileFolderExplorer.Services.Interfaces;

public interface IFolderService
{
    Task<Folder> CreateFolderAsync(string name, Guid? parentId);
    Task<IEnumerable<Folder>> GetAllFoldersAsync();
}