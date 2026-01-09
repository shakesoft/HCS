using System;
using Microsoft.Extensions.Logging;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.MultiTenancy;

namespace HC.BlobStoring;

/// <summary>
/// Custom MinIO Blob Name Calculator để theo dõi host/tenant
/// 
/// Lưu ý: ABP đã có sẵn DefaultMinioBlobNameCalculator với logic tương tự.
/// Custom calculator này chỉ cần thiết nếu muốn:
/// - Thêm logging/tracking
/// - Customize logic tính toán blob name
/// - Thêm metadata hoặc validation
/// 
/// Nếu không cần customize, có thể xóa file này và comment dòng replace trong HCDomainModule
/// </summary>
public class HCMinioBlobNameCalculator : IMinioBlobNameCalculator
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<HCMinioBlobNameCalculator> _logger;

    public HCMinioBlobNameCalculator(
        ICurrentTenant currentTenant,
        ILogger<HCMinioBlobNameCalculator> logger)
    {
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public string Calculate(BlobProviderArgs args)
    {
        var blobName = args.BlobName;
        var tenantId = _currentTenant.Id;

        // Logic giống DefaultMinioBlobNameCalculator của ABP
        // Nếu có tenant, prefix với tenants/{tenant-id}
        if (tenantId.HasValue)
        {
            var tenantBlobName = $"tenants/{tenantId.Value}/{blobName}";
            
            // Log để tracking (optional - có thể remove nếu không cần)
            _logger.LogDebug("Tenant {TenantId} upload blob: {BlobName} -> {FullBlobName}", 
                tenantId.Value, blobName, tenantBlobName);
            
            return tenantBlobName;
        }

        // Nếu không có tenant (host), prefix với "host"
        var hostBlobName = $"host/{blobName}";
        
        // Log để tracking (optional)
        _logger.LogDebug("Host upload blob: {BlobName} -> {FullBlobName}", 
            blobName, hostBlobName);
        
        return hostBlobName;
    }
}
