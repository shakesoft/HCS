using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HC.Chat;

namespace HC.Chat.Conversations;

public class CreateTaskConversationInput
{
    [Required]
    public Guid TaskId { get; set; }
    
    [StringLength(ChatConsts.MaxConversationNameLength)]
    public string Name { get; set; }
    
    public List<Guid> MemberUserIds { get; set; }
}
