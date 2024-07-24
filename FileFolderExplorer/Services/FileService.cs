using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services;

public class FileService(IFileRepository repository) : IFileService
{
    public async Task<File> UploadFileAsync(IFormFile file, Guid folderId)
    {
        if (file.Length < 1)
            throw new ArgumentException("No file uploaded");
        
        // check the extension is .csv or .geojson
        var extension = Path.GetExtension(file.FileName);
        if (extension != ".csv" && extension != ".geojson")
            throw new ArgumentException("Invalid file type");

        var uploadedFile = new File()
        {
            FileId = Guid.NewGuid(),
            Name = file.FileName,
            FileType = extension,
            FolderId = folderId,
            FormFile = file
        };

        await repository.AddAsync(uploadedFile);
        return uploadedFile;
    }
}