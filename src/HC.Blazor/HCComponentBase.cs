using HC.Localization;
using Volo.Abp.AspNetCore.Components;

namespace HC.Blazor;

public abstract class HCComponentBase : AbpComponentBase
{
    protected HCComponentBase()
    {
        LocalizationResource = typeof(HCResource);
    }
}
