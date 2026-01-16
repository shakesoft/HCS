using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Users.EntityFrameworkCore;
using HC.Chat.Conversations;
using HC.Chat.EntityFrameworkCore.Conversations;
using HC.Chat.EntityFrameworkCore.Messages;
using HC.Chat.EntityFrameworkCore.Users;
using HC.Chat.Messages;
using HC.Chat.Users;

namespace HC.Chat.EntityFrameworkCore;

[DependsOn(
    typeof(HCChatDomainModule),
    typeof(AbpUsersEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class HCChatEntityFrameworkCoreModule : AbpModule
{
    // Note: Chat repositories are registered in HCEntityFrameworkCoreModule
    // This module ensures Chat entities are configured in DbContext
}
