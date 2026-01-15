using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.BlobStoring;
using Microsoft.Extensions.Logging;

namespace HC.Controllers.BlobFiles;

[RemoteService]
[Area("app")]
[Route("api/app/blob-files")]
public class BlobFileController : AbpController
{
    private readonly IBlobContainer _blobContainer;
    private readonly ILogger<BlobFileController> _logger;

    public BlobFileController(IBlobContainer blobContainer, ILogger<BlobFileController> logger)
    {
        _blobContainer = blobContainer;
        _logger = logger;
    }

    /// <summary>
    /// Get file from blob storage by path
    /// </summary>
    /// <param name="path">File path in blob storage (e.g., "survey-criteria-images/file.png")</param>
    /// <returns>File stream</returns>
    [HttpGet]
    [Route("file")]
    public async Task<IActionResult> GetFileAsync([FromQuery] string path)
    {
        _logger.LogInformation("[BlobFileController] GetFileAsync called with path: {Path}", path);
        
        if (string.IsNullOrEmpty(path))
        {
            _logger.LogWarning("[BlobFileController] Path is empty");
            return BadRequest("Path is required");
        }

        try
        {
            _logger.LogInformation("[BlobFileController] Checking if file exists: {Path}", path);
            
            // Check if file exists
            if (!await _blobContainer.ExistsAsync(path))
            {
                _logger.LogWarning("[BlobFileController] File not found: {Path}", path);
                return NotFound($"File not found: {path}");
            }

            _logger.LogInformation("[BlobFileController] Getting file stream: {Path}", path);
            
            // Get file stream
            var stream = await _blobContainer.GetAsync(path);
            
            // Determine content type from file extension
            var contentType = GetContentType(path);
            
            // Get file name from path
            var fileName = Path.GetFileName(path);
            
            _logger.LogInformation("[BlobFileController] Returning file: {Path}, ContentType: {ContentType}, FileName: {FileName}", 
                path, contentType, fileName);
            
            return File(stream, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BlobFileController] Error retrieving file: {Path}", path);
            return StatusCode(500, $"Error retrieving file: {ex.Message}");
        }
    }

    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}
