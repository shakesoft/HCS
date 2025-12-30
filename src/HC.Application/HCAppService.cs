using HC.Localization;
using Volo.Abp.Application.Services;

namespace HC;

/* Inherit your application services from this class.
 */
public abstract class HCAppService : ApplicationService
{
    protected HCAppService()
    {
        LocalizationResource = typeof(HCResource);
    }
}
