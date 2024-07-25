using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.UnitTest;

public class FileServiceUnitTests
{
    private readonly Mock<IFileRepository> _mockFileRepository;
    private readonly FileService _fileService;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly string _storagePath;

    public FileServiceUnitTests()
    {
        _mockFileRepository = new Mock<IFileRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _storagePath = Path.GetTempPath();
        _configurationMock.Setup(config => config["StoragePath"]).Returns(_storagePath);
        _fileService = new FileService(_mockFileRepository.Object, _configurationMock.Object);
    }
    
    [Theory]
    [InlineData(".csv")]
    [InlineData(".geojson")]
    public async Task UploadFileAsync_WithValidFile_ReturnsFile(string extension)
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var fileName = $"test{extension}";
        const string fileContent = "test content";
        var formFile = new FormFile(
            new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent)),
            0, fileContent.Length, fileName, fileName);

        // Act
        var file = await _fileService.UploadFileAsync(formFile, folderId);

        // Assert
        file.Should().NotBeNull();
        file.Name.Should().Be(fileName);
        file.FolderId.Should().Be(folderId);

        // Verify the file was saved to the file system
        var filePath = Path.Combine(_storagePath, fileName);
        System.IO.File.Exists(filePath).Should().BeTrue();
        (await System.IO.File.ReadAllTextAsync(filePath)).Should().Be(fileContent);

        // Verify the file metadata was saved to the repository
        _mockFileRepository.Verify(r => r.AddAsync(It.Is<File>(f =>
            f.Name == fileName &&
            f.FolderId == folderId &&
            f.FilePath == filePath)), Times.Once);
    }
    
    [Fact]
    public async Task UploadFileAsync_WithInvalidFileType_ThrowsError()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        const string fileName = "test.txt";
        const string fileContent = "test content";
        var formFile = new FormFile(
            new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent)),
            0, fileContent.Length, fileName, fileName);

        // Act
        var act = async() =>
        {
            await _fileService.UploadFileAsync(formFile, folderId);
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
        var formFile = new FormFile(
            new MemoryStream(System.Text.Encoding.UTF8.GetBytes(string.Empty)),
            0, 0, string.Empty, string.Empty);

        // Act
        var act = async() =>
        {
            await _fileService.UploadFileAsync(formFile, folderId);
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("No file uploaded");
        _mockFileRepository.Verify(repo => repo.AddAsync(It.IsAny<File>()), Times.Never);
    }

   
}