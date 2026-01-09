# Sửa lỗi MinIO EndPoint Configuration

## ❌ Lỗi gặp phải

```
Minio.Exceptions.InvalidEndpointException: http://localhost:9000 is invalid hostname
```

## 🔍 Nguyên nhân

MinIO .NET client yêu cầu format EndPoint là `hostname:port` (không có `http://` hoặc `https://`). Protocol được xác định bởi property `WithSSL`.

**Sai:**
```csharp
minio.EndPoint = "http://localhost:9000";  // ❌ Lỗi
```

**Đúng:**
```csharp
minio.EndPoint = "localhost:9000";  // ✅ Đúng
minio.WithSSL = false;  // false = http, true = https
```

## ✅ Đã sửa

### 1. appsettings.json
- ✅ `HC.Blazor/appsettings.json`: `"EndPoint": "localhost:9000"`
- ✅ `HC.HttpApi.Host/appsettings.json`: `"EndPoint": "localhost:9000"`
- ✅ `HC.Blazor/appsettings.Production.json`: `"EndPoint": "minio:9000"`

### 2. Code Configuration
- ✅ `HCBlazorModule.cs`: Default `"minio:9000"` (cho Docker)
- ✅ `HCHttpApiHostModule.cs`: Default `"minio:9000"` (cho Docker)

### 3. Docker Compose
- ✅ `docker-compose-dev-port10.65.37.105.yml`: `MinIO__EndPoint=minio:9000`

## 📝 Format EndPoint đúng

### Local Development
```json
{
  "MinIO": {
    "EndPoint": "localhost:9000",  // Không có http://
    "WithSSL": false
  }
}
```

### Docker/Production
```json
{
  "MinIO": {
    "EndPoint": "minio:9000",  // Container name trong Docker network
    "WithSSL": false
  }
}
```

### Với HTTPS
```json
{
  "MinIO": {
    "EndPoint": "minio.example.com:9000",
    "WithSSL": true  // Sử dụng https
  }
}
```

## 🔄 Các bước tiếp theo

1. **Restart application** để áp dụng cấu hình mới
2. **Test upload file** để xác nhận đã hoạt động
3. **Kiểm tra MinIO Console** để xem file đã được upload

## ✅ Kết quả mong đợi

Sau khi sửa, upload file sẽ thành công và file sẽ xuất hiện trong MinIO bucket `hcs_bucket`.
