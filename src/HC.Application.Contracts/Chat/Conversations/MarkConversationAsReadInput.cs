using System;

namespace HC.Chat.Conversations;

public class MarkConversationAsReadInput
{
    public Guid TargetUserId { get; set; }
}
