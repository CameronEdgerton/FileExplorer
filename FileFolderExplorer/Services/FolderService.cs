using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;

namespace FileFolderExplorer.Services;

public class FolderService(IFolderRepository repository) : IFolderService
{
    private static readonly Guid RootId = Guid.NewGuid();

    public async Task<Folder> CreateFolderAsync(string name, Guid? parentId)
    {
        // If we have the parentId, we need to validate the parent folder exists before creating a relationship with
        // the new folder
        if (parentId.HasValue)
        {
            var parentExists = await repository.FolderExists(parentId.Value);
            if (!parentExists)
            {
                throw new Exception("Parent folder not found");
            }
        }
        else
        {
            // if the parentId is null we add the folder to the root directory
            var rootExists = await repository.FolderExists(RootId);
            if (!rootExists)
            {
                await repository.AddAsync(new Folder() { FolderId = RootId, Name = "Root" });
            }
            parentId = RootId;
        }
        
        var folder = new Folder()
        {
            FolderId = Guid.NewGuid(),
            Name = name,
            ParentId = parentId
        };
        
        await repository.AddAsync(folder);
        return folder;
    }
    
    public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
    {
        return await repository.GetAllAsync();
    }
}
