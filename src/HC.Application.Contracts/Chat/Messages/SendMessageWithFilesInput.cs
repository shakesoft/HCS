using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace HC.Chat.Messages;

public class SendMessageWithFilesInput
{
    public Guid TargetUserId { get; set; } // For Direct
    public Guid? ConversationId { get; set; } // For Group/Project/Task
    public string Message { get; set; }
    public List<Guid> FileIds { get; set; } // Uploaded file IDs
}
