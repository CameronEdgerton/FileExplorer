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
        var parentFolderId = GuidHelper.TryParse(request.ParentFolderId);
        Folder? folder;
        try
        {
            folder = await folderService.CreateFolderAsync(request.FolderName, parentFolderId);
            if (folder == null) return StatusCode(500, "An error occurred while creating the folder");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }


        return Ok(folder);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllFolders()
    {
        var folders = await folderService.GetAllFoldersAsync();
        return Ok(folders);
    }

    [HttpGet("{folderId}")]
    public async Task<ActionResult<Folder>> GetFolderById(string folderId)
    {
        var parsedFolderId = GuidHelper.TryParse(folderId);
        if (parsedFolderId == null) return BadRequest();

        var folder = await folderService.GetFolderByIdAsync(parsedFolderId.Value);
        if (folder == null)
            return NotFound();
        return Ok(folder);
    }
}