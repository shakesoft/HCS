# Docker Compose Commands - Update Single Service

## Dừng và Update một Service cụ thể

### 1. Dừng một service
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml stop <service-name>
```

Ví dụ:
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml stop hc-api
```

### 2. Pull image mới (nếu cần)
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull <service-name>
```

Ví dụ:
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull hc-api
```

### 3. Xóa container cũ và tạo lại với image mới
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps <service-name>
```

Ví dụ:
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps hc-api
```

**Giải thích các flags:**
- `-d`: Chạy ở detached mode (background)
- `--force-recreate`: Tạo lại container ngay cả khi config không đổi
- `--no-deps`: Không khởi động các service phụ thuộc

### 4. Hoặc dùng lệnh kết hợp (Recommended)
```bash
# Dừng, pull image mới, và restart service
docker-compose -f docker-compose-dev-port10.65.37.105.yml stop <service-name> && \
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull <service-name> && \
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps <service-name>
```

## Các lệnh hữu ích khác

### Xem logs của một service
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml logs -f <service-name>
```

### Xem trạng thái các services
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml ps
```

### Restart một service (không pull image mới)
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml restart <service-name>
```

### Xóa container và volumes của một service
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml rm -f -v <service-name>
```

## Ví dụ thực tế

### Update hc-api service
```bash
cd /path/to/etc/docker-compose
docker-compose -f docker-compose-dev-port10.65.37.105.yml stop hc-api
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull hc-api
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps hc-api
```

### Update hc-blazor service
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml stop hc-blazor
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull hc-blazor
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps hc-blazor
```

### Update hc-authserver service
```bash
docker-compose -f docker-compose-dev-port10.65.37.105.yml stop hc-authserver
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull hc-authserver
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps hc-authserver
```

## Lưu ý

1. **Dependencies**: Nếu service bạn update có dependencies (như `hc-blazor` depends on `hc-api`), bạn có thể cần restart cả service phụ thuộc:
   ```bash
   docker-compose -f docker-compose-dev-port10.65.37.105.yml restart hc-blazor
   ```

2. **Health checks**: Một số services có health checks, đợi chúng healthy trước khi tiếp tục:
   ```bash
   docker-compose -f docker-compose-dev-port10.65.37.105.yml ps
   ```

3. **Rollback**: Nếu có vấn đề, có thể rollback bằng cách pull lại version cũ:
   ```bash
   docker pull longnguyen1331/hc-api:previous-version
   docker tag longnguyen1331/hc-api:previous-version longnguyen1331/hc-api:latest
   docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate --no-deps hc-api
   ```

