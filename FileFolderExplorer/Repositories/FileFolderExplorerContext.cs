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
        base.OnModelCreating(modelBuilder);
        
        // Define the primary keys for the entities
        modelBuilder.Entity<Folder>()
            .HasKey(f => f.FolderId)
            .HasName("PrimaryKey_FolderId");
        
        modelBuilder.Entity<File>()
            .HasKey(f => f.FileId)
            .HasName("PrimaryKey_FileId");

        // Define the relationship between Folder and its files
        modelBuilder.Entity<Folder>()
            .HasMany(f => f.Files)
            .WithOne(f => f.Folder)
            .HasForeignKey(file => file.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define the self-referencing relationship for Folder
        modelBuilder.Entity<Folder>()
            .HasOne<Folder>()
            .WithMany()
            .HasForeignKey(f => f.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}