﻿using System.Text;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services.Interfaces;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services;

public class FileService(IFileRepository fileRepository, IFolderRepository folderRepository) : IFileService
{
    public async Task<File> UploadFileAsync(IFormFile formFile, Guid folderId)
    {
        ValidateFormFile(formFile);
        await VerifyFolderCanAcceptFile(folderId, formFile.FileName);

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

        await fileRepository.UploadFileAsync(file);
        return file;
    }

    public async Task<string> GetFileContentByIdAsync(Guid fileId)
    {
        var file = await fileRepository.GetFileByIdAsync(fileId);
        return file?.Content != null ? Encoding.UTF8.GetString(file.Content) : string.Empty;
    }

    private async Task VerifyFolderCanAcceptFile(Guid folderId, string fileName)
    {
        var folder = await folderRepository.GetFolderByIdAsync(folderId);
        if (folder == null)
            throw new ArgumentException("Folder not found");
        if (folder.Files.Any(x => x.Name == fileName))
            throw new ArgumentException("File already exists in folder");
    }

    private static void ValidateFormFile(IFormFile formFile)
    {
        ValidateFileNotEmpty(formFile);
        ValidateFileExtension(formFile);
    }

    private static void ValidateFileNotEmpty(IFormFile formFile)
    {
        if (formFile.Length < 1)
            throw new ArgumentException("File is empty");
    }

    private static void ValidateFileExtension(IFormFile formFile)
    {
        var extension = Path.GetExtension(formFile.FileName);
        if (extension != ".csv" && extension != ".geojson")
            throw new ArgumentException("Invalid file type");
    }
}