using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Notifications;

public abstract class NotificationUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Content { get; set; } = null!;
    [Required]
    public SourceType SourceType { get; set; } = SourceType.WORKFLOW;
    [Required]
    public EventType EventType { get; set; } = EventType.WORKFLOW_ASSIGNED;
    [Required]
    public RelatedType RelatedType { get; set; } = RelatedType.DOCUMENT;
    public string? RelatedId { get; set; }

    [Required]
    public string Priority { get; set; } = null!;
    public string ConcurrencyStamp { get; set; } = null!;
}