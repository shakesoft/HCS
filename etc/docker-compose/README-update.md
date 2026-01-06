# Hướng dẫn Update và Restart Containers

## Vấn đề
Docker Compose có thể sử dụng images cũ trong cache local thay vì pull images mới từ Docker Hub.

## Giải pháp

### Cách 1: Sử dụng script tự động (Khuyến nghị)
```bash
cd /path/to/etc/docker-compose
./update-and-restart.sh docker-compose-dev-port10.65.37.105.yml
```

### Cách 2: Manual commands
```bash
cd /path/to/etc/docker-compose

# 1. Stop containers
docker-compose -f docker-compose-dev-port10.65.37.105.yml down

# 2. Remove old images (force pull new)
docker rmi longnguyen1331/hc-blazor:latest 2>/dev/null || true
docker rmi longnguyen1331/hc-api:latest 2>/dev/null || true
docker rmi longnguyen1331/hc-authserver:latest 2>/dev/null || true
docker rmi longnguyen1331/hc-db-migrator:latest 2>/dev/null || true

# 3. Pull latest images
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull

# 4. Start containers
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d

# 5. Check status
docker-compose -f docker-compose-dev-port10.65.37.105.yml ps
```

### Cách 3: Pull và recreate trong một lệnh
```bash
cd /path/to/etc/docker-compose

# Pull images mới và recreate containers
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate
```

## Kiểm tra images đã được pull
```bash
# Xem images hiện tại
docker images | grep longnguyen1331

# Xem image details (created time)
docker inspect longnguyen1331/hc-blazor:latest | grep Created
```

## Troubleshooting

### Nếu vẫn dùng images cũ:
```bash
# Xóa tất cả images cũ
docker rmi $(docker images "longnguyen1331/hc-*" -q) 2>/dev/null || true

# Pull lại
docker-compose -f docker-compose-dev-port10.65.37.105.yml pull

# Recreate
docker-compose -f docker-compose-dev-port10.65.37.105.yml up -d --force-recreate
```

### Kiểm tra logs để xác nhận images mới:
```bash
docker logs hc-blazor --tail 20 | grep -i "cookie\|xsrf\|secure"
```

