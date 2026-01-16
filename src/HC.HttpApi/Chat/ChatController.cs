using Volo.Abp.AspNetCore.Mvc;
using HC.Localization;

namespace HC.Chat;

public abstract class ChatController : AbpControllerBase
{
    protected ChatController()
    {
        LocalizationResource = typeof(HCResource);
    }
}
