using FileFolderExplorer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories;

public class FileRepository(FileFolderExplorerContext dbContext) : IFileRepository
{
    public async Task<File?> GetByIdAsync(Guid id)
    {
        return await dbContext.Files.FirstOrDefaultAsync(f => f.FileId == id);
    }

    public async Task AddAsync(File file)
    {
        await dbContext.Files.AddAsync(file);
        await dbContext.SaveChangesAsync();;
    }
}