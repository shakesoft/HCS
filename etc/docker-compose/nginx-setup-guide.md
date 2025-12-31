# Hướng dẫn cấu hình Nginx cho HC Application

## Tổng quan

Có 2 cách cấu hình Nginx:

1. **Cách 1: Dùng subdomain** (`nginx-hc.conf`) - Cần cấu hình DNS
2. **Cách 2: Dùng port khác nhau** (`nginx-hc-simple.conf`) - Đơn giản hơn, không cần DNS

## Cách 1: Cấu hình với Subdomain (nginx-hc.conf)

### Bước 1: Copy file cấu hình

```bash
sudo cp nginx-hc.conf /etc/nginx/sites-available/hc-app
sudo ln -s /etc/nginx/sites-available/hc-app /etc/nginx/sites-enabled/
```

### Bước 2: Tạo SSL Certificate

```bash
# Tạo thư mục cho certificate
sudo mkdir -p /etc/nginx/ssl

# Tạo self-signed certificate cho IP 10.65.37.105
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout /etc/nginx/ssl/10.65.37.105.key \
  -out /etc/nginx/ssl/10.65.37.105.crt \
  -subj "/CN=10.65.37.105" \
  -addext "subjectAltName=IP:10.65.37.105"
```

### Bước 3: Test và reload Nginx

```bash
# Test cấu hình
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

### Truy cập:
- AuthServer: `https://10.65.37.105` hoặc `https://authserver.10.65.37.105`
- API: `https://api.10.65.37.105`
- Blazor: `https://10.65.37.105` hoặc `https://blazor.10.65.37.105`

## Cách 2: Cấu hình với Port khác nhau (nginx-hc-simple.conf) - Khuyến nghị

### Bước 1: Copy file cấu hình

```bash
sudo cp nginx-hc-simple.conf /etc/nginx/sites-available/hc-app
sudo ln -s /etc/nginx/sites-available/hc-app /etc/nginx/sites-enabled/
```

### Bước 2: Tạo SSL Certificate

```bash
# Tạo thư mục cho certificate
sudo mkdir -p /etc/nginx/ssl

# Tạo self-signed certificate cho IP 10.65.37.105
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout /etc/nginx/ssl/10.65.37.105.key \
  -out /etc/nginx/ssl/10.65.37.105.crt \
  -subj "/CN=10.65.37.105" \
  -addext "subjectAltName=IP:10.65.37.105"
```

### Bước 3: Test và reload Nginx

```bash
# Test cấu hình
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

### Truy cập:
- AuthServer: `https://10.65.37.105:8443`
- API: `https://10.65.37.105:8444`
- Blazor: `https://10.65.37.105:8445`

## Lưu ý quan trọng

1. **Firewall**: Đảm bảo mở các port cần thiết:
   ```bash
   sudo ufw allow 80/tcp
   sudo ufw allow 443/tcp
   sudo ufw allow 8443/tcp  # Nếu dùng cách 2
   sudo ufw allow 8444/tcp  # Nếu dùng cách 2
   sudo ufw allow 8445/tcp  # Nếu dùng cách 2
   ```

2. **Docker ports**: Các container đang chạy trên:
   - AuthServer: `44301`
   - API: `44379`
   - Blazor: `44302`

3. **Certificate**: 
   - Self-signed certificate sẽ có cảnh báo trong browser
   - Để production, nên dùng Let's Encrypt hoặc certificate từ CA

4. **CORS**: Nếu có vấn đề CORS, cần cập nhật `App__CorsOrigins` trong docker-compose để bao gồm domain/port của Nginx

## Troubleshooting

### Kiểm tra logs
```bash
# Nginx logs
sudo tail -f /var/log/nginx/hc-*-error.log

# Docker logs
docker logs hc-authserver
docker logs hc-api
docker logs hc-blazor
```

### Test kết nối
```bash
# Test từ server
curl -k https://localhost:44301
curl -k https://localhost:44379
curl -k https://localhost:44302
```

