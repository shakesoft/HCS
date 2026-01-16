using System;
using System.Collections.Generic;
using HC.Chat.Messages;
using HC.Chat.Users;

namespace HC.Chat.Conversations;

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
