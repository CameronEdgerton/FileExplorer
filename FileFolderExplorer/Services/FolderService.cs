using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;

namespace FileFolderExplorer.Services;

public class FolderService(IFolderRepository repository) : IFolderService
{
    public async Task<Folder> CreateFolderAsync(string name, Guid? parentId)
    {
        var folder = new Folder()
        {
            FolderId = Guid.NewGuid(),
            Name = name,
            ParentFolderId = parentId
        };
        
        // If we have the parentId, we need to validate the parent folder exists before creating a relationship with
        // the new folder
        if (parentId != null)
        {
            var parentExists = await repository.FolderExistsById(parentId.Value);
            if (!parentExists)
            {
                throw new Exception("Parent folder not found");
            }
        }
        else
        {
            // if the parentId is null we add the folder to the root directory
            var anyFolderExists = await repository.AnyFolderExists();
            if (!anyFolderExists)
            {
                // If there are no folders in the database, the folder becomes the root folder
                await repository.AddAsync(folder);
                return folder;
            }
            // If there are folders in the database but we haven't specified a parent folder, we should throw an error
            throw new Exception("Invalid parent folder id");
        }
        
        await repository.AddAsync(folder);
        return folder;
    }
    
    public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
    {
        return await repository.GetAllAsync();
    }
}
