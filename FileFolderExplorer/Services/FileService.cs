using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services;

public class FileService(IFileRepository repository, IConfiguration configuration) : IFileService
{
    private readonly string _storagePath = configuration["StoragePath"] ?? Path.GetTempPath();
    public async Task<File> UploadFileAsync(IFormFile formFile, Guid folderId)
    {
        if (formFile.Length < 1)
            throw new ArgumentException("No file uploaded");
        
        // check the extension is .csv or .geojson
        var extension = Path.GetExtension(formFile.FileName);
        if (extension != ".csv" && extension != ".geojson")
            throw new ArgumentException("Invalid file type");

        var filePath = Path.Combine(_storagePath, formFile.FileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(stream);
        }

        var file = new File
        {
            FileId = Guid.NewGuid(),
            Name = formFile.FileName,
            FilePath = filePath,
            FolderId = folderId
        };

        await repository.AddAsync(file);
        return file;
    }
    
    public async Task<FormFile> GetFileAsync(Guid fileId)
    {
        var file = await repository.GetByIdAsync(fileId);
        if (file == null)
        {
            throw new Exception("File not found.");
        }

        var formFile = new FormFile(new FileStream(file.FilePath, FileMode.Open), 0, new FileInfo(file.FilePath).Length, file.Name, file.Name)
        {
            Headers = new HeaderDictionary(),
            ContentType = "multipart/form-data"
        };

        return formFile;
    }
}