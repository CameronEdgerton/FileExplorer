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

        var validParentFolderExists = await ValidParentFolderExists(parentFolderId, folderName);
        if (!validParentFolderExists) return await CreateRootFolder(folder);

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

    // Validate whether the parent folder exists. It is valid if the parent folder exists and the folder name is unique
    // within the parent folder
    private async Task<bool> ValidParentFolderExists(Guid? parentFolderId, string folderName)
    {
        if (parentFolderId == null) return false;

        var parentFolder = await folderRepository.GetFolderByIdAsync(parentFolderId.Value);
        if (parentFolder == null) throw new ArgumentException("Parent folder not found");

        if (parentFolder.Subfolders.Any(x => x.Name == folderName))
            throw new ArgumentException("Folder already exists in parent folder");

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