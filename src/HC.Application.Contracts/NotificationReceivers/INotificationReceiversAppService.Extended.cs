using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HC.NotificationReceivers;

public partial interface INotificationReceiversAppService
{
    Task<PagedResultDto<NotificationReceiverWithNavigationPropertiesDto>> GetReadNotificationsAsync(GetUserNotificationsInput input);
    Task<PagedResultDto<NotificationReceiverWithNavigationPropertiesDto>> GetUnreadNotificationsAsync(GetUserNotificationsInput input);
    //Write your custom code here...
}