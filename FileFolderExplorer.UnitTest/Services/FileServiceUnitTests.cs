using System.Text;
using FileFolderExplorer.Repositories.Interfaces;
using FileFolderExplorer.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.UnitTest.Services;

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

        _mockFileRepository.Setup(repo => repo.UploadFileAsync(It.IsAny<File>())).Returns(Task.CompletedTask);
        _mockFolderRepository.Setup(repo => repo.FolderExistsById(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var file = await _fileService.UploadFileAsync(formFile, folderId);

        // Assert
        file.Should().NotBeNull();
        file.Name.Should().Be(fileName);
        file.FolderId.Should().Be(folderId);
        file.Content.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(fileContent));


        // Verify the file was saved to the repository
        _mockFileRepository.Verify(repo => repo.UploadFileAsync(It.IsAny<File>()), Times.Once);
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

        _mockFileRepository.Setup(repo => repo.UploadFileAsync(It.IsAny<File>())).Returns(Task.CompletedTask);
        _mockFolderRepository.Setup(repo => repo.FolderExistsById(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var act = async () => { await _fileService.UploadFileAsync(formFile, folderId); };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("Invalid file type");
        _mockFileRepository.Verify(repo => repo.UploadFileAsync(It.IsAny<File>()), Times.Never);
    }

    [Fact]
    public async Task UploadFileAsync_WithNonExistingFolder_ThrowsError()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        const string fileName = "test.csv";
        const string fileContent = "test content";
        var formFile = new FormFile(
            new MemoryStream(Encoding.UTF8.GetBytes(fileContent)),
            0, fileContent.Length, fileName, fileName);

        _mockFileRepository.Setup(repo => repo.UploadFileAsync(It.IsAny<File>())).Returns(Task.CompletedTask);
        // Return false to simulate a non-existing folder
        _mockFolderRepository.Setup(repo => repo.FolderExistsById(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var act = async () => { await _fileService.UploadFileAsync(formFile, folderId); };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("Folder not found");
        _mockFileRepository.Verify(repo => repo.UploadFileAsync(It.IsAny<File>()), Times.Never);
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
        _mockFileRepository.Verify(repo => repo.UploadFileAsync(It.IsAny<File>()), Times.Never);
    }

    [Fact]
    public async Task GetFileContentByIdAsync_WithExistingFile_ReturnsContent()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        const string fileContent = "test content";
        var file = new File
        {
            FileId = fileId,
            Name = "test.csv",
            Content = Encoding.UTF8.GetBytes(fileContent),
            FolderId = Guid.NewGuid()
        };

        _mockFileRepository.Setup(repo => repo.GetFileByIdAsync(It.IsAny<Guid>())).ReturnsAsync(file);

        // Act
        var content = await _fileService.GetFileContentByIdAsync(fileId);

        // Assert
        content.Should().Be(fileContent);
    }

    [Fact]
    public async Task GetFileContentByIdAsync_WithNonExistingFile_ReturnsEmptyString()
    {
        // Arrange
        var fileId = Guid.NewGuid();

        _mockFileRepository.Setup(repo => repo.GetFileByIdAsync(It.IsAny<Guid>())).ReturnsAsync((File)null!);

        // Act
        var content = await _fileService.GetFileContentByIdAsync(fileId);

        // Assert
        content.Should().BeEmpty();
    }
}