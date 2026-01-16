using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.Chat.Messages;

public interface IMessageFileRepository : IBasicRepository<MessageFile, Guid>
{
    Task<List<MessageFile>> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default);
    
    Task<List<MessageFile>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    
    Task<MessageFile> GetWithMessageAsync(Guid fileId, CancellationToken cancellationToken = default);
}
