# Kế Hoạch Mở Rộng Chat Module

## 1. Tổng Quan

Tài liệu này mô tả phương án mở rộng module Chat từ `chat-samples` sang dự án chính `src` với các tính năng bổ sung.

### 1.1. Hiện Trạng

**Cấu trúc hiện tại trong chat-samples:**
- **Conversation**: Chỉ hỗ trợ chat 1-1 (UserId và TargetUserId)
- **Message**: Chỉ có text, không có reply, pin, hoặc file attachments
- **UserMessage**: Quan hệ giữa user và message
- Không có entity cho file attachments
- Không hỗ trợ group conversations

**Cấu trúc dự án chính (src):**
- Đã tích hợp Chat module từ ABP (Volo.Chat)
- Có trang Chat1.razor đang override UI
- Module đang được comment là "Temporarily disabled Chat feature"

### 1.2. Yêu Cầu Mở Rộng

1. **ChatConversations (ROOM)**
   - Tạo conversation theo các type: DIRECT / GROUPS / PROJECT / TASK
   - Conversation Name
   - Pinned conversations
   - Mở rộng room nhiều member

2. **ChatMessage**
   - Pin message
   - Reply message

3. **ChatFiles**
   - Gửi kèm files trong conversations

---

## 2. Phương Án Thiết Kế

### 2.1. ChatConversations (ROOM) - Mở Rộng

#### 2.1.1. Database Schema Changes

**Thay đổi Entity `Conversation`:**

```csharp
public enum ConversationType
{
    Direct = 1,      // Chat 1-1
    Group = 2,        // Group chat
    Project = 3,      // Project-related chat
    Task = 4          // Task-related chat
}

public class Conversation : Entity<Guid>, IMultiTenant, IAggregateRoot<Guid>
{
    // Existing properties
    public virtual Guid? TenantId { get; protected set; }
    public virtual Guid UserId { get; protected set; }
    public virtual Guid? TargetUserId { get; protected set; } // Nullable for group chats
    
    // New properties
    public virtual ConversationType Type { get; protected set; }
    public virtual string Name { get; protected set; } // For groups/projects/tasks
    public virtual string Description { get; protected set; }
    // Note: IsPinned is per-user, stored in ConversationMember, not here
    public virtual Guid? ProjectId { get; protected set; } // For PROJECT type
    public virtual Guid? TaskId { get; protected set; }   // For TASK type
    
    // Existing properties
    public virtual ChatMessageSide LastMessageSide { get; set; }
    public virtual string LastMessage { get; set; }
    public virtual DateTime LastMessageDate { get; set; }
    public virtual int UnreadMessageCount { get; protected set; }
    
    // Navigation properties
    public virtual ICollection<ConversationMember> Members { get; protected set; }
}
```

**Tạo Entity mới `ConversationMember`:**

```csharp
public class ConversationMember : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }
    public virtual Guid ConversationId { get; protected set; }
    public virtual Guid UserId { get; protected set; }
    public virtual DateTime JoinedDate { get; protected set; }
    public virtual string Role { get; protected set; } //ADMIN / MEMBER For group management
    public virtual bool IsActive { get; protected set; }
    public virtual bool IsPinned { get; protected set; } // Per-user pin status
    public virtual DateTime? PinnedDate { get; protected set; } // When user pinned this conversation
    
    // Navigation
    public virtual Conversation Conversation { get; protected set; }
    
    // Methods
    public virtual void Pin()
    {
        IsPinned = true;
        PinnedDate = DateTime.UtcNow;
    }
    
    public virtual void Unpin()
    {
        IsPinned = false;
        PinnedDate = null;
    }
}
```

#### 2.1.2. Migration Strategy

**Bước 1: Tạo migration cho Conversation changes**
- Thêm các cột mới vào bảng `ChatConversations`
- Set default values cho existing records:
  - `Type = ConversationType.Direct`
  - `Name = null` (sẽ được populate từ user names)

**Bước 2: Tạo bảng mới `ChatConversationMembers`**
- Foreign key đến `ChatConversations`
- Index trên `ConversationId` và `UserId`
- Index trên `(UserId, IsPinned)` để query pinned conversations nhanh
- Unique constraint trên `(ConversationId, UserId)`
- Columns: `IsPinned` (bool, default false), `PinnedDate` (datetime, nullable)

**Bước 3: Migrate existing data**
- Tạo ConversationMember records cho mỗi existing conversation
- Tạo 2 members: UserId và TargetUserId

#### 2.1.3. Repository Changes

**IConversationRepository - Thêm methods:**

```csharp
public interface IConversationRepository : IRepository<Conversation, Guid>
{
    // Existing methods...
    
    // New methods
    Task<List<Conversation>> GetByTypeAsync(
        Guid userId, 
        ConversationType type, 
        bool includePinned = false
    );
    
    Task<Conversation> GetWithMembersAsync(Guid conversationId);
    
    Task<bool> IsUserMemberAsync(Guid conversationId, Guid userId);
    
    Task<List<Conversation>> GetByProjectIdAsync(Guid projectId);
    
    Task<List<Conversation>> GetByTaskIdAsync(Guid taskId);
}
```

**IConversationMemberRepository - Tạo mới:**

```csharp
public interface IConversationMemberRepository : IRepository<ConversationMember, Guid>
{
    Task<List<ConversationMember>> GetByConversationIdAsync(Guid conversationId);
    
    Task<List<ConversationMember>> GetByUserIdAsync(Guid userId);
    
    Task<List<ConversationMember>> GetPinnedByUserIdAsync(Guid userId); // Get user's pinned conversations
    
    Task<ConversationMember> GetByConversationAndUserAsync(
        Guid conversationId, 
        Guid userId
    );
    
    Task<bool> ExistsAsync(Guid conversationId, Guid userId);
    
    Task<bool> IsPinnedAsync(Guid conversationId, Guid userId); // Check if user pinned this conversation
}
```

#### 2.1.4. Pin Conversation Logic (Per-User)

**Vấn đề:** Mỗi user có thể pin/unpin conversations riêng của họ. Do đó, pin status được lưu trong `ConversationMember` entity, không phải trong `Conversation`.

**Cách hoạt động:**
1. Khi user pin một conversation, update `ConversationMember.IsPinned = true` cho user đó
2. Khi query conversations, join với `ConversationMember` để check pin status của current user
3. Mỗi user có danh sách pinned conversations riêng

**Implementation Example:**

```csharp
// In ConversationAppService
public virtual async Task PinConversationAsync(Guid conversationId)
{
    var currentUserId = CurrentUser.GetId();
    
    // Get or create ConversationMember for current user
    var member = await _conversationMemberRepository
        .GetByConversationAndUserAsync(conversationId, currentUserId);
    
    if (member == null)
    {
        throw new BusinessException("User is not a member of this conversation");
    }
    
    member.Pin(); // Sets IsPinned = true, PinnedDate = DateTime.UtcNow
    await _conversationMemberRepository.UpdateAsync(member);
}

public virtual async Task<List<ConversationDto>> GetPinnedConversationsAsync()
{
    var currentUserId = CurrentUser.GetId();
    
    // Get all pinned ConversationMembers for current user
    var pinnedMembers = await _conversationMemberRepository
        .GetPinnedByUserIdAsync(currentUserId);
    
    var conversationIds = pinnedMembers.Select(m => m.ConversationId).ToList();
    
    // Load conversations
    var conversations = await _conversationRepository
        .GetListAsync(c => conversationIds.Contains(c.Id));
    
    // Map to DTOs with IsPinned = true
    return conversations.Select(c => new ConversationDto
    {
        Id = c.Id,
        Type = c.Type,
        Name = c.Name,
        IsPinned = true, // Current user pinned this
        // ... other properties
    }).ToList();
}

// When getting all conversations, include pin status
public virtual async Task<List<ConversationDto>> GetConversationsAsync()
{
    var currentUserId = CurrentUser.GetId();
    
    // Get all conversations for user
    var conversations = await _conversationRepository
        .GetListAsync(/* filter by user */);
    
    // Get all ConversationMembers for current user to check pin status
    var memberMap = (await _conversationMemberRepository
        .GetByUserIdAsync(currentUserId))
        .ToDictionary(m => m.ConversationId, m => m);
    
    return conversations.Select(c => new ConversationDto
    {
        Id = c.Id,
        Type = c.Type,
        Name = c.Name,
        IsPinned = memberMap.ContainsKey(c.Id) && memberMap[c.Id].IsPinned,
        PinnedDate = memberMap.ContainsKey(c.Id) ? memberMap[c.Id].PinnedDate : null,
        // ... other properties
    }).ToList();
}
```

**Database Query Optimization:**

```csharp
// In EfCoreConversationMemberRepository
public async Task<List<ConversationMember>> GetPinnedByUserIdAsync(Guid userId)
{
    return await DbSet
        .Where(m => m.UserId == userId && m.IsPinned)
        .OrderByDescending(m => m.PinnedDate) // Most recently pinned first
        .ToListAsync();
}

// Optimized query to get conversations with pin status
public async Task<List<ConversationWithPinDto>> GetConversationsWithPinStatusAsync(Guid userId)
{
    var query = from c in DbContext.ChatConversations
                join m in DbContext.ChatConversationMembers
                    on new { ConversationId = c.Id, UserId = userId }
                    equals new { ConversationId = m.ConversationId, UserId = m.UserId }
                    into memberJoin
                from member in memberJoin.DefaultIfEmpty()
                select new ConversationWithPinDto
                {
                    Conversation = c,
                    IsPinned = member != null && member.IsPinned,
                    PinnedDate = member != null ? member.PinnedDate : null
                };
    
    return await query.ToListAsync();
}
```

#### 2.1.5. Application Service Changes

**IConversationAppService - Thêm methods:**

```csharp
public interface IConversationAppService
{
    // Existing methods...
    
    // New methods
    Task<ConversationDto> CreateGroupConversationAsync(CreateGroupConversationInput input);
    
    Task<ConversationDto> CreateProjectConversationAsync(CreateProjectConversationInput input);
    
    Task<ConversationDto> CreateTaskConversationAsync(CreateTaskConversationInput input);
    
    Task<ConversationDto> UpdateConversationNameAsync(UpdateConversationNameInput input);
    
    Task PinConversationAsync(Guid conversationId); // Pin for current user
    
    Task UnpinConversationAsync(Guid conversationId); // Unpin for current user
    
    Task AddMemberAsync(AddMemberInput input);
    
    Task RemoveMemberAsync(RemoveMemberInput input);
    
    Task<List<ConversationMemberDto>> GetMembersAsync(Guid conversationId);
    
    Task<List<ConversationDto>> GetPinnedConversationsAsync();
    
    Task<List<ConversationDto>> GetByTypeAsync(ConversationType type);
}
```

**DTOs:**

```csharp
public class CreateGroupConversationInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Guid> MemberUserIds { get; set; }
}

public class CreateProjectConversationInput
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public List<Guid> MemberUserIds { get; set; }
}

public class CreateTaskConversationInput
{
    public Guid TaskId { get; set; }
    public string Name { get; set; }
    public List<Guid> MemberUserIds { get; set; }
}

public class ConversationDto
{
    public Guid Id { get; set; }
    public ConversationType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPinned { get; set; } // Per current user - true if current user pinned this conversation
    public DateTime? PinnedDate { get; set; } // When current user pinned
    public Guid? ProjectId { get; set; }
    public Guid? TaskId { get; set; }
    public int MemberCount { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageDate { get; set; }
    public int UnreadMessageCount { get; set; }
    // For Direct type
    public ChatTargetUserInfo TargetUserInfo { get; set; }
    // For Group/Project/Task types
    public List<ConversationMemberDto> Members { get; set; }
}
```

---

### 2.2. ChatMessage - Pin & Reply

#### 2.2.1. Database Schema Changes

**Thay đổi Entity `Message`:**

```csharp
public class Message : CreationAuditedAggregateRoot<Guid>, IMultiTenant
{
    // Existing properties
    public virtual Guid? TenantId { get; protected set; }
    public virtual string Text { get; protected set; }
    public virtual bool IsAllRead { get; protected set; }
    public virtual DateTime? ReadTime { get; protected set; }
    
    // New properties
    public virtual bool IsPinned { get; protected set; }
    public virtual DateTime? PinnedDate { get; protected set; }
    public virtual Guid? PinnedByUserId { get; protected set; }
    public virtual Guid? ReplyToMessageId { get; protected set; }
    
    // Navigation
    public virtual Message ReplyToMessage { get; protected set; }
    public virtual ICollection<Message> Replies { get; protected set; }
}
```

**Tạo Entity mới `MessageFile` (xem section 2.3)**

#### 2.2.2. Repository Changes

**IMessageRepository - Thêm methods:**

```csharp
public interface IMessageRepository : IRepository<Message, Guid>
{
    // Existing methods...
    
    // New methods
    Task<List<Message>> GetPinnedMessagesAsync(Guid conversationId);
    
    Task<Message> GetWithReplyAsync(Guid messageId);
    
    Task<List<Message>> GetRepliesAsync(Guid messageId);
}
```

#### 2.2.3. Application Service Changes

**IConversationAppService - Thêm methods:**

```csharp
public interface IConversationAppService
{
    // Existing methods...
    
    // New methods for messages
    Task<ChatMessageDto> SendReplyMessageAsync(SendReplyMessageInput input);
    
    Task PinMessageAsync(Guid messageId);
    
    Task UnpinMessageAsync(Guid messageId);
    
    Task<List<ChatMessageDto>> GetPinnedMessagesAsync(Guid conversationId);
}
```

**DTOs:**

```csharp
public class SendReplyMessageInput
{
    public Guid TargetUserId { get; set; } // For Direct
    public Guid? ConversationId { get; set; } // For Group/Project/Task
    public Guid ReplyToMessageId { get; set; }
    public string Message { get; set; }
}

public class ChatMessageDto
{
    // Existing properties
    public Guid Id { get; set; }
    public string Message { get; set; }
    public DateTime MessageDate { get; set; }
    public bool IsRead { get; set; }
    public DateTime ReadDate { get; set; }
    public ChatMessageSide Side { get; set; }
    
    // New properties
    public bool IsPinned { get; set; }
    public DateTime? PinnedDate { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public ChatMessageDto ReplyToMessage { get; set; } // Nested reply info
    public List<MessageFileDto> Files { get; set; }
}
```

---

### 2.3. ChatFiles - File Attachments

#### 2.3.1. Database Schema

**Tạo Entity mới `MessageFile`:**

```csharp
public class MessageFile : Entity<Guid>, IMultiTenant, ICreationAudited
{
    public virtual Guid? TenantId { get; protected set; }
    public virtual Guid MessageId { get; protected set; }
    public virtual string FileName { get; protected set; }
    public virtual string FilePath { get; protected set; } // Blob storage path
    public virtual string ContentType { get; protected set; }
    public virtual long FileSize { get; protected set; }
    public virtual string FileExtension { get; protected set; }
    public virtual DateTime CreationTime { get; protected set; }
    public virtual Guid? CreatorId { get; protected set; }
    
    // Navigation
    public virtual Message Message { get; protected set; }
}
```

#### 2.3.2. Repository

**IMessageFileRepository:**

```csharp
public interface IMessageFileRepository : IRepository<MessageFile, Guid>
{
    Task<List<MessageFile>> GetByMessageIdAsync(Guid messageId);
    
    Task<List<MessageFile>> GetByConversationIdAsync(Guid conversationId);
    
    Task<MessageFile> GetWithMessageAsync(Guid fileId);
}
```

#### 2.3.3. Application Service

**IConversationAppService - Thêm methods:**

```csharp
public interface IConversationAppService
{
    // Existing methods...
    
    // New methods for files
    Task<ChatMessageDto> SendMessageWithFilesAsync(SendMessageWithFilesInput input);
    
    Task<MessageFileDto> UploadFileAsync(UploadFileInput input);
    
    Task<FileDto> DownloadFileAsync(Guid fileId);
    
    Task DeleteFileAsync(Guid fileId);
}
```

**DTOs:**

```csharp
public class SendMessageWithFilesInput
{
    public Guid TargetUserId { get; set; } // For Direct
    public Guid? ConversationId { get; set; } // For Group/Project/Task
    public string Message { get; set; }
    public List<Guid> FileIds { get; set; } // Uploaded file IDs
}

public class UploadFileInput
{
    public IFormFile File { get; set; }
    public Guid? ConversationId { get; set; } // Optional: pre-upload before sending message
}

public class MessageFileDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public string FileExtension { get; set; }
    public string DownloadUrl { get; set; } // Generated download URL
    public DateTime CreationTime { get; set; }
}
```

#### 2.3.4. Blob Storage Integration

- Sử dụng ABP Blob Storage (đã có Minio trong project)
- Store files trong container: `chat-files`
- Path structure: `{TenantId}/{ConversationId}/{MessageId}/{FileName}`
- Generate signed URLs cho download (có expiration time)

---

## 3. Implementation Steps

### Phase 1: Database & Domain Layer

1. **Tạo enums và constants**
   - `ConversationType` enum
   - Constants cho max lengths

2. **Update Conversation entity**
   - Thêm properties mới (Type, Name, Description, ProjectId, TaskId)
   - **KHÔNG thêm IsPinned** (vì pin là per-user, lưu trong ConversationMember)
   - Update constructors và methods
   - Thêm navigation properties

3. **Tạo ConversationMember entity**
   - Full entity với all properties
   - **Thêm IsPinned và PinnedDate** (per-user pin status)
   - Thêm methods Pin() và Unpin()
   - Repository interface với methods để query pinned conversations

4. **Update Message entity**
   - Thêm Pin và Reply properties
   - Update methods

5. **Tạo MessageFile entity**
   - Full entity
   - Repository interface

6. **Create Migrations**
   - Migration cho Conversation changes
   - Migration cho ConversationMember table
   - Migration cho Message changes
   - Migration cho MessageFile table
   - Data migration script

### Phase 2: Repository & EF Core

1. **Update IConversationRepository**
   - Implement new methods trong EF Core repository

2. **Tạo ConversationMemberRepository**
   - EF Core implementation

3. **Update IMessageRepository**
   - Implement new methods

4. **Tạo MessageFileRepository**
   - EF Core implementation

5. **Update DbContext**
   - Configure new entities
   - Add DbSets

### Phase 3: Application Layer

1. **Update DTOs**
   - ConversationDto với new properties
   - ChatMessageDto với Pin/Reply/Files
   - New input/output DTOs

2. **Update ConversationAppService**
   - Implement new methods
   - **Pin/Unpin logic**: Update ConversationMember.IsPinned cho current user
   - **GetConversations**: Join với ConversationMember để include pin status
   - **GetPinnedConversations**: Query từ ConversationMember where IsPinned = true
   - Business logic validation
   - Permission checks

3. **Tạo MessageFileAppService** (optional, hoặc merge vào ConversationAppService)
   - File upload/download logic
   - Blob storage integration

4. **Update MessagingManager**
   - Support group conversations
   - Support reply messages
   - Support file attachments

### Phase 4: HTTP API Layer

1. **Update Controllers**
   - ConversationController: new endpoints
   - MessageController: pin/reply endpoints
   - FileController: upload/download endpoints

2. **Update API documentation**
   - Swagger annotations

### Phase 5: SignalR & Real-time

1. **Update ChatHub**
   - New events cho group conversations
   - Events cho pinned messages
   - Events cho file uploads

2. **Update Real-time message senders**
   - Support group broadcasting
   - Include file info trong messages

### Phase 6: Blazor UI

1. **Update Conversation List**
   - Show conversation types
   - **Show pinned conversations first** (sorted by PinnedDate desc, then by LastMessageDate)
   - Show pin icon cho conversations đã được current user pin
   - Show member count
   - Pin/unpin action button cho mỗi conversation

2. **Update Chat UI**
   - Reply message UI
   - Pin message UI
   - File attachment UI
   - File preview/download

3. **Create Group Management UI**
   - Create group dialog
   - Add/remove members
   - Update group name

4. **Create Project/Task Integration**
   - Auto-create conversations
   - Link to project/task pages

### Phase 7: Testing & Migration

1. **Unit Tests**
   - Domain logic tests
   - Application service tests

2. **Integration Tests**
   - API endpoint tests
   - SignalR tests

3. **Data Migration**
   - Migrate existing conversations
   - Create ConversationMember records

4. **Performance Testing**
   - Large group conversations
   - File upload/download

---

## 4. Migration từ chat-samples sang src

### 4.1. Copy Source Code

1. **Copy Domain Layer**
   ```
   chat-samples/src/Volo.Chat.Domain/* 
   → src/HC.Domain/Chat/
   ```

2. **Copy Application Layer**
   ```
   chat-samples/src/Volo.Chat.Application/* 
   → src/HC.Application/Chat/
   ```

3. **Copy Application Contracts**
   ```
   chat-samples/src/Volo.Chat.Application.Contracts/* 
   → src/HC.Application.Contracts/Chat/
   ```

4. **Copy EF Core Layer**
   ```
   chat-samples/src/Volo.Chat.EntityFrameworkCore/* 
   → src/HC.EntityFrameworkCore/Chat/
   ```

5. **Copy HTTP API Layer**
   ```
   chat-samples/src/Volo.Chat.HttpApi/* 
   → src/HC.HttpApi/Chat/
   ```

6. **Copy Blazor Components** (optional, nếu muốn customize)
   ```
   chat-samples/src/Volo.Chat.Blazor/* 
   → src/HC.Blazor/Components/Chat/
   ```

### 4.2. Namespace Changes

- Thay đổi namespace từ `Volo.Chat.*` → `HC.Chat.*`
- Update all references

### 4.3. Module Dependencies

- Remove dependency trên `Volo.Chat.*` modules
- Add dependency trên custom `HC.Chat.*` modules

### 4.4. Database Migration

- Tạo initial migration cho chat tables
- Run migration

---

## 5. Best Practices & Considerations

### 5.1. Performance

- **Indexing**: 
  - Index trên `ConversationId`, `UserId` trong `ChatConversationMembers`
  - **Composite index trên `(UserId, IsPinned)`** để query pinned conversations nhanh
  - Index trên `Type` trong `ChatConversations`
- **Caching**: Cache conversation list, pinned messages
- **Pagination**: Always paginate message lists
- **Lazy Loading**: Use explicit loading cho navigation properties

### 5.2. Security

- **Permission Checks**: Verify user is member before accessing conversation
- **File Upload**: Validate file types, sizes
- **File Download**: Generate signed URLs với expiration
- **Group Admin**: Only admins can add/remove members

### 5.3. Scalability

- **SignalR**: Consider using Redis backplane cho multiple servers
- **File Storage**: Use CDN cho file serving
- **Database**: Consider partitioning conversations table by tenant

### 5.4. User Experience

- **Real-time Updates**: Use SignalR cho instant updates
- **File Preview**: Preview images, PDFs inline
- **Typing Indicators**: Show when users are typing
- **Read Receipts**: Show message read status
- **Search**: Full-text search trong messages và files

---

## 6. API Endpoints Summary

### Conversations

```
GET    /api/chat/conversations                    - Get all conversations
GET    /api/chat/conversations/pinned              - Get pinned conversations
GET    /api/chat/conversations/type/{type}         - Get by type
POST   /api/chat/conversations/group               - Create group
POST   /api/chat/conversations/project             - Create project chat
POST   /api/chat/conversations/task                - Create task chat
PUT    /api/chat/conversations/{id}/name           - Update name
POST   /api/chat/conversations/{id}/pin            - Pin conversation
DELETE /api/chat/conversations/{id}/pin             - Unpin conversation
GET    /api/chat/conversations/{id}/members        - Get members
POST   /api/chat/conversations/{id}/members        - Add member
DELETE /api/chat/conversations/{id}/members/{userId} - Remove member
```

### Messages

```
POST   /api/chat/messages                          - Send message
POST   /api/chat/messages/reply                    - Send reply
POST   /api/chat/messages/{id}/pin                 - Pin message
DELETE /api/chat/messages/{id}/pin                 - Unpin message
GET    /api/chat/conversations/{id}/messages/pinned - Get pinned messages
```

### Files

```
POST   /api/chat/files/upload                      - Upload file
GET    /api/chat/files/{id}/download               - Download file
DELETE /api/chat/files/{id}                        - Delete file
GET    /api/chat/messages/{id}/files                - Get message files
```

---

## 7. Timeline Estimate

- **Phase 1-2**: Database & Domain (1-2 weeks)
- **Phase 3**: Application Layer (1-2 weeks)
- **Phase 4**: HTTP API (1 week)
- **Phase 5**: SignalR (1 week)
- **Phase 6**: Blazor UI (2-3 weeks)
- **Phase 7**: Testing & Migration (1-2 weeks)

**Total**: ~8-12 weeks

---

## 8. Notes

### 8.1. Pin Conversation - Important Notes

- **Pin là per-user**: Mỗi user có thể pin/unpin conversations riêng của họ
- **Pin status lưu trong ConversationMember**: Không lưu trong Conversation entity
- **Query optimization**: Sử dụng composite index `(UserId, IsPinned)` để query nhanh
- **UI sorting**: Pinned conversations hiển thị đầu tiên, sort by PinnedDate desc
- **Multiple users**: Một conversation có thể được nhiều users pin, mỗi user có pin status riêng

### 8.2. General Notes

- Cần review và adjust theo business requirements cụ thể
- Consider backward compatibility với existing chat data
- Plan cho rollback strategy nếu có issues
- Document all API changes
- Consider internationalization (i18n) cho UI
- **Testing**: Đặc biệt test pin/unpin với multiple users trong cùng conversation
