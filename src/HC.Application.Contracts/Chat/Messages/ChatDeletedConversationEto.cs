using System;

namespace HC.Chat.Messages;

public class ChatDeletedConversationEto
{
    public Guid TargetUserId { get; set; }
    
    public Guid UserId { get; set; }
}
