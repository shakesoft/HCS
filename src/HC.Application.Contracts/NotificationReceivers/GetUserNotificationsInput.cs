using Volo.Abp.Application.Dtos;

namespace HC.NotificationReceivers;

public class GetUserNotificationsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
}

