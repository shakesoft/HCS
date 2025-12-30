using Volo.Abp.Settings;

namespace HC;

public class HCSettingContributor : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        var leptonxThemeApplicationLayoutSetting = context.GetOrNull("Volo.Abp.LeptonXTheme.ApplicationLayout");
        if (leptonxThemeApplicationLayoutSetting != null)
        {
            leptonxThemeApplicationLayoutSetting.DefaultValue = "TopMenu";
        }
    }
}