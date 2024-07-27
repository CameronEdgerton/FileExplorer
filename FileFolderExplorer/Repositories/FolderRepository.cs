﻿using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileFolderExplorer.Repositories;

public class FolderRepository(FileFolderExplorerContext dbContext) : IFolderRepository
{
    public async Task AddAsync(Folder entity)
    {
        await dbContext.Folders.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IList<Folder>> GetAllAsync()
    {
        return await dbContext.Folders
            .Include(x => x.Files)
            .Include(x => x.ParentFolder)
            .Include(x => x.Subfolders)
            .OrderByDescending(x => x.ParentFolder)
            .ToListAsync();
    }

    public async Task<bool> FolderExistsById(Guid folderId)
    {
        return await dbContext.Folders.AnyAsync(f => f.FolderId == folderId);
    }

    public async Task<bool> AnyFolderExists()
    {
        return await dbContext.Folders.AnyAsync();
    }

    public async Task<Folder?> GetFolderByIdAsync(Guid folderId)
    {
        return await dbContext.Folders
            .Include(x => x.Subfolders)
            .Include(x => x.ParentFolder)
            .Include(x => x.Files)
            .FirstOrDefaultAsync(x => x.FolderId == folderId);
    }
}