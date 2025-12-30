using HC.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace HC.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class HCController : AbpControllerBase
{
    protected HCController()
    {
        LocalizationResource = typeof(HCResource);
    }
}
