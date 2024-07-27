using FileFolderExplorer.Models;

namespace FileFolderExplorer.Services.Interfaces;

public interface IFolderService
{
    Task<Folder?> CreateFolderAsync(string folderName, Guid? parentFolderId);
    Task<IList<Folder>> GetAllFoldersAsync();
    Task<Folder?> GetFolderByIdAsync(Guid folderId);
}