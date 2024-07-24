using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Moq;

namespace FileFolderExplorer.UnitTest;

public class FolderServiceUnitTests
{
    private readonly Mock<IFolderRepository> _mockFolderRepository;
    private readonly FolderService _folderService;

    public FolderServiceUnitTests()
    {
        _mockFolderRepository = new Mock<IFolderRepository>();
        _folderService = new FolderService(_mockFolderRepository.Object);
    }
    
    [Fact]
    public async Task CreateFolderAsync_WithValidParentId_ReturnsFolder()
    {
        // Arrange
        const string folderName = "Test Folder";
        var parentId = Guid.NewGuid();
        
        _mockFolderRepository
            .Setup(repo => repo.FolderExistsById(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var folder = await _folderService.CreateFolderAsync(folderName, parentId);

        // Assert
        folder.Should().NotBeNull();
        folder.Name.Should().Be(folderName);
        folder.ParentId.Should().Be(parentId);
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateFolderAsync_WithInvalidParentId_ThrowsError()
    {
        // Arrange
        const string folderName = "Test Folder";
        var parentId = Guid.NewGuid();
        
        _mockFolderRepository
            .Setup(repo => repo.FolderExistsById(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var act = async() =>
        {
            await _folderService.CreateFolderAsync(folderName, parentId);
        };

        // Assert
        await act.Should().ThrowAsync<Exception>("Parent folder not found");
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateFolderAsync_WithNoParentId_ReturnsFolder_IfNoneInDb()
    {
        // Arrange
        const string folderName = "Test Folder";
        
        _mockFolderRepository
            .Setup(repo => repo.AnyFolderExists())
            .ReturnsAsync(false);

        // Act
        var folder = await _folderService.CreateFolderAsync(folderName, null);

        // Assert
        folder.Should().NotBeNull();
        folder.Name.Should().Be(folderName);
        folder.ParentId.Should().BeNull();
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateFolderAsync_WithNoParentId_ThrowsError_IfFoldersInDb()
    {
        // Arrange
        const string folderName = "Test Folder";
        
        _mockFolderRepository
            .Setup(repo => repo.AnyFolderExists())
            .ReturnsAsync(true);

        // Act
        var act = async() =>
        {
            await _folderService.CreateFolderAsync(folderName, null);
        };

        // Assert
        await act.Should().ThrowAsync<Exception>("Invalid parent folder id");
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Never);
    }
    
   
}