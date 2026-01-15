using System;
using Volo.Abp.Bundling;

namespace Volo.Chat.Blazor;

[Obsolete("This class is obsolete and will be removed in the future versions. Use GlobalAssets instead.")]
public class ChatBundleContributor: IBundleContributor
{
    public void AddScripts(BundleContext context)
    {
        context.Add("_content/Volo.Chat.Blazor/libs/AvatarManager.js");
    }

    public void AddStyles(BundleContext context)
    {

    }
}
