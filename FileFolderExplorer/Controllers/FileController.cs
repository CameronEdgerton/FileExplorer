using FileFolderExplorer.Models;
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

    [HttpGet("folder/{folderId}")]
    public async Task<ActionResult<IEnumerable<File>>> GetFilesByFolderId(string folderId)
    {
        var folderIdGuid = GuidHelper.TryParse(folderId);
        if (folderIdGuid == null) return BadRequest();

        var files = await fileService.GetFilesByFolderIdAsync(folderIdGuid.Value);
        return Ok(files);
    }

    [HttpGet("{fileId}")]
    public async Task<ActionResult<File>> GetFileById(string fileId)
    {
        var fileIdGuid = GuidHelper.TryParse(fileId);
        if (fileIdGuid == null) return BadRequest();

        var file = await fileService.GetFileByIdAsync(fileIdGuid.Value);
        if (file == null) return NotFound();
        return Ok(file);
    }
}