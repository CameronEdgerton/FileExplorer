using FileFolderExplorer.Models;

namespace FileFolderExplorer.Services.Interfaces;

public interface IFolderService
{
    Task<Folder?> CreateFolderAsync(string folderName, Guid? parentFolderId);
    Task<Folder?> GetFolderTreeAsync();
    Task<Folder?> GetFolderByIdAsync(Guid folderId);
}