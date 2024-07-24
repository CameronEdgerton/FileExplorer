using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services;

public class FileService(IFileRepository repository, IWebHostEnvironment hostEnvironment) : IFileService
{
    public async Task<File> UploadFileAsync(IFormFile file, Guid folderId)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded");
        
        var fileName = Path.GetFileName(file.FileName);
        
        // check the extension is .csv or .geojson
        var extension = Path.GetExtension(fileName);
        if (extension != ".csv" && extension != ".geojson")
            throw new ArgumentException("Invalid file type");

        var filePath = Path.Combine(hostEnvironment.WebRootPath, "uploads", fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var uploadedFile = new File()
        {
            FileId = Guid.NewGuid(),
            Name = fileName,
            FileType = extension,
            FolderId = folderId,
            Path = filePath
        };

        await repository.AddAsync(uploadedFile);
        return uploadedFile;
    }
}