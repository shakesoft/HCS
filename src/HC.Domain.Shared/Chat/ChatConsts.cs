namespace HC.Chat;

public static class ChatConsts
{
    public const int MaxConversationNameLength = 256;
    public const int MaxConversationDescriptionLength = 1000;
    public const int MaxMessageTextLength = 4096;
    public const int MaxFileNameLength = 512;
    public const int MaxFileSize = 100 * 1024 * 1024; // 100MB
    public const int OtherContactLimitPerRequest = 50;
}
