using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using HC.Chat.Conversations;
using HC.Chat.Messages;
using HC.Chat.Users;

namespace HC.Chat.EntityFrameworkCore;

[ConnectionStringName("Default")]
public interface IChatDbContext : IEfCoreDbContext
{
    DbSet<Message> ChatMessages { get; }

    DbSet<ChatUser> ChatUsers { get; }

    DbSet<UserMessage> ChatUserMessages { get; }

    DbSet<Conversation> ChatConversations { get; }
    
    // New DbSets
    DbSet<ConversationMember> ChatConversationMembers { get; }
    
    DbSet<MessageFile> ChatMessageFiles { get; }
}
