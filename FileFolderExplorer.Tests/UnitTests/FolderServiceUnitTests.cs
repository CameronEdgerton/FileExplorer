using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Moq;

namespace FileFolderExplorer.UnitTest.UnitTests;

public class FolderServiceUnitTests
{
    private readonly FolderService _folderService;
    private readonly Mock<IFolderRepository> _mockFolderRepository;

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

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Folder
        {
            Name = folderName,
            ParentFolderId = parentId
        });

        // Act
        var folder = await _folderService.CreateFolderAsync(folderName, parentId);

        // Assert
        folder.Should().NotBeNull();
        folder.Name.Should().Be(folderName);
        folder.ParentFolderId.Should().Be(parentId);
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateFolderAsync_WithInvalidName_ThrowsError(string name)
    {
        // Arrange
        var parentId = Guid.NewGuid();

        // Act
        var act = async () => { await _folderService.CreateFolderAsync(name, parentId); };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("Folder name cannot be empty");
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Never);
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
        var act = async () => { await _folderService.CreateFolderAsync(folderName, parentId); };

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
        folder.ParentFolderId.Should().BeNull();
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
        var act = async () => { await _folderService.CreateFolderAsync(folderName, null); };

        // Assert
        await act.Should().ThrowAsync<Exception>("Invalid parent folder id");
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Never);
    }

    [Fact]
    public async Task GetAllFoldersAsync_ReturnsFolders()
    {
        // Arrange
        var folders = new List<Folder>
        {
            new() { Name = "Folder 1" },
            new() { Name = "Folder 2" }
        };

        _mockFolderRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(folders);

        // Act
        var result = await _folderService.GetAllFoldersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(folders);
    }

    [Fact]
    public async Task GetFolderByIdAsync_WithValidId_ReturnsFolder()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var folder = new Folder { Name = "Folder 1", FolderId = folderId };

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(folderId)).ReturnsAsync(folder);

        // Act
        var result = await _folderService.GetFolderByIdAsync(folderId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(folder);
    }

    [Fact]
    public async Task GetFolderByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var folderId = Guid.NewGuid();

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(folderId)).ReturnsAsync((Folder)null!);

        // Act
        var result = await _folderService.GetFolderByIdAsync(folderId);

        // Assert
        result.Should().BeNull();
    }
}