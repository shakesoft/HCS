# Dynamic Claims trong ABP Framework

## 1. Dynamic Claims là gì?

**Dynamic Claims** là tính năng của ABP Framework cho phép **tự động refresh (làm mới) claims của user** trong suốt phiên làm việc mà không cần đăng xuất/đăng nhập lại.

### Claims bao gồm:
- **Permissions** (quyền): VD: `HC.Users.Create`, `HC.Projects.Edit`
- **Roles** (vai trò): VD: `Admin`, `User`
- **User Info**: Email, Name, etc.

## 2. Tại sao cần Dynamic Claims?

### Vấn đề không có Dynamic Claims:
```
1. User đăng nhập → JWT token chứa permissions/roles
2. Admin thay đổi permissions của user
3. User vẫn thấy permissions cũ (từ JWT token)
4. Phải đăng xuất và đăng nhập lại mới có permissions mới
```

### Với Dynamic Claims:
```
1. User đăng nhập → JWT token chứa permissions/roles
2. Admin thay đổi permissions của user
3. ABP tự động gọi /userinfo endpoint để lấy permissions mới
4. User thấy permissions mới ngay lập tức (không cần đăng xuất)
```

## 3. Cách hoạt động

### Flow:
1. **Mỗi request**, ABP kiểm tra xem claims có cần refresh không
2. Nếu cần, gọi **AuthServer's `/userinfo` endpoint** để lấy claims mới
3. Cache claims trong một khoảng thời gian (thường 30 phút)
4. Refresh lại khi cache hết hạn

### Code trong ABP:
```csharp
// Enable dynamic claims
context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
{
    options.IsDynamicClaimsEnabled = true; // Bật dynamic claims
});

// Middleware tự động refresh claims
app.UseDynamicClaims();
```

## 4. Khi nào cần Dynamic Claims?

### ✅ CẦN Dynamic Claims khi:
- **Permissions thay đổi thường xuyên** trong runtime
- **Multi-tenant** với permissions khác nhau theo tenant
- **Real-time permission updates** (không muốn user phải logout/login)
- **Security-critical** applications (cần permissions mới nhất)

### ❌ KHÔNG CẦN Dynamic Claims khi:
- **Permissions ít thay đổi** (chỉ khi user login)
- **Simple applications** không có nhiều permissions
- **Performance-critical** (tránh overhead của API calls)
- **Blazor Server với SignalR** (có thể gây deadlock)

## 5. Vấn đề với Blazor Server + SignalR

### Deadlock Problem:
```
1. Blazor Server giữ SignalR circuit (sync context)
2. Dynamic Claims cố refresh → gọi API (async)
3. API call bị block bởi SignalR sync context
4. → DEADLOCK! 💥
```

### Giải pháp:
- **Skip refresh trong SignalR context** (SafeDynamicClaimsMiddleware)
- **Hoặc disable dynamic claims** nếu không cần thiết

## 6. Có thể Disable Dynamic Claims không?

### ✅ CÓ THỂ DISABLE nếu:
- Permissions không thay đổi trong runtime
- Chấp nhận user phải logout/login để có permissions mới
- Muốn tránh deadlock trong Blazor Server

### Code để Disable:
```csharp
context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
{
    options.IsDynamicClaimsEnabled = false; // Tắt dynamic claims
});

// Và comment/remove:
// app.UseDynamicClaims();
```

### Khi Disable:
- Permissions chỉ được load **một lần khi login** (từ JWT token)
- Không có API call để refresh claims
- **Không có deadlock risk**
- User phải logout/login để có permissions mới

## 7. Recommendation cho dự án HC

### Option 1: Giữ Dynamic Claims (hiện tại)
- ✅ Permissions được update real-time
- ✅ User không cần logout/login
- ⚠️ Có risk deadlock (đã có SafeDynamicClaimsMiddleware để giảm risk)

### Option 2: Disable Dynamic Claims
- ✅ Không có deadlock risk
- ✅ Performance tốt hơn (ít API calls)
- ❌ User phải logout/login để có permissions mới

### Khuyến nghị:
- **Nếu permissions ít thay đổi** → **Disable** để tránh deadlock
- **Nếu cần real-time permissions** → **Giữ** nhưng dùng SafeDynamicClaimsMiddleware
