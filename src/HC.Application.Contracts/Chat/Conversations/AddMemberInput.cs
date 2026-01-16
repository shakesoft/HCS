using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HC.Chat.Conversations;

public class AddMemberInput
{
    [Required]
    public Guid ConversationId { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Guid> UserIds { get; set; }
}
