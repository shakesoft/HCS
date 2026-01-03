using Volo.Abp.Application.Dtos;
using System;

namespace HC.Notifications;

public abstract class NotificationExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public SourceType? SourceType { get; set; }

    public EventType? EventType { get; set; }

    public RelatedType? RelatedType { get; set; }

    public string? RelatedId { get; set; }

    public string? Priority { get; set; }

    public NotificationExcelDownloadDtoBase()
    {
    }
}