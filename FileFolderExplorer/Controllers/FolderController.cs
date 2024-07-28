using FileFolderExplorer.Dtos;
using FileFolderExplorer.Models;
using FileFolderExplorer.Services.Interfaces;
using FileFolderExplorer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FileFolderExplorer.Controllers;

[Route("api/folders")]
[ApiController]
public class FolderController(IFolderService folderService) : ControllerBase
{
    /**
     * @summary Creates a folder
     */
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

    /**
     * @summary Gets the folder tree. This is the root folder and all its subfolders.
     */
    [HttpGet("tree")]
    public async Task<IActionResult> GetFolderTree()
    {
        var folderTree = await folderService.GetFolderTreeAsync();
        return Ok(folderTree);
    }

    /**
     * @summary Gets a folder by its ID
     */
    [HttpGet("{folderId}")]
    public async Task<ActionResult> GetFolderById(string folderId)
    {
        var parsedFolderId = GuidHelper.TryParse(folderId);
        if (parsedFolderId == null || parsedFolderId == Guid.Empty) return BadRequest();

        var folder = await folderService.GetFolderByIdAsync(parsedFolderId.Value);
        if (folder == null)
            return NotFound();

        return Ok(folder);
    }
}