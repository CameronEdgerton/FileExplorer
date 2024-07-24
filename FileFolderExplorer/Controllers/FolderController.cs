using FileFolderExplorer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileFolderExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FolderController(IFolderService service) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CreateFolder(string name, Guid? parentId)
    {
        var folder = await service.CreateFolderAsync(name, parentId);
        return Ok(folder);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFolders()
    {
        var folders = await service.GetAllFoldersAsync();
        return Ok(folders);
    }
}