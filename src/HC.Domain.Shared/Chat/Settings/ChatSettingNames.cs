namespace HC.Chat.Settings;

public static class ChatSettingNames
{
    private const string Prefix = "HC.Chat";

    public static class Messaging
    {
        private const string MessagingPrefix = Prefix + ".Messaging";

        public const string SendMessageOnEnter = MessagingPrefix + ".SendMessageOnEnter";
        
        public const string DeletingMessages = MessagingPrefix + ".DeletingMessages";
        
        public const string DeletingConversations = MessagingPrefix + ".DeletingConversations";
        
        public const string MessageDeletionPeriod = MessagingPrefix + ".MessageDeletionPeriod";
    }
}
