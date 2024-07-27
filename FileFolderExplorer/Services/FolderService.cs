using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;

namespace FileFolderExplorer.Services;

public class FolderService(IFolderRepository folderRepository) : IFolderService
{
    public async Task<Folder?> CreateFolderAsync(string folderName, Guid? parentFolderId)
    {
        if (string.IsNullOrWhiteSpace(folderName)) throw new ArgumentException("Folder name cannot be empty");

        var folder = new Folder
        {
            FolderId = Guid.NewGuid(),
            Name = folderName,
            ParentFolderId = parentFolderId
        };

        var parentFolderExists = await ParentFolderExists(parentFolderId);
        if (!parentFolderExists) return await CreateRootFolder(folder);

        await folderRepository.AddAsync(folder);

        // Get the folder back from the db with its relationships populated
        return await GetFolderByIdAsync(folder.FolderId);
    }

    public async Task<Folder?> GetFolderTreeAsync()
    {
        var folders = await folderRepository.GetFolderTreeAsync();
        return folders.FirstOrDefault();
    }

    public async Task<Folder?> GetFolderByIdAsync(Guid folderId)
    {
        return await folderRepository.GetFolderByIdAsync(folderId);
    }

    private async Task<bool> ParentFolderExists(Guid? parentFolderId)
    {
        if (parentFolderId == null) return false;
        var parentExists = await folderRepository.FolderExistsById(parentFolderId.Value);
        if (!parentExists) throw new ArgumentException("Parent folder not found");
        return true;
    }

    private async Task<Folder> CreateRootFolder(Folder folder)
    {
        // If there are no folders in the database, the folder becomes the root folder
        var anyFolderExists = await folderRepository.AnyFolderExists();
        if (anyFolderExists) throw new ArgumentException("Cannot create root folder when folders already exist");
        await folderRepository.AddAsync(folder);
        return folder;
    }
}