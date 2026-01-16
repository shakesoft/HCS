using Localization.Resources.AbpUi;
using Volo.Abp;
using HC.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace HC.Chat;

[DependsOn(
    typeof(HCChatApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class HCChatHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(HCChatHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<HCResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
