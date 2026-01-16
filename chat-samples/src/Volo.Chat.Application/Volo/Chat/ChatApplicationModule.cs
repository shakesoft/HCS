using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Volo.Chat;

[DependsOn(
    typeof(ChatDomainModule),
    typeof(ChatApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpPermissionManagementDomainSharedModule)
    )]
public class ChatApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<ChatApplicationModule>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        LicenseChecker.Check<ChatApplicationModule>(context);
    }
}
