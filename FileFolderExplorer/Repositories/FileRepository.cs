using FileFolderExplorer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories;

public class FileRepository(FileFolderExplorerContext dbContext) : IFileRepository
{
    public async Task<File?> GetFileByIdAsync(Guid id)
    {
        return await dbContext.Files.FindAsync(id);
    }

    public async Task AddAsync(File file)
    {
        await dbContext.Files.AddAsync(file);
        await dbContext.SaveChangesAsync();;
    }

    public async Task<IEnumerable<File>> GetFilesByFolderIdAsync(Guid folderId)
    {
        return await dbContext.Files.Where(f => f.FolderId == folderId).ToListAsync();
    }
}