using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using HC.Chat.Messages;
using HC.Chat.Conversations;

namespace HC.Chat.Conversations;

public class Conversation : Entity<Guid>, IMultiTenant, IAggregateRoot<Guid>
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid UserId { get; protected set; }

    public virtual Guid? TargetUserId { get; protected set; } // Nullable for group chats
    
    // New properties
    public virtual ConversationType Type { get; protected set; }
    public virtual string? Name { get; protected set; } // For groups/projects/tasks
    public virtual string? Description { get; protected set; }
    public virtual Guid? ProjectId { get; protected set; } // For PROJECT type
    public virtual Guid? TaskId { get; protected set; }   // For TASK type

    public virtual ChatMessageSide LastMessageSide { get; set; }

    public virtual string? LastMessage { get; set; }

    public virtual DateTime LastMessageDate { get; set; }

    public virtual int UnreadMessageCount { get; protected set; }
    
    // Navigation properties
    public virtual ICollection<ConversationMember> Members { get; protected set; }

    protected Conversation()
    {
        Members = new List<ConversationMember>();
    }

    public Conversation(
        Guid id, 
        Guid userId, 
        Guid? targetUserId, 
        ConversationType type = ConversationType.Direct,
        string name = null,
        string description = null,
        Guid? projectId = null,
        Guid? taskId = null,
        Guid? tenantId = null)
        : base(id)
    {
        UserId = userId;
        TargetUserId = targetUserId;
        Type = type;
        Name = name;
        Description = description;
        ProjectId = projectId;
        TaskId = taskId;
        TenantId = tenantId;
        Members = new List<ConversationMember>();
    }

    public virtual void AddUnreadMessage(int count = 1)
    {
        UnreadMessageCount += count;
    }

    public virtual void ResetUnreadMessageCount()
    {
        UnreadMessageCount = 0;
    }

    public void SetLastMessage(string messageText, DateTime messageTime, ChatMessageSide messageSide, bool ignoreNullOrEmpty = false)
    {
        LastMessage = ignoreNullOrEmpty ? messageText : Check.NotNullOrWhiteSpace(messageText, nameof(messageText));
        LastMessageDate = messageTime;
        LastMessageSide = messageSide;

        if (messageSide == ChatMessageSide.Sender)
        {
            ResetUnreadMessageCount();
        }
        else
        {
            AddUnreadMessage();
        }
    }
    
    public virtual void UpdateName(string name)
    {
        Name = name;
    }
    
    public virtual void UpdateDescription(string description)
    {
        Description = description;
    }
}
