using Volo.Abp.Modularity;

namespace HC;

[DependsOn(
    typeof(HCApplicationModule),
    typeof(HCDomainTestModule)
)]
public class HCApplicationTestModule : AbpModule
{

}
