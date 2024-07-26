using System.Text;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.UnitTest.UnitTests;

public class FileServiceUnitTests
{
    private readonly FileService _fileService;
    private readonly Mock<IFileRepository> _mockFileRepository;
    private readonly Mock<IFolderRepository> _mockFolderRepository;

    public FileServiceUnitTests()
    {
        _mockFileRepository = new Mock<IFileRepository>();
        _mockFolderRepository = new Mock<IFolderRepository>();
        _fileService = new FileService(_mockFileRepository.Object, _mockFolderRepository.Object);
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
            new MemoryStream(Encoding.UTF8.GetBytes(fileContent)),
            0, fileContent.Length, fileName, fileName);

        // Act
        var file = await _fileService.UploadFileAsync(formFile, folderId);

        // Assert
        file.Should().NotBeNull();
        file.Name.Should().Be(fileName);
        file.FolderId.Should().Be(folderId);
        file.Content.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(fileContent));


        // Verify the file was saved to the repository
        _mockFileRepository.Verify(repo => repo.AddAsync(It.IsAny<File>()), Times.Once);
    }

    [Fact]
    public async Task UploadFileAsync_WithInvalidFileType_ThrowsError()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        const string fileName = "test.txt";
        const string fileContent = "test content";
        var formFile = new FormFile(
            new MemoryStream(Encoding.UTF8.GetBytes(fileContent)),
            0, fileContent.Length, fileName, fileName);

        _mockFileRepository.Setup(repo => repo.AddAsync(It.IsAny<File>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => { await _fileService.UploadFileAsync(formFile, folderId); };

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
            new MemoryStream(Encoding.UTF8.GetBytes(string.Empty)),
            0, 0, string.Empty, string.Empty);

        // Act
        var act = async () => { await _fileService.UploadFileAsync(formFile, folderId); };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("File is empty");
        _mockFileRepository.Verify(repo => repo.AddAsync(It.IsAny<File>()), Times.Never);
    }

    [Fact]
    public async Task GetFilesByFolderIdAsync_ShouldReturnFiles()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var files = new List<File>
        {
            new() { FileId = Guid.NewGuid(), Name = "test1.csv", Content = content, FolderId = folderId },
            new() { FileId = Guid.NewGuid(), Name = "test2.geojson", Content = content, FolderId = folderId }
        };

        _mockFileRepository.Setup(repo => repo.GetFilesByFolderIdAsync(folderId)).ReturnsAsync(files);

        // Act
        var result = (await _fileService.GetFilesByFolderIdAsync(folderId)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(files);
    }

    [Fact]
    public async Task GetFileByIdAsync_ShouldReturnFile()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var file = new File
            { FileId = fileId, Name = "test.csv", Content = [1, 2, 3, 4, 5], FolderId = Guid.NewGuid() };

        _mockFileRepository.Setup(repo => repo.GetFileByIdAsync(fileId)).ReturnsAsync(file);

        // Act
        var result = await _fileService.GetFileByIdAsync(fileId);

        // Assert
        result.Should().Be(file);
    }
}