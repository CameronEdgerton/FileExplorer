using FileFolderExplorer.Models;
using FileFolderExplorer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileFolderExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FolderController(IFolderService folderService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CreateFolder(string name, Guid? parentId)
    {
        var folder = await folderService.CreateFolderAsync(name, parentId);
        return Ok(folder);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFolders()
    {
        var folders = await folderService.GetAllFoldersAsync();
        return Ok(folders);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Folder>> GetFolderById(Guid id)
    {
        var folder = await folderService.GetFolderByIdAsync(id);
        if (folder == null)
        {
            return NotFound();
        }
        return Ok(folder);
    }
}