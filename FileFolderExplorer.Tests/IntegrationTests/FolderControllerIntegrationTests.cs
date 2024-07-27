using System.Net.Http.Json;
using FileFolderExplorer.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FileFolderExplorer.UnitTest.IntegrationTests;

public class FolderControllerIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string Route = "/api/folders";

    [Fact]
    public async Task GetFolders_ShouldReturnFolders()
    {
        var response = await _client.GetAsync($"{Route}/all");
        response.EnsureSuccessStatusCode();

        var folders = await response.Content.ReadFromJsonAsync<List<Folder>>();
        folders.Should().NotBeNull();
        folders.Should().BeOfType<List<Folder>>();
    }

    /*[Fact]
    public async Task GetFolderById_ShouldReturnFolder()
    {
        var folderId = Guid.NewGuid();
        var response = await _client.GetAsync($"{Route}/{folderId}");
        response.EnsureSuccessStatusCode();

        var folder = await response.Content.ReadFromJsonAsync<Folder>();
        folder.Should().NotBeNull();
        folder.Should().BeOfType<Folder>();
        folder!.FolderId.Should().Be(folderId);
    }

    [Fact]
    public async Task CreateFolder_ShouldCreateFolder()
    {
        var newFolder = new Folder
        {
            Name = "New Folder",
            ParentFolderId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync($"{Route}/", (newFolder.Name, newFolder.ParentFolderId));
        response.EnsureSuccessStatusCode();

        var createdFolder = await response.Content.ReadFromJsonAsync<Folder>();
        createdFolder.Should().NotBeNull();
        createdFolder!.Name.Should().Be(newFolder.Name);
    }*/
}