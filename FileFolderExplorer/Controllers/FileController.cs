using FileFolderExplorer.Dtos;
using FileFolderExplorer.Services.Interfaces;
using FileFolderExplorer.Utils;
using Microsoft.AspNetCore.Mvc;
using File = FileFolderExplorer.Models.File;

namespace FileFolderExplorer.Controllers;

[Route("api/files")]
[ApiController]
public class FileController(IFileService fileService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest fileRequest)
    {
        var folderIdGuid = GuidHelper.TryParse(fileRequest.FolderId);
        if (folderIdGuid == null || folderIdGuid == Guid.Empty) return BadRequest();
        File uploadedFile;

        try
        {
            uploadedFile = await fileService.UploadFileAsync(fileRequest.File, folderIdGuid.Value);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }

        return Ok(uploadedFile);
    }

    [HttpGet("{fileId}")]
    public async Task<ActionResult> GetFileContentById(string fileId)
    {
        var fileIdGuid = GuidHelper.TryParse(fileId);
        if (fileIdGuid == null) return BadRequest();

        var content = await fileService.GetFileContentByIdAsync(fileIdGuid.Value);
        if (string.IsNullOrEmpty(content)) return NotFound();
        return Ok(content);
    }
}