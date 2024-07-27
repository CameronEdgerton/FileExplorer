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
        modelBuilder.Entity<Folder>(entity =>
        {
            entity.HasKey(f => f.FolderId);
            entity.Property(f => f.Name).IsRequired();

            entity.HasMany(f => f.Files)
                .WithOne(f => f.Folder)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(f => f.Subfolders)
                .WithOne(f => f.ParentFolder)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(f => f.FileId);
            entity.Property(f => f.Name).IsRequired();
            entity.Property(f => f.Content).IsRequired();

            entity.HasOne(f => f.Folder)
                .WithMany(f => f.Files)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}