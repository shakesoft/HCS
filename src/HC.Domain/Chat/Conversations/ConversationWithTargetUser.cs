using HC.Chat.Users;

namespace HC.Chat.Conversations;

public class ConversationWithTargetUser
{
    public Conversation Conversation { get; set; }

    public ChatUser TargetUser { get; set; }
}
