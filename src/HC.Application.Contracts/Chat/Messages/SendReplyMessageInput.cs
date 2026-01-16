using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace HC.Chat.Messages;

public class SendReplyMessageInput
{
    public Guid TargetUserId { get; set; } // For Direct
    public Guid? ConversationId { get; set; } // For Group/Project/Task
    public Guid ReplyToMessageId { get; set; }
    
    [Required]
    [DynamicStringLength(typeof(ChatMessageConsts),nameof(ChatMessageConsts.MaxTextLength), nameof(ChatMessageConsts.MinTextLength))]
    public string Message { get; set; }
}
