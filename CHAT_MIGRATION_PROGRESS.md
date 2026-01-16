# Chat Module Migration Progress

## Đã hoàn thành ✅

### 1. Domain Layer - Entities
- ✅ `ConversationType` enum
- ✅ `ChatConsts` 
- ✅ `ChatMessageSide` enum
- ✅ `ChatMessageConsts`
- ✅ `Conversation` entity (với properties mới: Type, Name, Description, ProjectId, TaskId)
- ✅ `ConversationMember` entity (với IsPinned, PinnedDate)
- ✅ `Message` entity (với IsPinned, ReplyToMessageId)
- ✅ `MessageFile` entity (với SetMessageId method)
- ✅ `UserMessage` entity
- ✅ `ChatUser` entity
- ✅ Helper classes: `ConversationPair`, `ConversationWithTargetUser`, `MessageWithDetails`

### 2. Domain Layer - Repositories
- ✅ `IConversationRepository` (với methods mới)
- ✅ `IConversationMemberRepository` (mới)
- ✅ `IMessageRepository` (với methods mới)
- ✅ `IMessageFileRepository` (mới)
- ✅ `IChatUserRepository`
- ✅ `IChatUserLookupService`
- ✅ `ChatUserLookupService`
- ✅ `ChatUserSynchronizer`
- ✅ `MessagingManager`
- ✅ `ChatDomainModule`

### 3. Application Contracts Layer
- ✅ `IConversationAppService` với tất cả methods mới
- ✅ `IContactAppService`
- ✅ `ISettingsAppService`
- ✅ Tất cả DTOs (ConversationDto, ChatMessageDto, MessageFileDto, etc.)
- ✅ `GetConversationInput` updated với ConversationId support

### 4. Application Layer
- ✅ `ConversationAppService` với **FULL IMPLEMENTATION** của tất cả methods:
  - ✅ `CreateGroupConversationAsync` - Tạo group conversation với nhiều members
  - ✅ `CreateProjectConversationAsync` - Tạo project conversation
  - ✅ `CreateTaskConversationAsync` - Tạo task conversation
  - ✅ `UpdateConversationNameAsync` - Update tên conversation
  - ✅ `PinConversationAsync` - Pin conversation (per-user)
  - ✅ `UnpinConversationAsync` - Unpin conversation (per-user)
  - ✅ `AddMemberAsync` - Thêm member vào conversation
  - ✅ `RemoveMemberAsync` - Xóa member khỏi conversation
  - ✅ `GetMembersAsync` - Lấy danh sách members
  - ✅ `GetPinnedConversationsAsync` - Lấy danh sách pinned conversations
  - ✅ `GetByTypeAsync` - Lấy conversations theo type
  - ✅ `SendReplyMessageAsync` - Gửi reply message
  - ✅ `PinMessageAsync` - Pin message
  - ✅ `UnpinMessageAsync` - Unpin message
  - ✅ `GetPinnedMessagesAsync` - Lấy pinned messages
  - ✅ `SendMessageWithFilesAsync` - Gửi message kèm files
  - ✅ `UploadFileAsync` - Upload file lên MINIO
  - ✅ `DownloadFileAsync` - Download file từ MINIO
  - ✅ `DeleteFileAsync` - Xóa file
- ✅ `ContactAppService`
- ✅ `SettingsAppService`
- ✅ `DistributedEventBusRealTimeChatMessageSender`
- ✅ `ChatApplicationModule`
- ✅ Helper methods: `MapToConversationDtoAsync`, `MapToChatMessageDtoAsync`

### 5. EF Core Layer
- ✅ `IChatDbContext`
- ✅ `ChatDbContextModelCreatingExtensions` với configurations cho tất cả entities mới
- ✅ `EfCoreConversationRepository` (với methods mới)
- ✅ `EfCoreConversationMemberRepository` (mới)
- ✅ `EfCoreMessageRepository` (với methods mới)
- ✅ `EfCoreMessageFileRepository` (mới)
- ✅ `EfCoreUserMessageRepository`
- ✅ `EfCoreChatUserRepository`
- ✅ `HCChatEntityFrameworkCoreModule`
- ✅ `HCDbContextBase` implements `IChatDbContext` với Chat DbSets
- ✅ `HCEntityFrameworkCoreModule` đã register tất cả Chat repositories

### 6. HTTP API Layer
- ✅ `HCChatHttpApiModule`
- ✅ `ConversationController` với tất cả endpoints mới
- ✅ `ContactController`
- ✅ `FileController`
- ✅ `ChatController` base class

### 7. Module Updates
- ✅ `HCDomainModule` - updated to use `HCChatDomainModule`
- ✅ `HCDomainSharedModule` - updated to use `HCChatDomainSharedModule`
- ✅ `HCApplicationModule` - updated to use `HCChatApplicationModule`
- ✅ `HCHttpApiModule` - updated to use `HCChatHttpApiModule`
- ✅ `HCEntityFrameworkCoreModule` - updated to use `HCChatEntityFrameworkCoreModule`
- ✅ `HCHttpApiHostModule` - removed Volo.Chat dependencies
- ✅ `HCBlazorModule` - commented out Chat SignalR dependencies (cần tạo sau)
- ✅ `HCHttpApiClientModule` - removed Volo.Chat dependencies

### 8. Database Migrations
- ✅ Migration `AddChatModuleExpansion` đã được tạo thành công
- ✅ Migration đã được apply lên database thành công

### 9. Blazor Components
- ✅ `IChatHubConnectionService` và `ChatHubConnectionService` đã được tạo
- ✅ `Chat1.razor` và `Chat1.razor.cs` đã được update để sử dụng `HC.Chat` namespaces

## Đã hoàn thành ✅ (tiếp)

### 10. UI Updates (Chat1.razor)
- ✅ Thêm UI buttons để tạo group/project/task conversations với modals
- ✅ Thêm icons cho conversation types (direct/group/task/project) trong contact list
- ✅ Thêm pin/unpin conversation button trong dropdown menu
- ✅ Thêm reply message UI với preview
- ✅ Thêm pin message UI trong message dropdown
- ✅ Thêm file upload UI với InputFile component
- ✅ Thêm file preview/download trong messages
- ✅ Update `Chat1.razor.cs` với tất cả methods mới
- ✅ Update `ContactAppService` để map ConversationId, IsPinned, MemberCount
- ✅ Update `ChatContactDto` với ConversationId property
- ✅ Update `SetActiveAsync` để support cả Direct và Group/Project/Task conversations

## Cần làm tiếp

### Phase 11: Testing & Refinement
1. Test tất cả features:
   - Tạo group/project/task conversations
   - Pin/unpin conversations
   - Add/remove members
   - Send messages với reply
   - Pin/unpin messages
   - Upload/download files
2. Fix bugs nếu có
3. Optimize queries nếu cần
4. Implement member selection UI trong modals (hiện tại là placeholder)
5. Implement project/task selection dropdown trong modals (hiện tại là placeholder)
6. Implement proper message retrieval cho group conversations (hiện tại return empty list)

### Phase 11: SignalR Integration (Optional)
1. Tạo `HCChatSignalRModule` nếu cần real-time updates
2. Implement SignalR Hub cho chat (hoặc extend NotificationHub)
3. Update `HCBlazorModule` và `HCHttpApiHostModule` để enable SignalR
4. Update `ChatHubConnectionService` để connect tới SignalR Hub

### Phase 12: Testing & Refinement
1. Test tất cả features:
   - Tạo group/project/task conversations
   - Pin/unpin conversations
   - Add/remove members
   - Send messages với reply
   - Pin/unpin messages
   - Upload/download files
2. Fix bugs nếu có
3. Optimize queries nếu cần

## Notes

- ✅ Migration đã được tạo và apply thành công
- ✅ Tất cả modules đã được update để sử dụng `HC.Chat` thay vì `Volo.Chat`
- ✅ Application Services đã được implement đầy đủ với logic chi tiết
- ✅ File upload sử dụng ABP Blob Storage với MINIO
- ✅ File path structure: `chat-files/{TenantId}/{ConversationId}/{MessageId}/{FileName}`
- ⏳ SignalR module đã được comment out, cần tạo sau nếu cần real-time features
- ⏳ UI updates cần được implement trong Chat1.razor
- ⏳ Group conversation message retrieval cần được implement (hiện tại return empty list)

## Implementation Details

### File Upload Flow
1. User uploads file → `UploadFileAsync` → Save to MINIO → Return `MessageFileDto` với temp MessageId
2. User sends message → `SendMessageWithFilesAsync` → Create message → Link files via `SetMessageId()`
3. File path: `chat-files/{TenantId}/{ConversationId}/{MessageId}/{FileName}`

### Conversation Creation Flow
1. Create main `Conversation` entity for creator
2. Create `ConversationMember` entries for all members
3. Create `Conversation` entries for each member (similar to Direct conversation pattern)

### Pin Conversation Logic
- Stored in `ConversationMember` entity (per-user)
- Query via `GetPinnedByUserIdAsync` in repository
- Indexed for performance: `(UserId, IsPinned)`
