using System;

namespace HC.Notifications;

public abstract class NotificationExcelDtoBase
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string SourceType { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string RelatedType { get; set; } = null!;
    public string? RelatedId { get; set; }

    public string Priority { get; set; } = null!;
}