using Volo.Abp.Modularity;

namespace HC;

[DependsOn(
    typeof(HCDomainModule),
    typeof(HCTestBaseModule)
)]
public class HCDomainTestModule : AbpModule
{

}
