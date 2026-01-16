using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using HC.Chat.Conversations;
using HC.Chat.Messages;

namespace HC.Chat.EntityFrameworkCore.Conversations;

public class EfCoreConversationRepository : EfCoreRepository<IChatDbContext, Conversation, Guid>, IConversationRepository
{
    public EfCoreConversationRepository(IDbContextProvider<IChatDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task<ConversationPair> FindPairAsync(Guid senderId, Guid targetId, CancellationToken cancellationToken = default)
    {
        var matchedConversations = await (await GetDbSetAsync())
            .Where(x => (x.UserId == senderId && x.TargetUserId == targetId) ||
                        (x.UserId == targetId && x.TargetUserId == senderId)).ToListAsync(GetCancellationToken(cancellationToken));

        if (!matchedConversations.Any())
        {
            return null;
        }

        return new ConversationPair
        {
            SenderConversation = matchedConversations.FirstOrDefault(x => x.UserId == senderId),
            TargetConversation = matchedConversations.FirstOrDefault(x => x.UserId == targetId)
        };
    }

    public virtual async Task<List<ConversationWithTargetUser>> GetListByUserIdAsync(Guid userId, string filter,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var conversations = await GetDbSetAsync();
        var conversationMembers = dbContext.ChatConversationMembers;
        
        // Get conversations in two ways:
        // 1. Direct conversations: where UserId == userId (old way, for backward compatibility)
        // 2. Group/Project/Task conversations: where user is a member via ConversationMember
        var directConversationsQuery = from chatConversation in conversations
                    join targetUser in dbContext.ChatUsers 
                        on chatConversation.TargetUserId equals targetUser.Id into userGroup
                    from targetUser in userGroup.DefaultIfEmpty()
                    where userId == chatConversation.UserId && chatConversation.Type == ConversationType.Direct
                    select new { chatConversation, targetUser };
        
        // Get group/project/task conversations where user is a member
        var groupConversationsQuery = from member in conversationMembers
                    join conversation in conversations on member.ConversationId equals conversation.Id
                    join targetUser in dbContext.ChatUsers 
                        on conversation.TargetUserId equals targetUser.Id into userGroup
                    from targetUser in userGroup.DefaultIfEmpty()
                    where member.UserId == userId && member.IsActive 
                        && conversation.Type != ConversationType.Direct
                    select new { chatConversation = conversation, targetUser };
        
        // Combine both queries
        var combinedQuery = directConversationsQuery.Union(groupConversationsQuery);
        
        // Apply filter if provided
        if (!string.IsNullOrWhiteSpace(filter))
        {
            combinedQuery = combinedQuery.Where(x => 
                (x.targetUser != null && 
                 (x.targetUser.Name != null && x.targetUser.Name.Contains(filter) || 
                  x.targetUser.Surname != null && x.targetUser.Surname.Contains(filter) || 
                  x.targetUser.UserName != null && x.targetUser.UserName.Contains(filter))) ||
                (x.targetUser == null && 
                 x.chatConversation.Name != null && x.chatConversation.Name.Contains(filter)));
        }
        
        // Execute query and map to result
        var results = await combinedQuery
            .OrderByDescending(x => x.chatConversation.LastMessageDate)
            .ToListAsync(GetCancellationToken(cancellationToken));
        
        return results.Select(x => new ConversationWithTargetUser
        {
            Conversation = x.chatConversation,
            TargetUser = x.targetUser
        }).ToList();
    }

    public virtual async Task<int> GetTotalUnreadMessageCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var conversations = await GetQueryableAsync();
        var conversationMembers = dbContext.ChatConversationMembers;
        
        // Get unread count from:
        // 1. Direct conversations: where UserId == userId
        var directUnreadCount = await conversations
            .Where(x => x.UserId == userId && x.Type == ConversationType.Direct && x.LastMessageSide == ChatMessageSide.Receiver)
            .SumAsync(x => x.UnreadMessageCount, cancellationToken: GetCancellationToken(cancellationToken));
        
        // 2. Group/Project/Task conversations: where user is a member via ConversationMember
        var groupUnreadCount = await (from member in conversationMembers
                    join conversation in conversations on member.ConversationId equals conversation.Id
                    where member.UserId == userId && member.IsActive 
                        && conversation.Type != ConversationType.Direct
                        && conversation.LastMessageSide == ChatMessageSide.Receiver
                    select conversation.UnreadMessageCount)
            .SumAsync(GetCancellationToken(cancellationToken));
        
        return directUnreadCount + groupUnreadCount;
    }
    
    // New methods
    public virtual async Task<List<Conversation>> GetByTypeAsync(
        Guid userId, 
        ConversationType type, 
        bool includePinned = false,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryableAsync())
            .Where(x => x.UserId == userId && x.Type == type);
            
        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<Conversation> GetWithMembersAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == conversationId, GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<bool> IsUserMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.ChatConversationMembers
            .AnyAsync(x => x.ConversationId == conversationId && x.UserId == userId && x.IsActive, GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<List<Conversation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
    
    public virtual async Task<List<Conversation>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.TaskId == taskId)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
