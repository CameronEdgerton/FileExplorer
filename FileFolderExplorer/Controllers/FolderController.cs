using FileFolderExplorer.Models;
using FileFolderExplorer.Services.Interfaces;
using FileFolderExplorer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FileFolderExplorer.Controllers;

[Route("api/folders")]
[ApiController]
public class FolderController(IFolderService folderService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
    {
        var parentId = GuidHelper.TryParse(request.ParentFolderId);
        var folder = await folderService.CreateFolderAsync(request.FolderName, parentId);
        return Ok(folder);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetFolders()
    {
        var folders = await folderService.GetAllFoldersAsync();
        return Ok(folders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Folder>> GetFolderById(string id)
    {
        var folderId = GuidHelper.TryParse(id);
        if (folderId == null) return BadRequest();

        var folder = await folderService.GetFolderByIdAsync(folderId.Value);
        if (folder == null) return NotFound();

        return Ok(folder);
    }
}