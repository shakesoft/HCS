using System;
using System.ComponentModel.DataAnnotations;
using HC.Chat;

namespace HC.Chat.Conversations;

public class UpdateConversationNameInput
{
    [Required]
    public Guid ConversationId { get; set; }
    
    [Required]
    [StringLength(ChatConsts.MaxConversationNameLength)]
    public string Name { get; set; }
}
