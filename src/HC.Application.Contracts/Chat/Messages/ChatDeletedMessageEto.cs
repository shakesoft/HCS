using System;

namespace HC.Chat.Messages;

public class ChatDeletedMessageEto
{
    public Guid TargetUserId { get; set; }
    
    public Guid MessageId { get; set; }
}
