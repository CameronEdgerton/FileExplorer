﻿namespace FileFolderExplorer.Models;

public class CreateFolderRequest
{
    public string FolderName { get; set; }
    public string ParentFolderId { get; set; }
}