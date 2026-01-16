using System;
using System.ComponentModel.DataAnnotations;

namespace HC.Chat.Conversations;

public class RemoveMemberInput
{
    [Required]
    public Guid ConversationId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
}
