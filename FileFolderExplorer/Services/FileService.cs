using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services;

public class FileService(IFileRepository fileRepository, IFolderRepository folderRepository) : IFileService
{
    public async Task<File> UploadFileAsync(IFormFile formFile, Guid folderId)
    {
        if (formFile.Length < 1)
            throw new ArgumentException("File is empty");

        // verify the folderId exists
        var folderExists = await folderRepository.FolderExistsById(folderId);
        if (!folderExists)
            throw new ArgumentException("Folder not found");

        // check the extension is .csv or .geojson
        var extension = Path.GetExtension(formFile.FileName);
        if (extension != ".csv" && extension != ".geojson")
            throw new ArgumentException("Invalid file type");

        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        var fileContent = memoryStream.ToArray();

        var file = new File
        {
            FileId = Guid.NewGuid(),
            Name = formFile.FileName,
            Content = fileContent,
            FolderId = folderId
        };

        await fileRepository.AddAsync(file);
        return file;
    }

    public async Task<File?> GetFileByIdAsync(Guid fileId)
    {
        return await fileRepository.GetFileByIdAsync(fileId);
    }

    public async Task<IEnumerable<File>> GetFilesByFolderIdAsync(Guid folderId)
    {
        return await fileRepository.GetFilesByFolderIdAsync(folderId);
    }
}