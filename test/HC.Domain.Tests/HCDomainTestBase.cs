using Volo.Abp.Modularity;

namespace HC;

/* Inherit from this class for your domain layer tests. */
public abstract class HCDomainTestBase<TStartupModule> : HCTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
