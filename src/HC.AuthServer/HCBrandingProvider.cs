using Microsoft.Extensions.Localization;
using HC.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace HC;

[Dependency(ReplaceServices = true)]
public class HCBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<HCResource> _localizer;

    public HCBrandingProvider(IStringLocalizer<HCResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
    public override string LogoUrl => "https://axis.benhvien199.vn/assets/a3b86cb3-f552-4a4f-a5f0-0cee73b363db";
}
