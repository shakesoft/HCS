using Volo.Abp.Modularity;

namespace HC;

public abstract class HCApplicationTestBase<TStartupModule> : HCTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
