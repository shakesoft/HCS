using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HC.Blazor;
using HC.Blazor.Components.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Volo.Abp.Localization;
using HC.Chat.Authorization;
using HC.Chat.Conversations;
using HC.Chat.Messages;
using HC.Chat.Settings;
using HC.Chat.Users;
using HC.Projects;
using HC.ProjectTasks;
using HC.ProjectMembers;
using HC.Shared;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp.Application.Dtos;
using HC.Blazor.Extensions;


namespace HC.Blazor.Pages.Chat1;

public partial class Chat1 : HCComponentBase
{
    [Inject]
    public IContactAppService ContactAppService { get; set; }

    [Inject]
    public IConversationAppService ConversationAppService { get; set; }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    public ISettingsAppService SettingsAppService { get; set; }
    
    [Inject]
    public new IAuthorizationService AuthorizationService { get; set; }

    public List<ChatContactDto> ChatContactDtos { get; set; } = new List<ChatContactDto>();

    public Dictionary<ChatContactDto, string> ChatContactsActive { get; set; } = new Dictionary<ChatContactDto, string>();

    public Dictionary<ChatContactDto, ElementReference> CanvasElementReferences { get; set; } = new Dictionary<ChatContactDto, ElementReference>();

    public string SearchValue { get; set; } = string.Empty;

    public ChatContactDto CurrentChatContact { get; set; }

    public ElementReference CurrentChatContactCanvas { get; set; }

    public ChatConversationDto ChatConversationDto { get; set; }

    public new string Message { get; set; }

    public ElementReference MessageTextArea { get; set; }

    public bool SendOnEnter { get; set; } = true; // Default: Enter to send
    
    // Loading state
    public bool IsLoadingMessages { get; set; }
    public bool IsSendingMessage { get; set; } // Loading state for send button
    
    // Flag to update avatar after render
    private bool _shouldUpdateAvatar = false;
    
    // New properties for expanded features
    public ChatMessageDto ReplyingToMessage { get; set; }
    
    public List<MessageFileDto> UploadedFiles { get; set; } = new List<MessageFileDto>();
    
    public Guid? CurrentConversationId { get; set; }
    
    // Modal states
    public bool ShowCreateDirectModal { get; set; }
    public bool ShowCreateGroupModal { get; set; }
    public bool ShowCreateProjectModal { get; set; }
    public bool ShowCreateTaskModal { get; set; }
    
    // Form inputs
    public string NewGroupName { get; set; }
    public string NewGroupDescription { get; set; }
    public List<LookupDto<Guid>> SelectedMembers { get; set; } = new List<LookupDto<Guid>>();
    public List<LookupDto<Guid>> SelectedDirectUser { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    
    public List<LookupDto<Guid>> SelectedProject { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> ProjectsCollection { get; set; } = new List<LookupDto<Guid>>();
    public string NewProjectName { get; set; }
    
    public List<LookupDto<Guid>> SelectedTask { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> ProjectTasksCollection { get; set; } = new List<LookupDto<Guid>>();
    public string NewTaskName { get; set; }

    [Inject]
    protected NavigationManager Navigation { get; set; }

    [Inject]
    protected IChatHubConnectionService ChatHubConnectionService { get; set; }
    
    protected ChatSettingsDto ChatSettings { get; set; }
    public string ChatMessagesContainerStyle { get; set; }
    public bool HasSearchingPermission { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await GetChatSettingsAsync();

        HasSearchingPermission = await AuthorizationService.IsGrantedAsync(ChatPermissions.Searching);

        await ChatHubConnectionService.OnReceiveMessageAsync(async message =>
        {
            if (CurrentChatContact != null)
            {
                // Check if message is for current conversation
                bool isForCurrentConversation = false;
                if (CurrentChatContact.Type == ConversationType.Direct && CurrentChatContact.UserId == message.SenderUserId)
                {
                    isForCurrentConversation = true;
                }
                else if (CurrentChatContact.Type != ConversationType.Direct && CurrentConversationId.HasValue)
                {
                    // For group conversations, check if sender is a member (simplified check)
                    isForCurrentConversation = true; // TODO: Implement proper check
                }
                
                if (isForCurrentConversation)
                {
                    // Refresh conversation
                    if (CurrentChatContact.Type == ConversationType.Direct)
                    {
                        ChatConversationDto = await ConversationAppService.GetConversationAsync(
                            new GetConversationInput { TargetUserId = CurrentChatContact.UserId, MaxResultCount = 100 });
                    }
                    else if (CurrentChatContact.ConversationId.HasValue)
                    {
                        ChatConversationDto = await ConversationAppService.GetConversationAsync(
                            new GetConversationInput { ConversationId = CurrentChatContact.ConversationId.Value, TargetUserId = Guid.Empty, MaxResultCount = 100 });
                    }

                    if (CurrentChatContact.UnreadMessageCount > 0 && CurrentChatContact.Type == ConversationType.Direct)
                    {
                        await ConversationAppService.MarkConversationAsReadAsync(
                            new MarkConversationAsReadInput { TargetUserId = CurrentChatContact.UserId });
                    }

                    if (ChatConversationDto != null)
                    {
                        ChatConversationDto.Messages.Reverse();
                        var lastMessage = ChatConversationDto.Messages.LastOrDefault();
                        CurrentChatContact.LastMessage = lastMessage?.Message;
                        CurrentChatContact.LastMessageDate = lastMessage?.MessageDate;
                    }
                }
            }

            await InvokeAsync(StateHasChanged);
        });

        await ChatHubConnectionService.OnDeletedMessageAsync(async messageId =>
        {
            ChatConversationDto.Messages.RemoveAll(message => message.Id == messageId);
            var lastMessage = ChatConversationDto.Messages.LastOrDefault();
            CurrentChatContact.LastMessage = lastMessage?.Message;
            CurrentChatContact.LastMessageDate = lastMessage?.MessageDate;
            await InvokeAsync(StateHasChanged);
        });
        
        await ChatHubConnectionService.OnDeletedConversationAsync(async userId =>
        {
            if (userId == CurrentChatContact.UserId)
            {
                CanvasElementReferences.Clear();
                ChatConversationDto = null;
                ChatContactDtos.RemoveAll(contact => contact.UserId == userId);
            }
            await InvokeAsync(StateHasChanged);
        });
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            await GetContactsAsync();
        }

        foreach (var contactDto in ChatContactDtos)
        {
            if (CanvasElementReferences.ContainsKey(contactDto))
            {
                await JsRuntime.SafeInvokeVoidAsync("VoloChatAvatarManager.createCanvasForUser", CanvasElementReferences[contactDto], contactDto.Username, GetName(contactDto));
            }
        }
        
        // Update avatar for current chat contact after canvas is rendered in DOM
        if (_shouldUpdateAvatar && CurrentChatContact?.Type == ConversationType.Direct)
        {
            _shouldUpdateAvatar = false;
            try
            {
                await JsRuntime.SafeInvokeVoidAsync("VoloChatAvatarManager.createCanvasForUser", CurrentChatContactCanvas, CurrentChatContact?.Username, GetName(CurrentChatContact));
            }
            catch
            {
                // Ignore errors - canvas might not be ready or component disposed
            }
        }

        await JsRuntime.SafeInvokeVoidAsync("eval", "document.getElementById('chat-messages-container')?.scrollTo({ top: document.getElementById('chat-messages-container').scrollHeight, behavior: 'smooth' })");
    }

    public static string GetName(ChatContactDto contact)
    {
        var name = "";

        if (!string.IsNullOrEmpty(contact.Name))
        {
            name += contact.Name + ' ';
        }

        if (!string.IsNullOrEmpty(contact.Surname))
        {
            name += contact.Surname;
        }

        if (name == string.Empty)
        {
            name = contact.Username;
        }

        return name;
    }
    
    public static string GetContactDisplayName(ChatContactDto contact)
    {
        // For group/project/task, use ConversationName if available
        if (contact.Type != ConversationType.Direct && !string.IsNullOrWhiteSpace(contact.ConversationName))
        {
            return contact.ConversationName;
        }
        
        return GetName(contact);
    }

    public async Task GetContactsAsync(bool includeOtherContacts = false)
    {
        CanvasElementReferences.Clear();
        ChatContactsActive.Clear();

        var input = new GetContactsInput
        {
            Filter = SearchValue ?? string.Empty,
            IncludeOtherContacts = includeOtherContacts
        };
        
        ChatContactDtos = await ContactAppService.GetContactsAsync(input);

        foreach (var contactDto in ChatContactDtos)
        {
            ChatContactsActive[contactDto] = "";
        }

        CurrentChatContact = ChatContactDtos.FirstOrDefault();
        if (CurrentChatContact != null)
        {
            await SetActiveAsync(CurrentChatContact);
        }
        else
        {
            await JsRuntime.SafeInvokeVoidAsync("VoloChatAvatarManager.createCanvasForUser", CurrentChatContactCanvas, null, null);
            ChatConversationDto = null;
        }

        
        await InvokeAsync(StateHasChanged);
    }
    
    protected virtual bool IsDeletingMessageEnabled(ChatMessageDto message)
    {
        if (ChatSettings.DeletingMessages == ChatDeletingMessages.Disabled)
        {
            return false;
        }

        if (ChatSettings.DeletingMessages == ChatDeletingMessages.EnabledWithDeletionPeriod)
        {
            if(message.MessageDate.AddSeconds(ChatSettings.MessageDeletionPeriod) < Clock.Now)
            {
                return false;
            }
        }

        return true;
    }
    
    protected virtual bool IsDeletingConversationEnabled()
    {
        if (ChatSettings.DeletingMessages != ChatDeletingMessages.Enabled)
        {
            return false;
        }
        
        if (ChatSettings.DeletingConversations == ChatDeletingConversations.Disabled)
        {
            return false;
        }
        
        return true;
    }
    
    protected virtual async Task DeleteMessageAsync(ChatMessageDto message)
    {
        try
        {
            await ConversationAppService.DeleteMessageAsync(new DeleteMessageInput
            {
                MessageId = message.Id,
                TargetUserId = CurrentChatContact.UserId
            });
            
            ChatConversationDto.Messages.Remove(message);
            var lastMessage = ChatConversationDto.Messages.LastOrDefault();
            CurrentChatContact.LastMessage = lastMessage?.Message;
            CurrentChatContact.LastMessageDate = lastMessage?.MessageDate;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    protected virtual async Task DeleteConversationAsync()
    {
        try
        {
            await ConversationAppService.DeleteConversationAsync(new DeleteConversationInput
            {
                TargetUserId = CurrentChatContact.UserId
            });

            ChatConversationDto = null;
            await GetContactsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task SetActiveAsync(ChatContactDto contactDto)
    {
        try
        {
            // Show loading spinner
            IsLoadingMessages = true;
            ChatConversationDto = null; // Clear previous messages
            await InvokeAsync(StateHasChanged);
            
            // Get chat settings safely
            try
            {
                await GetChatSettingsAsync();
            }
            catch
            {
                // If settings fail, continue with default behavior
            }
            
            CurrentChatContact = contactDto;
            
            // For group conversations, always allow chat (permission check not needed)
            // For direct conversations, check HasChatPermission
            var hasPermission = contactDto.Type != ConversationType.Direct || contactDto.HasChatPermission;
            ChatMessagesContainerStyle = !hasPermission ? "pointer-events: none; opacity: 0.5;" : "";
            
            ChatContactsActive[contactDto] = "active";
            foreach (var dto in ChatContactsActive.Where(x => x.Key != contactDto))
            {
                ChatContactsActive[dto.Key] = "";
            }

            // Update avatar/icon based on conversation type
            // For Group/Project/Task, icon will be shown in UI instead of canvas
            // Canvas update will be done after messages are loaded

            // Support both Direct and Group/Project/Task conversations
            if (CurrentChatContact.Type == ConversationType.Direct)
            {
                ChatConversationDto = await ConversationAppService.GetConversationAsync(new GetConversationInput
                {
                    TargetUserId = CurrentChatContact.UserId,
                    MaxResultCount = 100
                });
                CurrentConversationId = null;
            }
            else if (CurrentChatContact.ConversationId.HasValue)
            {
                ChatConversationDto = await ConversationAppService.GetConversationAsync(new GetConversationInput
                {
                    ConversationId = CurrentChatContact.ConversationId.Value,
                    TargetUserId = Guid.Empty, // Not used for group conversations
                    MaxResultCount = 100
                });
                CurrentConversationId = CurrentChatContact.ConversationId.Value;
            }
            else
            {
                // No conversation ID for group, create empty conversation
                ChatConversationDto = new ChatConversationDto
                {
                    Messages = new List<ChatMessageDto>()
                };
                CurrentConversationId = null;
            }
            
            // Hide loading spinner and update UI
            IsLoadingMessages = false;
            
            // Set flag to update avatar after render
            if (CurrentChatContact?.Type == ConversationType.Direct)
            {
                _shouldUpdateAvatar = true;
            }
            
            await InvokeAsync(StateHasChanged);

            if (contactDto.UnreadMessageCount > 0 && CurrentChatContact.Type == ConversationType.Direct)
            {
                await ConversationAppService.MarkConversationAsReadAsync(new MarkConversationAsReadInput
                {
                    TargetUserId = contactDto.UserId
                });
            }

            if (ChatConversationDto?.Messages != null)
            {
                ChatConversationDto.Messages.Reverse();
                var lastMessage = ChatConversationDto.Messages.LastOrDefault();
                CurrentChatContact.LastMessage = lastMessage?.Message;
                CurrentChatContact.LastMessageDate = lastMessage?.MessageDate;
            }

            Message = "";

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnSearchChangeAsync(string value)
    {
        SearchValue = value;
        await GetContactsAsync(!SearchValue.IsNullOrEmpty());
    }

    private async Task OnSearchKeyupAsync(KeyboardEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
        await GetContactsAsync(!SearchValue.IsNullOrEmpty());
    }


    private async Task StartConversationAsync()
    {
        await OnSearchChangeAsync(" ");
    }

    private async Task OnMessageEntryAsync(KeyboardEventArgs e)
    {
        // Send on Enter if enabled and not already sending
        if (e.Code == "Enter" && SendOnEnter && !IsSendingMessage)
        {
            await SendMessageAsync();
        }
    }
    
    private async Task GetChatSettingsAsync()
    {
        ChatSettings = await SettingsAppService.GetAsync();
    }
    
    // New methods for expanded features
    private async Task ShowCreateDirectModalAsync()
    {
        ShowCreateDirectModal = true;
        SelectedDirectUser.Clear();
        await GetIdentityUserCollectionLookupAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task CreateDirectConversationAsync()
    {
        try
        {
            if (!SelectedDirectUser.Any())
            {
                // TODO: Show warning message
                return;
            }
            
            var targetUserId = SelectedDirectUser.First().Id;
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            
            if (targetUserId == currentUserId)
            {
                // Cannot chat with yourself
                // TODO: Show warning message
                return;
            }
            
            // Check if conversation already exists in current list
            var existingContact = ChatContactDtos.FirstOrDefault(c => c.Type == ConversationType.Direct && c.UserId == targetUserId);
            if (existingContact != null)
            {
                // Conversation already exists, just set it active
                await SetActiveAsync(existingContact);
            }
            else
            {
                // Refresh contacts to get the user (including other contacts)
                await GetContactsAsync(true);
                
                // Check again after refresh
                existingContact = ChatContactDtos.FirstOrDefault(c => c.Type == ConversationType.Direct && c.UserId == targetUserId);
                if (existingContact != null)
                {
                    await SetActiveAsync(existingContact);
                }
                else
                {
                    // User not found in contacts, get user info and create contact entry
                    // The conversation will be created automatically when first message is sent
                    var allContacts = await ContactAppService.GetContactsAsync(new GetContactsInput
                    {
                        Filter = "",
                        IncludeOtherContacts = true
                    });
                    
                    var newContact = allContacts.FirstOrDefault(c => c.UserId == targetUserId);
                    if (newContact != null)
                    {
                        newContact.Type = ConversationType.Direct;
                        ChatContactDtos.Insert(0, newContact);
                        ChatContactsActive[newContact] = "";
                        await SetActiveAsync(newContact);
                    }
                    else
                    {
                        // Fallback: create minimal contact entry
                        var selectedUser = SelectedDirectUser.First();
                        var fallbackContact = new ChatContactDto
                        {
                            UserId = targetUserId,
                            Username = selectedUser.DisplayName,
                            Name = selectedUser.DisplayName,
                            Type = ConversationType.Direct,
                            HasChatPermission = true
                        };
                        
                        ChatContactDtos.Insert(0, fallbackContact);
                        ChatContactsActive[fallbackContact] = "";
                        await SetActiveAsync(fallbackContact);
                    }
                }
            }
            
            ShowCreateDirectModal = false;
            SelectedDirectUser.Clear();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task ShowCreateGroupModalAsync()
    {
        ShowCreateGroupModal = true;
        NewGroupName = "";
        NewGroupDescription = "";
        SelectedMembers.Clear();
        await GetIdentityUserCollectionLookupAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task ShowCreateProjectModalAsync()
    {
        ShowCreateProjectModal = true;
        SelectedProject.Clear();
        NewProjectName = "";
        SelectedMembers.Clear();
        await GetProjectCollectionLookupAsync();
        await GetIdentityUserCollectionLookupAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task ShowCreateTaskModalAsync()
    {
        ShowCreateTaskModal = true;
        SelectedTask.Clear();
        NewTaskName = "";
        SelectedMembers.Clear();
        await GetProjectTaskCollectionLookupAsync();
        await GetIdentityUserCollectionLookupAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task CreateGroupConversationAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewGroupName) || !SelectedMembers.Any())
            {
                // TODO: Show warning message
                return;
            }
            
            var memberIds = SelectedMembers.Select(m => m.Id).ToList();
            var result = await ConversationAppService.CreateGroupConversationAsync(new CreateGroupConversationInput
            {
                Name = NewGroupName,
                Description = NewGroupDescription,
                MemberUserIds = memberIds
            });
            
            ShowCreateGroupModal = false;
            NewGroupName = "";
            NewGroupDescription = "";
            SelectedMembers.Clear();
            await GetContactsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task CreateProjectConversationAsync()
    {
        try
        {
            if (!SelectedProject.Any())
            {
                // TODO: Show warning message
                return;
            }
            
            var projectId = SelectedProject.First().Id;
            var memberIds = SelectedMembers?.Select(m => m.Id).ToList() ?? new List<Guid>();
            
            var result = await ConversationAppService.CreateProjectConversationAsync(new CreateProjectConversationInput
            {
                ProjectId = projectId,
                Name = NewProjectName,
                MemberUserIds = memberIds
            });
            
            ShowCreateProjectModal = false;
            SelectedProject.Clear();
            NewProjectName = "";
            SelectedMembers.Clear();
            await GetContactsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task CreateTaskConversationAsync()
    {
        try
        {
            if (!SelectedTask.Any())
            {
                // TODO: Show warning message
                return;
            }
            
            var taskId = SelectedTask.First().Id;
            var memberIds = SelectedMembers?.Select(m => m.Id).ToList() ?? new List<Guid>();
            
            var result = await ConversationAppService.CreateTaskConversationAsync(new CreateTaskConversationInput
            {
                TaskId = taskId,
                Name = NewTaskName,
                MemberUserIds = memberIds
            });
            
            ShowCreateTaskModal = false;
            SelectedTask.Clear();
            NewTaskName = "";
            SelectedMembers.Clear();
            await GetContactsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    // Select2 lookup methods
    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await ((IProjectMembersAppService)ProjectMembersAppService).GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    
    private async Task<List<LookupDto<Guid>>> GetIdentityUserCollectionLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        IdentityUsersCollection = (await ProjectMembersAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = filter })).Items;
        return IdentityUsersCollection.ToList();
    }
    
    private async Task GetProjectCollectionLookupAsync(string? newValue = null)
    {
        ProjectsCollection = (await ProjectTasksAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    
    private async Task<List<LookupDto<Guid>>> GetProjectCollectionLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        ProjectsCollection = (await ProjectTasksAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = filter })).Items;
        return ProjectsCollection.ToList();
    }
    
    private async Task GetProjectTaskCollectionLookupAsync(string? newValue = null)
    {
        var input = new GetProjectTasksInput
        {
            FilterText = newValue,
            MaxResultCount = 20,
            SkipCount = 0,
        };
        
        var result = await ProjectTasksAppService.GetListAsync(input);
        ProjectTasksCollection = result.Items
            .Select(x => new LookupDto<Guid>
            {
                Id = x.ProjectTask.Id,
                DisplayName = $"{x.ProjectTask.Code} - {x.ProjectTask.Title}"
            })
            .ToList();
    }
    
    private async Task<List<LookupDto<Guid>>> GetProjectTaskCollectionLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var input = new GetProjectTasksInput
        {
            FilterText = filter,
            MaxResultCount = 20,
            SkipCount = 0,
        };
        
        var result = await ProjectTasksAppService.GetListAsync(input);
        ProjectTasksCollection = result.Items
            .Select(x => new LookupDto<Guid>
            {
                Id = x.ProjectTask.Id,
                DisplayName = $"{x.ProjectTask.Code} - {x.ProjectTask.Title}"
            })
            .ToList();
        
        return ProjectTasksCollection.ToList();
    }
    
    private async Task TogglePinConversationAsync(ChatContactDto contact)
    {
        try
        {
            if (!contact.ConversationId.HasValue)
            {
                return; // Direct conversations don't support pinning via this method
            }
            
            if (contact.IsPinned)
            {
                await ConversationAppService.UnpinConversationAsync(contact.ConversationId.Value);
            }
            else
            {
                await ConversationAppService.PinConversationAsync(contact.ConversationId.Value);
            }
            
            await GetContactsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task ReplyToMessageAsync(ChatMessageDto message)
    {
        ReplyingToMessage = message;
        await MessageTextArea.FocusAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task TogglePinMessageAsync(ChatMessageDto message)
    {
        try
        {
            if (message.IsPinned)
            {
                await ConversationAppService.UnpinMessageAsync(message.Id);
            }
            else
            {
                await ConversationAppService.PinMessageAsync(message.Id);
            }
            
            // Refresh conversation
            if (CurrentChatContact != null)
            {
                await SetActiveAsync(CurrentChatContact);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        try
        {
            foreach (var file in e.GetMultipleFiles(int.MaxValue))
            {
                // Validate file size (100MB max)
                if (file.Size > 100 * 1024 * 1024)
                {
                    // TODO: Show error message
                    continue;
                }
                
                // Read file content
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream(long.MaxValue).CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var fileBytes = memoryStream.ToArray();
                
                // Upload file
                var uploadedFile = await ConversationAppService.UploadFileAsync(new UploadFileInput
                {
                    FileContent = fileBytes,
                    FileName = file.Name,
                    ContentType = file.ContentType,
                    ConversationId = CurrentConversationId
                });
                
                UploadedFiles.Add(uploadedFile);
            }
            
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task DownloadFileAsync(Guid fileId)
    {
        try
        {
            var file = await ConversationAppService.DownloadFileAsync(fileId);
            
            // Create download link using JavaScript
            var base64 = Convert.ToBase64String(file.Content);
            var dataUrl = $"data:{file.ContentType};base64,{base64}";
            
            // Using SafeInvokeVoidAsync to automatically handle JSDisconnectedException
            await JsRuntime.SafeInvokeVoidAsync("eval", $@"
                var link = document.createElement('a');
                link.href = '{dataUrl}';
                link.download = '{file.FileName}';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            ");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task SendMessageAsync()
    {
        if (Message.IsNullOrWhiteSpace() && (UploadedFiles == null || !UploadedFiles.Any()))
        {
            return;
        }

        if (CurrentChatContact == null)
        {
            return;
        }

        // Set sending state and show spinner in send button
        IsSendingMessage = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            if (ReplyingToMessage != null)
            {
                // Send reply message
                await ConversationAppService.SendReplyMessageAsync(new SendReplyMessageInput
                {
                    TargetUserId = CurrentChatContact.UserId,
                    ConversationId = CurrentConversationId,
                    ReplyToMessageId = ReplyingToMessage.Id,
                    Message = Message ?? string.Empty
                });
                
                ReplyingToMessage = null;
            }
            else if (UploadedFiles != null && UploadedFiles.Any())
            {
                // Send message with files
                await ConversationAppService.SendMessageWithFilesAsync(new SendMessageWithFilesInput
                {
                    TargetUserId = CurrentChatContact.UserId,
                    ConversationId = CurrentConversationId,
                    Message = Message,
                    FileIds = UploadedFiles.Select(f => f.Id).ToList()
                });
                
                UploadedFiles.Clear();
            }
            else
            {
                // Send normal message
                await ConversationAppService.SendMessageAsync(new SendMessageInput
                {
                    Message = Message,
                    TargetUserId = CurrentChatContact.UserId
                });
            }

            Message = "";
            
            // Refresh conversation to show new message (without showing loading spinner in chatBox)
            if (CurrentChatContact.Type == ConversationType.Direct)
            {
                ChatConversationDto = await ConversationAppService.GetConversationAsync(new GetConversationInput
                {
                    TargetUserId = CurrentChatContact.UserId,
                    MaxResultCount = 100
                });
            }
            else if (CurrentChatContact.ConversationId.HasValue)
            {
                ChatConversationDto = await ConversationAppService.GetConversationAsync(new GetConversationInput
                {
                    ConversationId = CurrentChatContact.ConversationId.Value,
                    TargetUserId = Guid.Empty,
                    MaxResultCount = 100
                });
            }
            
            if (ChatConversationDto?.Messages != null)
            {
                ChatConversationDto.Messages.Reverse();
                var lastMessage = ChatConversationDto.Messages.LastOrDefault();
                CurrentChatContact.LastMessage = lastMessage?.Message;
                CurrentChatContact.LastMessageDate = lastMessage?.MessageDate;
            }
            
            // Refresh contacts to update last message
            await GetContactsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            // Hide spinner in send button
            IsSendingMessage = false;
            await InvokeAsync(StateHasChanged);
            await MessageTextArea.FocusAsync();
        }
    }
}
