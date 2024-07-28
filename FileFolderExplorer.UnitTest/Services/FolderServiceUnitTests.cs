using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Moq;

namespace FileFolderExplorer.UnitTest.Services;

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
        var folder = new Folder
        {
            FolderId = Guid.NewGuid(),
            Name = folderName,
            ParentFolderId = Guid.NewGuid()
        };

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(folder.ParentFolderId.Value)).ReturnsAsync(
            new Folder
            {
                Name = "parent folder",
                ParentFolderId = Guid.NewGuid()
            });

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(It.IsAny<Guid>())).ReturnsAsync(folder);

        // Act
        var resultFolder = await _folderService.CreateFolderAsync(folderName, folder.ParentFolderId);

        // Assert
        resultFolder.Should().NotBeNull();
        resultFolder!.Name.Should().Be(folderName);
        resultFolder.ParentFolderId.Should().Be(folder.ParentFolderId);
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
        var folder = new Folder
        {
            FolderId = Guid.NewGuid(),
            Name = folderName,
            ParentFolderId = Guid.NewGuid()
        };

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(folder.ParentFolderId.Value))
            .ReturnsAsync((Folder)null!);

        // Act
        var act = async () => { await _folderService.CreateFolderAsync(folderName, folder.ParentFolderId); };

        // Assert
        await act.Should().ThrowAsync<Exception>("Parent folder not found");
        _mockFolderRepository.Verify(repo => repo.AddAsync(It.IsAny<Folder>()), Times.Never);
    }

    [Fact]
    public async Task CreateFolderAsync_WithDuplicateName_ThrowsError()
    {
        // Arrange
        const string folderName = "Test Folder";
        var folder = new Folder
        {
            FolderId = Guid.NewGuid(),
            Name = folderName,
            ParentFolderId = Guid.NewGuid()
        };

        _mockFolderRepository.Setup(repo => repo.GetFolderByIdAsync(folder.ParentFolderId.Value)).ReturnsAsync(
            new Folder
            {
                Name = "parent folder",
                ParentFolderId = Guid.NewGuid(),
                Subfolders = new List<Folder> { new() { Name = folderName } }
            });

        // Act
        var act = async () => { await _folderService.CreateFolderAsync(folderName, folder.ParentFolderId); };

        // Assert
        await act.Should().ThrowAsync<Exception>("Folder already exists in parent folder");
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
        folder!.Name.Should().Be(folderName);
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
    public async Task GetFolderTree_ReturnsOnlyOneFolderAtTopLevel()
    {
        // Arrange
        var folders = new List<Folder>
        {
            new() { Name = "Folder 1" },
            new() { Name = "Folder 2" }
        };

        _mockFolderRepository.Setup(repo => repo.GetFolderTreeAsync()).ReturnsAsync(folders);

        // Act
        var result = await _folderService.GetFolderTreeAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Folder 1");
    }

    [Fact]
    public async Task GetFolderTree_ReturnsNull_IfNoFolders()
    {
        // Arrange
        _mockFolderRepository.Setup(repo => repo.GetFolderTreeAsync()).ReturnsAsync(new List<Folder>());

        // Act
        var result = await _folderService.GetFolderTreeAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFolderTree_ReturnsNestedFolders()
    {
        // Arrange
        var folders = new List<Folder>
        {
            new()
            {
                Name = "Folder 1",
                Subfolders = new List<Folder>
                {
                    new() { Name = "Folder 1.1" },
                    new() { Name = "Folder 1.2" }
                }
            }
        };

        _mockFolderRepository.Setup(repo => repo.GetFolderTreeAsync()).ReturnsAsync(folders);

        // Act
        var result = await _folderService.GetFolderTreeAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Folder 1");
        result.Subfolders.Should().HaveCount(2);
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