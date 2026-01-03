using System;

namespace HC.Notifications;

public abstract class NotificationExcelDtoBase
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public SourceType SourceType { get; set; } = SourceType.WORKFLOW;
    public EventType EventType { get; set; } = EventType.WORKFLOW_ASSIGNED;
    public RelatedType RelatedType { get; set; } = RelatedType.DOCUMENT;
    public string? RelatedId { get; set; }

    public string Priority { get; set; } = null!;
}