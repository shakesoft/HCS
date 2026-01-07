using Volo.Abp.Application.Dtos;
using System;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadAtMin { get; set; }

    public DateTime? ReadAtMax { get; set; }

    public Guid? NotificationId { get; set; }

    public Guid? IdentityUserId { get; set; }

    public NotificationReceiverExcelDownloadDtoBase()
    {
    }
}