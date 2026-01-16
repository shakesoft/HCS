using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using HC.Chat.Messages;

namespace HC.Chat.EntityFrameworkCore.Messages;

public class EfCoreMessageFileRepository : EfCoreRepository<IChatDbContext, MessageFile, Guid>, IMessageFileRepository
{
    public EfCoreMessageFileRepository(IDbContextProvider<IChatDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    
    public virtual async Task<List<MessageFile>> GetByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.MessageId == messageId)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<List<MessageFile>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        
        // Get message IDs from the conversation
        var messageIds = await dbContext.ChatUserMessages
            .Where(um => dbContext.ChatConversations.Any(c => 
                ((c.UserId == um.UserId && c.TargetUserId == um.TargetUserId) ||
                 (c.UserId == um.TargetUserId && c.TargetUserId == um.UserId)) &&
                c.Id == conversationId))
            .Select(um => um.ChatMessageId)
            .Distinct()
            .ToListAsync(GetCancellationToken(cancellationToken));
            
        return await (await GetDbSetAsync())
            .Where(x => x.MessageId.HasValue && messageIds.Contains(x.MessageId.Value))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<MessageFile> GetWithMessageAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.Message)
            .FirstOrDefaultAsync(x => x.Id == fileId, GetCancellationToken(cancellationToken));
    }
}
