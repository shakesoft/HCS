using System.ComponentModel.DataAnnotations;

namespace HC.Chat.Users;

public class GetContactsInput
{
    public string? Filter { get; set; } = string.Empty;

    public bool IncludeOtherContacts { get; set; } = false;
    
    public int SkipCount { get; set; } = 0;
    
    public int MaxResultCount { get; set; } = 15; // Default load 15 conversations
}
