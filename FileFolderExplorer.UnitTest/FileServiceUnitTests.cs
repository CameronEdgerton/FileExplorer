using System.Net;
using FileFolderExplorer.Models;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.UnitTest;

public class FileServiceUnitTests
{
    private readonly Mock<IFileRepository> _mockFileRepository;
    private readonly FileService _fileService;

    public FileServiceUnitTests()
    {
        _mockFileRepository = new Mock<IFileRepository>();
        _fileService = new FileService(_mockFileRepository.Object);
    }
    
    [Theory]
    [InlineData(".csv")]
    [InlineData(".geojson")]
    public async Task UploadFileAsync_WithValidFile_ReturnsFile(string fileType)
    {
        // Arrange
        var fileName = $"{Guid.NewGuid().ToString()}{fileType}";
        var folderId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        await writer.WriteAsync("test");
        await writer.FlushAsync();
        ms.Position = 0;
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.ContentType).Returns("multipart/form-data");

        _mockFileRepository
            .Setup(repo => repo.AddAsync(It.IsAny<File>()))
            .Returns(Task.CompletedTask);

        // Act
        var file = await _fileService.UploadFileAsync(fileMock.Object, folderId);

        // Assert
        file.Should().NotBeNull();
        file.Name.Should().Be(fileName);
        file.FileType.Should().Be(fileType);
        file.FolderId.Should().Be(folderId);
        file.FormFile.Length.Should().Be(ms.Length);
        _mockFileRepository.Verify(repo => repo.AddAsync(It.IsAny<File>()), Times.Once);
    }
    
    [Fact]
    public async Task UploadFileAsync_WithInvalidFileType_ThrowsError()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        await writer.WriteAsync("test");
        await writer.FlushAsync();
        ms.Position = 0;
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns("test.txt");
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.ContentType).Returns("multipart/form-data");

        // Act
        var act = async() =>
        {
            await _fileService.UploadFileAsync(fileMock.Object, folderId);
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("Invalid file type");
        _mockFileRepository.Verify(repo => repo.AddAsync(It.IsAny<File>()), Times.Never);
    }
    
    [Fact]
    public async Task UploadFileAsync_WithEmptyFile_ThrowsError()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        // Act
        var act = async() =>
        {
            await _fileService.UploadFileAsync(fileMock.Object, folderId);
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("No file uploaded");
        _mockFileRepository.Verify(repo => repo.AddAsync(It.IsAny<File>()), Times.Never);
    }

   
}