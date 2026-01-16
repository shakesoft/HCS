namespace HC.Chat.Conversations;

/// <summary>
/// Type of conversation
/// </summary>
public enum ConversationType
{
    /// <summary>
    /// Direct chat between two users (1-1)
    /// </summary>
    Direct = 1,
    
    /// <summary>
    /// Group chat with multiple members
    /// </summary>
    Group = 2,
    
    /// <summary>
    /// Project-related chat
    /// </summary>
    Project = 3,
    
    /// <summary>
    /// Task-related chat
    /// </summary>
    Task = 4
}
