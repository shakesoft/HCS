using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;
using HC.Localization;

namespace HC.Chat;

[DependsOn(
    typeof(HCDomainSharedModule),
    typeof(AbpDddDomainModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpUsersDomainModule)
    )]
public class HCChatDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("HC.Chat", typeof(HCResource));
        });
    }
}
