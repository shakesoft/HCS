using System.ComponentModel.DataAnnotations;

namespace HC.Chat.Users;

public class GetContactsInput
{
    public string? Filter { get; set; } = string.Empty;

    public bool IncludeOtherContacts { get; set; } = false;
}
