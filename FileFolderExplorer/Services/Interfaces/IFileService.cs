﻿using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Services.Interfaces;

public interface IFileService
{
    Task<File> UploadFileAsync(IFormFile file, Guid folderId);
    Task<string> GetFileContentByIdAsync(Guid fileId);
}