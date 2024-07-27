using FileFolderExplorer.Controllers;
using FileFolderExplorer.Models;
using FileFolderExplorer.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.UnitTest.Controllers;

public class FileControllerUnitTests
{
    private readonly FileController _fileController;
    private readonly Mock<IFileService> _fileServiceMock;

    public FileControllerUnitTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _fileController = new FileController(_fileServiceMock.Object);
    }

    [Fact]
    public async Task UploadFile_WithValidRequest_ReturnsFile()
    {
        // Arrange
        var request = new UploadFileRequest
        {
            FolderId = Guid.NewGuid().ToString(),
            File = new FormFile(null!, 0, 0, "Test File", "test.txt")
        };
        var file = new File
        {
            Name = request.File.FileName,
            FolderId = Guid.Parse(request.FolderId),
            Content = [1, 2, 3]
        };
        _fileServiceMock
            .Setup(service => service.UploadFileAsync(request.File, It.IsAny<Guid>()))
            .ReturnsAsync(file);

        // Act
        var result = await _fileController.UploadFile(request) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<File>();
        result.Value.Should().BeEquivalentTo(file);
    }

    [Theory]
    [InlineData("invalid-Guid")]
    [InlineData("")]
    public async Task UploadFile_WithInvalidFolderId_ReturnsBadRequest(string folderId)
    {
        // Arrange
        var request = new UploadFileRequest
        {
            FolderId = folderId,
            File = new FormFile(null!, 0, 0, "Test File", "test.txt")
        };

        // Act
        var result = await _fileController.UploadFile(request) as BadRequestResult;

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UploadFile_WithInvalidFile_ReturnsBadRequest()
    {
        // Arrange
        var request = new UploadFileRequest
        {
            FolderId = Guid.NewGuid().ToString(),
            File = new FormFile(null!, 0, 0, "Test File", "test.txt")
        };
        _fileServiceMock
            .Setup(service => service.UploadFileAsync(request.File, It.IsAny<Guid>()))
            .ThrowsAsync(new ArgumentException("Invalid file"));

        // Act
        var result = await _fileController.UploadFile(request) as BadRequestObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().Be("Invalid file");
    }
}