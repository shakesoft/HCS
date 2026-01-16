using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HC.Chat.Settings;

public interface ISettingsAppService : IApplicationService
{
    Task SetSendOnEnterSettingAsync(SendOnEnterSettingDto input);

    Task<ChatSettingsDto> GetAsync();

    Task UpdateAsync(ChatSettingsDto input);
}
