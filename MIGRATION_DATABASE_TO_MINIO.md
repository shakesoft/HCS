# Hướng dẫn chuyển từ Database Blob Storage sang MinIO

## ✅ Đã hoàn thành

### 1. Gỡ bỏ Database Provider
- ✅ Xóa package `Volo.Abp.BlobStoring.Database.Domain` từ `HC.Domain`
- ✅ Xóa package `Volo.Abp.BlobStoring.Database.Domain.Shared` từ `HC.Domain.Shared`
- ✅ Xóa package `Volo.Abp.BlobStoring.Database.EntityFrameworkCore` từ `HC.EntityFrameworkCore`
- ✅ Xóa `BlobStoringDatabaseDomainModule` từ `HCDomainModule`
- ✅ Xóa `BlobStoringDatabaseDomainSharedModule` từ `HCDomainSharedModule`
- ✅ Xóa `BlobStoringDatabaseEntityFrameworkCoreModule` từ `HCEntityFrameworkCoreModule`

### 2. Đã cấu hình MinIO
- ✅ Package `Volo.Abp.BlobStoring.Minio` đã được thêm
- ✅ Module `AbpBlobStoringMinioModule` đã được đăng ký
- ✅ Cấu hình MinIO trong `HCBlazorModule` và `HCHttpApiHostModule`
- ✅ Cấu hình trong `appsettings.json` và Docker

## 📋 Các bước tiếp theo (nếu cần)

### Bước 1: Backup dữ liệu hiện có (nếu có)

Nếu bạn đã có file được lưu trong database, cần migrate dữ liệu:

```sql
-- Kiểm tra xem có dữ liệu trong bảng DatabaseBlob không
SELECT COUNT(*) FROM "DatabaseBlob";
SELECT COUNT(*) FROM "DatabaseBlobContainer";
```

### Bước 2: Tạo Migration để xóa bảng Database Blob (Optional)

Nếu muốn xóa các bảng `DatabaseBlob` và `DatabaseBlobContainer` khỏi database:

```bash
cd src/HC.EntityFrameworkCore
dotnet ef migrations add Remove_DatabaseBlobStorage --startup-project ../HC.DbMigrator
```

Sau đó tạo migration code để xóa bảng:

```csharp
// Trong file migration mới tạo
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable("DatabaseBlob");
    migrationBuilder.DropTable("DatabaseBlobContainer");
}
```

**Lưu ý:** Chỉ làm bước này nếu:
- Bạn chắc chắn không còn dữ liệu quan trọng trong database
- Hoặc đã migrate tất cả dữ liệu sang MinIO

### Bước 3: Restore packages và build

```bash
dotnet restore
dotnet build
```

### Bước 4: Chạy migration (nếu có)

```bash
cd src/HC.DbMigrator
dotnet run
```

## 🎯 Kiểm tra hoạt động

### 1. Kiểm tra MinIO đang chạy

```bash
# Kiểm tra MinIO container
docker ps | grep minio

# Hoặc truy cập MinIO Console
# http://localhost:9001 hoặc http://10.65.37.105:9001
# Username: hcsadmin
# Password: hcsadminpassword
```

### 2. Test upload file

```csharp
// Trong service hoặc controller
public class TestBlobService : ApplicationService
{
    private readonly IBlobContainer _blobContainer;

    public TestBlobService(IBlobContainer blobContainer)
    {
        _blobContainer = blobContainer;
    }

    public async Task TestUploadAsync()
    {
        var testContent = Encoding.UTF8.GetBytes("Hello MinIO!");
        await _blobContainer.SaveAsync("test-file.txt", testContent);
        
        Logger.LogInformation("File uploaded to MinIO successfully!");
    }
}
```

### 3. Kiểm tra trong MinIO Console

1. Truy cập MinIO Console
2. Vào bucket `hcs_bucket`
3. Kiểm tra có file `host/test-file.txt` hoặc `tenants/{tenant-id}/test-file.txt`

## 📊 So sánh trước và sau

| Trước (Database) | Sau (MinIO) |
|------------------|-------------|
| File lưu trong bảng `DatabaseBlob` | File lưu trong MinIO bucket |
| Database lớn, backup chậm | Database nhỏ, backup nhanh |
| Performance chậm với file lớn | Performance tốt với file lớn |
| Khó scale | Dễ scale |

## ⚠️ Lưu ý quan trọng

1. **Dữ liệu cũ:** Nếu có file đã lưu trong database, cần script migrate dữ liệu sang MinIO trước khi xóa bảng
2. **Backup:** Luôn backup database trước khi chạy migration xóa bảng
3. **MinIO phải chạy:** Đảm bảo MinIO service đang chạy trước khi upload file
4. **Cấu hình:** Kiểm tra lại cấu hình MinIO trong `appsettings.json` và Docker

## 🔄 Rollback (nếu cần)

Nếu cần rollback về Database provider:

1. Thêm lại packages Database
2. Thêm lại DependsOn modules
3. Tạo migration để tạo lại bảng
4. Restore dữ liệu từ backup

## ✅ Kết quả

Sau khi hoàn thành:
- ✅ File sẽ được lưu vào MinIO thay vì database
- ✅ Database nhẹ hơn, backup nhanh hơn
- ✅ Performance tốt hơn với file lớn
- ✅ Dễ scale và maintain
