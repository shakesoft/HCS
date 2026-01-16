using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HC.Chat;

namespace HC.Chat.Conversations;

public class CreateProjectConversationInput
{
    [Required]
    public Guid ProjectId { get; set; }
    
    [StringLength(ChatConsts.MaxConversationNameLength)]
    public string Name { get; set; }
    
    public List<Guid> MemberUserIds { get; set; }
}
