using System;
using System.Text.RegularExpressions;

namespace HC.BlobStoring;

/// <summary>
/// Helper class để parse blob name và xác định tenant/host
/// </summary>
public static class HCBlobNameHelper
{
    private static readonly Regex TenantBlobPattern = new Regex(@"^tenants/([^/]+)/(.+)$", RegexOptions.Compiled);
    private static readonly Regex HostBlobPattern = new Regex(@"^host/(.+)$", RegexOptions.Compiled);

    /// <summary>
    /// Parse blob name để lấy thông tin tenant/host
    /// </summary>
    /// <param name="fullBlobName">Full blob name từ MinIO (ví dụ: tenants/123/file.pdf hoặc host/file.pdf)</param>
    /// <returns>Thông tin về tenant/host và blob name thực tế</returns>
    public static BlobNameInfo ParseBlobName(string fullBlobName)
    {
        if (string.IsNullOrWhiteSpace(fullBlobName))
        {
            return new BlobNameInfo
            {
                IsHost = true,
                TenantId = null,
                OriginalBlobName = fullBlobName
            };
        }

        // Kiểm tra pattern tenant: tenants/{tenant-id}/{blob-name}
        var tenantMatch = TenantBlobPattern.Match(fullBlobName);
        if (tenantMatch.Success)
        {
            return new BlobNameInfo
            {
                IsHost = false,
                TenantId = Guid.Parse(tenantMatch.Groups[1].Value),
                OriginalBlobName = tenantMatch.Groups[2].Value,
                FullBlobName = fullBlobName
            };
        }

        // Kiểm tra pattern host: host/{blob-name}
        var hostMatch = HostBlobPattern.Match(fullBlobName);
        if (hostMatch.Success)
        {
            return new BlobNameInfo
            {
                IsHost = true,
                TenantId = null,
                OriginalBlobName = hostMatch.Groups[1].Value,
                FullBlobName = fullBlobName
            };
        }

        // Nếu không match pattern nào, coi như host với tên gốc
        return new BlobNameInfo
        {
            IsHost = true,
            TenantId = null,
            OriginalBlobName = fullBlobName,
            FullBlobName = fullBlobName
        };
    }

    /// <summary>
    /// Kiểm tra blob name có phải của tenant không
    /// </summary>
    public static bool IsTenantBlob(string fullBlobName)
    {
        return TenantBlobPattern.IsMatch(fullBlobName);
    }

    /// <summary>
    /// Kiểm tra blob name có phải của host không
    /// </summary>
    public static bool IsHostBlob(string fullBlobName)
    {
        return HostBlobPattern.IsMatch(fullBlobName);
    }

    /// <summary>
    /// Lấy tenant ID từ blob name
    /// </summary>
    public static Guid? GetTenantId(string fullBlobName)
    {
        var info = ParseBlobName(fullBlobName);
        return info.TenantId;
    }
}

/// <summary>
/// Thông tin về blob name sau khi parse
/// </summary>
public class BlobNameInfo
{
    /// <summary>
    /// Có phải blob của host không
    /// </summary>
    public bool IsHost { get; set; }

    /// <summary>
    /// Tenant ID (null nếu là host blob)
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Tên blob gốc (không có prefix)
    /// </summary>
    public string OriginalBlobName { get; set; } = string.Empty;

    /// <summary>
    /// Full blob name (có prefix)
    /// </summary>
    public string FullBlobName { get; set; } = string.Empty;
}
