using System;
using System.Threading.Tasks;
using HC.BlobStoring;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Microsoft.Extensions.Logging;

namespace HC.Application.BlobStoring;

/// <summary>
/// Example service để demo cách sử dụng MinIO với tenant/host tracking
/// </summary>
public class ExampleBlobService : ApplicationService
{
    private readonly IBlobContainer _blobContainer;
    private readonly HCBlobTrackingService _blobTrackingService;

    public ExampleBlobService(
        IBlobContainer blobContainer,
        HCBlobTrackingService blobTrackingService)
    {
        _blobContainer = blobContainer;
        _blobTrackingService = blobTrackingService;
    }

    /// <summary>
    /// Example: Upload file và trả về full blob name
    /// </summary>
    public async Task<string> UploadFileExampleAsync(string fileName, byte[] content)
    {
        // Upload file - MinIO sẽ tự động thêm prefix dựa trên tenant
        await _blobContainer.SaveAsync(fileName, content);

        // Tính toán full blob name để lưu vào database
        var tenantId = CurrentTenant.Id;
        var fullBlobName = tenantId.HasValue
            ? $"tenants/{tenantId.Value}/{fileName}"
            : $"host/{fileName}";

        Logger.LogInformation($"Uploaded file: {fullBlobName}");

        return fullBlobName;
    }

    /// <summary>
    /// Example: Download file từ full blob name
    /// </summary>
    public async Task<byte[]> DownloadFileExampleAsync(string fullBlobName)
    {
        // Parse để lấy original blob name
        var info = _blobTrackingService.ParseBlobName(fullBlobName);

        // Kiểm tra quyền truy cập
        if (!info.IsHost && info.TenantId != CurrentTenant.Id)
        {
            throw new UnauthorizedAccessException("Không có quyền truy cập blob này");
        }

        // Download sử dụng original blob name
        return await _blobContainer.GetAllBytesAsync(info.OriginalBlobName);
    }

    /// <summary>
    /// Example: Xác định host/tenant từ blob name
    /// </summary>
    public BlobNameInfo GetBlobInfoExample(string fullBlobName)
    {
        var info = _blobTrackingService.ParseBlobName(fullBlobName);

        Logger.LogInformation($"Blob Info - IsHost: {info.IsHost}, TenantId: {info.TenantId}, OriginalName: {info.OriginalBlobName}");

        return info;
    }

    /// <summary>
    /// Example: Kiểm tra blob có thuộc tenant hiện tại không
    /// </summary>
    public bool CheckBlobAccessExample(string fullBlobName)
    {
        return _blobTrackingService.IsBlobBelongsToCurrentTenant(fullBlobName);
    }
}
