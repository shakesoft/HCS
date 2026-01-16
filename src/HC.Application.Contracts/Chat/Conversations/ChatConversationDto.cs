using System.Collections.Generic;
using HC.Chat.Messages;
using HC.Chat.Users;

namespace HC.Chat.Conversations;

public class ChatConversationDto
{
    public List<ChatMessageDto> Messages { get; set; }

    public ChatTargetUserInfo TargetUserInfo { get; set; }
}
