# MinIO Blob Name Calculator - Hướng dẫn sử dụng

## Lưu ý quan trọng

**ABP Framework đã có sẵn `DefaultMinioBlobNameCalculator`** với logic tự động tổ chức blob name theo tenant/host. 

Theo [tài liệu ABP](https://abp.io/docs/latest/framework/infrastructure/blob-storing/minio):
- `IMinioBlobNameCalculator` được implement bởi `DefaultMinioBlobNameCalculator` mặc định
- Logic tự động: `host/{blob-name}` hoặc `tenants/{tenant-id}/{blob-name}`

**Custom calculator (`HCMinioBlobNameCalculator`) chỉ cần thiết nếu:**
- Muốn thêm logging/tracking
- Muốn customize logic tính toán
- Muốn thêm validation hoặc metadata

**Nếu không cần customize:** Có thể xóa `HCMinioBlobNameCalculator` và comment dòng replace trong `HCDomainModule`. ABP sẽ tự động dùng `DefaultMinioBlobNameCalculator`.

## Cách hoạt động

MinIO Blob Provider tự động tổ chức tên BLOB theo quy tắc (giống DefaultMinioBlobNameCalculator):

### 1. **Host Blob** (không có tenant)
- Pattern: `host/{blob-name}`
- Ví dụ: `host/document.pdf`
- Sử dụng khi: User đăng nhập với host account (không có tenant)

### 2. **Tenant Blob** (có tenant)
- Pattern: `tenants/{tenant-id}/{blob-name}`
- Ví dụ: `tenants/123e4567-e89b-12d3-a456-426614174000/document.pdf`
- Sử dụng khi: User đăng nhập với tenant account

## Cách xác định Host/Tenant từ Blob Name

### Sử dụng Helper Class

```csharp
using HC.BlobStoring;

// Parse blob name
var fullBlobName = "tenants/123e4567-e89b-12d3-a456-426614174000/document.pdf";
var info = HCBlobNameHelper.ParseBlobName(fullBlobName);

// Kiểm tra thông tin
if (info.IsHost)
{
    Console.WriteLine($"Đây là blob của Host: {info.OriginalBlobName}");
}
else
{
    Console.WriteLine($"Đây là blob của Tenant {info.TenantId}: {info.OriginalBlobName}");
}

// Hoặc sử dụng helper methods
bool isTenant = HCBlobNameHelper.IsTenantBlob(fullBlobName);
bool isHost = HCBlobNameHelper.IsHostBlob(fullBlobName);
Guid? tenantId = HCBlobNameHelper.GetTenantId(fullBlobName);
```

### Sử dụng Service

```csharp
using HC.Application.BlobStoring;
using Volo.Abp.Application.Services;

public class MyService : ApplicationService
{
    private readonly HCBlobTrackingService _blobTrackingService;

    public MyService(HCBlobTrackingService blobTrackingService)
    {
        _blobTrackingService = blobTrackingService;
    }

    public async Task ProcessBlob(string fullBlobName)
    {
        // Parse blob name
        var info = _blobTrackingService.ParseBlobName(fullBlobName);
        
        // Kiểm tra blob có thuộc tenant hiện tại không
        bool belongsToCurrentTenant = _blobTrackingService.IsBlobBelongsToCurrentTenant(fullBlobName);
        
        if (belongsToCurrentTenant)
        {
            // Xử lý blob của tenant hiện tại
        }
    }
}
```

## Ví dụ Upload và Download

### Upload Blob

```csharp
using Volo.Abp.BlobStoring;
using Volo.Abp.Application.Services;

public class FileService : ApplicationService
{
    private readonly IBlobContainer _blobContainer;

    public FileService(IBlobContainer blobContainer)
    {
        _blobContainer = blobContainer;
    }

    public async Task<string> UploadFileAsync(string fileName, byte[] content)
    {
        // Tự động thêm prefix dựa trên tenant hiện tại
        // - Nếu có tenant: "tenants/{tenant-id}/{fileName}"
        // - Nếu không có tenant: "host/{fileName}"
        await _blobContainer.SaveAsync(fileName, content);
        
        // Lấy full blob name để lưu vào database nếu cần
        var tenantId = CurrentTenant.Id;
        var fullBlobName = tenantId.HasValue 
            ? $"tenants/{tenantId.Value}/{fileName}"
            : $"host/{fileName}";
            
        return fullBlobName;
    }

    public async Task<byte[]> DownloadFileAsync(string originalFileName)
    {
        // Chỉ cần truyền tên file gốc, MinIO sẽ tự động thêm prefix
        return await _blobContainer.GetAllBytesAsync(originalFileName);
    }
}
```

## Tạo Bucket riêng cho mỗi Tenant (Optional)

Nếu muốn tạo bucket riêng cho mỗi tenant thay vì dùng prefix, có thể cấu hình như sau:

```csharp
Configure<AbpBlobStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseMinio(minio =>
        {
            minio.EndPoint = configuration["MinIO:EndPoint"];
            minio.AccessKey = configuration["MinIO:AccessKey"];
            minio.SecretKey = configuration["MinIO:SecretKey"];
            
            // Dynamic bucket name based on tenant
            var tenantId = CurrentTenant.Id;
            minio.BucketName = tenantId.HasValue 
                ? $"hcs-bucket-tenant-{tenantId.Value}"
                : "hcs-bucket-host";
                
            minio.WithSSL = false;
        });
    });
});
```

**Lưu ý:** Cách này phức tạp hơn và cần quản lý nhiều bucket. Khuyến nghị sử dụng prefix trong cùng 1 bucket.

## Kiểm tra trong MinIO Console

1. Truy cập MinIO Console: `http://localhost:9001`
2. Login với credentials: `hcsadmin` / `hcsadminpassword`
3. Vào bucket `hcs_bucket`
4. Sẽ thấy cấu trúc:
   ```
   hcs_bucket/
   ├── host/
   │   └── file1.pdf
   └── tenants/
       ├── {tenant-id-1}/
       │   └── file2.pdf
       └── {tenant-id-2}/
           └── file3.pdf
   ```

## Best Practices

1. **Luôn sử dụng tên file gốc** khi gọi `SaveAsync()` hoặc `GetAllBytesAsync()`
2. **Lưu full blob name vào database** nếu cần tracking
3. **Sử dụng helper methods** để parse blob name thay vì tự parse
4. **Kiểm tra quyền truy cập** trước khi cho phép download blob của tenant khác
