using FileFolderExplorer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController(IFileService fileService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] Guid folderId)
    {
        var uploadedFile = await fileService.UploadFileAsync(file, folderId);
        return Ok(uploadedFile);
    }
    
    [HttpGet("folder/{folderId:guid}")]
    public async Task<ActionResult<IEnumerable<File>>> GetFilesByFolderId(Guid folderId)
    {
        var files = await fileService.GetFilesByFolderIdAsync(folderId);
        return Ok(files);
    }
    
    [HttpGet("{fileId:guid}")]
    public async Task<ActionResult<File>> GetFileById(Guid fileId)
    {
        var file = await fileService.GetFileByIdAsync(fileId);
        if (file == null)
        {
            return NotFound();
        }
        return Ok(file);
    }
}