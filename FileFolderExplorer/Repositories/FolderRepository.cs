using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileFolderExplorer.Repositories;

public class FolderRepository(FileFolderExplorerContext dbContext) : IFolderRepository
{
    public async Task AddAsync(Folder entity)
    {
        await dbContext.Folders.AddAsync(entity);
        await dbContext.SaveChangesAsync();;
    }

    public async Task<IEnumerable<Folder>> GetAllAsync()
    {
        return await dbContext.Folders.ToListAsync();
    }

    public async Task<bool> FolderExistsById(Guid id)
    {
        return await dbContext.Folders.AnyAsync(f => f.FolderId == id);
    }

    public async Task<bool> AnyFolderExists()
    {
        return await dbContext.Folders.AnyAsync();
    }
}