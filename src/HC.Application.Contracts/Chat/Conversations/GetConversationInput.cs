using System;
using Volo.Abp.Application.Dtos;

namespace HC.Chat.Conversations;

public class GetConversationInput : PagedResultRequestDto
{
    public Guid TargetUserId { get; set; } // For Direct conversations (required for backward compatibility)
    public Guid? ConversationId { get; set; } // For Group/Project/Task conversations (optional)
}
