using Volo.Abp.Localization;
using Volo.Abp.Settings;
using HC.Chat.Messages;

namespace HC.Chat.Settings;

public class ChatSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                ChatSettingNames.Messaging.SendMessageOnEnter,
                true.ToString(),
                isVisibleToClients: true)
        );
        
        context.Add(
            new SettingDefinition(
                ChatSettingNames.Messaging.DeletingMessages,
                ChatDeletingMessages.Enabled.ToString(),
                isVisibleToClients: true)
        );
        
        context.Add(
            new SettingDefinition(
                ChatSettingNames.Messaging.MessageDeletionPeriod,
                0.ToString(),
                isVisibleToClients: true)
        );
        
        context.Add(
            new SettingDefinition(
                ChatSettingNames.Messaging.DeletingConversations,
                ChatDeletingConversations.Enabled.ToString(),
                isVisibleToClients: true)
        );
    }
}
