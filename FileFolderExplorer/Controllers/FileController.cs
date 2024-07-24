using FileFolderExplorer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileFolderExplorer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController(IFileService service) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] Guid folderId)
    {
        var uploadedFile = await service.UploadFileAsync(file, folderId);
        return Ok(uploadedFile);
    }
}