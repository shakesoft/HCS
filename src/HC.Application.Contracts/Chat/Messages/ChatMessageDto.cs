using System;
using System.Collections.Generic;

namespace HC.Chat.Messages;

public class ChatMessageDto
{
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
    
    // Sender information (for Group/Project/Task conversations)
    public Guid? SenderUserId { get; set; }
    public string SenderName { get; set; }
    public string SenderSurname { get; set; }
    public string SenderUsername { get; set; }
}
