using Volo.Abp.Settings;

namespace HC.Settings;

public class HCSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(HCSettings.MySetting1));
    }
}
