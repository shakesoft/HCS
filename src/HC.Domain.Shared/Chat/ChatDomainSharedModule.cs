using Volo.Abp.Features;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace HC.Chat;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule)
    )]
public class HCChatDomainSharedModule : AbpModule
{
    // Note: HCResource is already registered in HCDomainSharedModule
    // No need to register it again here to avoid duplicate resource error
}
