using FileFolderExplorer.Repositories.Interfaces;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories;

public class FileRepository(FileFolderExplorerContext dbContext) : IFileRepository
{
    public async Task UploadFileAsync(File file)
    {
        await dbContext.Files.AddAsync(file);
        await dbContext.SaveChangesAsync();
    }

    public async Task<File?> GetFileByIdAsync(Guid fileId)
    {
        return await dbContext.Files.FindAsync(fileId);
    }
}