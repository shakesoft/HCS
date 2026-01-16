using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HC.Chat;

namespace HC.Chat.Conversations;

public class CreateGroupConversationInput
{
    [Required]
    [StringLength(ChatConsts.MaxConversationNameLength)]
    public string Name { get; set; }
    
    [StringLength(ChatConsts.MaxConversationDescriptionLength)]
    public string Description { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Guid> MemberUserIds { get; set; }
}
