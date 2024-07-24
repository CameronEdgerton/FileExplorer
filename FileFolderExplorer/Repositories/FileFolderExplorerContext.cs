using FileFolderExplorer.Models;
using Microsoft.EntityFrameworkCore;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Repositories;

public class FileFolderExplorerContext(DbContextOptions<FileFolderExplorerContext> options) : DbContext(options)
{
    public DbSet<Folder> Folders { get; set; }
    public DbSet<File> Files { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<File>()
            .HasKey(f => f.FileId);

        modelBuilder.Entity<File>()
            .HasOne(f => f.Folder)
            .WithMany(f => f.Files)
            .HasForeignKey(f => f.FolderId);

        modelBuilder.Entity<Folder>()
            .HasKey(f => f.FolderId);

        modelBuilder.Entity<Folder>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.Subfolders)
            .HasForeignKey(f => f.ParentFolderId);
    }
}