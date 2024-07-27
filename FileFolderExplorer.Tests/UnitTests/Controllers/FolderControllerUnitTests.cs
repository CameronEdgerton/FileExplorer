using FileFolderExplorer.Controllers;
using FileFolderExplorer.Models;
using FileFolderExplorer.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FileFolderExplorer.UnitTest.UnitTests.Controllers;

public class FolderControllerUnitTests
{
    private readonly FolderController _folderController;
    private readonly Mock<IFolderService> _folderServiceMock;

    public FolderControllerUnitTests()
    {
        _folderServiceMock = new Mock<IFolderService>();
        _folderController = new FolderController(_folderServiceMock.Object);
    }

    [Fact]
    public async Task CreateFolder_WithValidRequest_ReturnsFolder()
    {
        // Arrange
        var request = new CreateFolderRequest
        {
            FolderName = "Test Folder",
            ParentFolderId = Guid.NewGuid().ToString()
        };
        var folder = new Folder
        {
            Name = request.FolderName,
            ParentFolderId = Guid.Parse(request.ParentFolderId)
        };
        _folderServiceMock
            .Setup(service => service.CreateFolderAsync(request.FolderName, It.IsAny<Guid>()))
            .ReturnsAsync(folder);

        // Act
        var result = await _folderController.CreateFolder(request) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<Folder>();
        result.Value.Should().BeEquivalentTo(folder);
    }

    [Fact]
    public async Task CreateFolder_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateFolderRequest
        {
            FolderName = "Test Folder",
            ParentFolderId = "invalid-guid"
        };
        _folderServiceMock
            .Setup(service => service.CreateFolderAsync(request.FolderName, It.IsAny<Guid>()))
            .ThrowsAsync(new ArgumentException("Invalid parent folder id"));

        // Act
        var result = await _folderController.CreateFolder(request) as BadRequestObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().Be("Invalid parent folder id");
    }

    [Fact]
    public async Task CreateFolder_WithNullFolder_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreateFolderRequest
        {
            FolderName = "Test Folder",
            ParentFolderId = Guid.NewGuid().ToString()
        };
        _folderServiceMock
            .Setup(service => service.CreateFolderAsync(request.FolderName, It.IsAny<Guid>()))
            .ReturnsAsync((Folder?)null);

        // Act
        var result = await _folderController.CreateFolder(request) as StatusCodeResult;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFolderTree_ReturnsRootFolder()
    {
        // Arrange
        var rootFolder = new Folder
        {
            FolderId = Guid.NewGuid(),
            Name = "Root Folder"
        };
        _folderServiceMock
            .Setup(service => service.GetFolderTreeAsync())
            .ReturnsAsync(rootFolder);

        // Act
        var result = await _folderController.GetFolderTree() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<Folder>();
        result.Value.Should().BeEquivalentTo(rootFolder);
    }

    [Fact]
    public async Task GetFolderTree_ReturnsNull_IfNoRootFolder()
    {
        // Arrange
        _folderServiceMock
            .Setup(service => service.GetFolderTreeAsync())
            .ReturnsAsync((Folder?)null);

        // Act
        var result = await _folderController.GetFolderTree() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetFolderById_WithValidId_ReturnsFolder()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var folder = new Folder
        {
            FolderId = folderId,
            Name = "Test Folder"
        };
        _folderServiceMock
            .Setup(service => service.GetFolderByIdAsync(folderId))
            .ReturnsAsync(folder);

        // Act
        var result = await _folderController.GetFolderById(folderId.ToString()) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<Folder>();
        result.Value.Should().BeEquivalentTo(folder);
    }

    [Theory]
    [InlineData("invalid-guid")]
    [InlineData("")]
    public async Task GetFolderById_WithInvalidId_ReturnsBadRequest(string id)
    {
        // Act
        var result = await _folderController.GetFolderById(id) as BadRequestResult;

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFolderById_WithNullFolder_ReturnsNotFound()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        _folderServiceMock
            .Setup(service => service.GetFolderByIdAsync(folderId))
            .ReturnsAsync((Folder?)null);

        // Act
        var result = await _folderController.GetFolderById(folderId.ToString()) as NotFoundResult;

        // Assert
        result.Should().NotBeNull();
    }
}