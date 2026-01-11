using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.Notifications;

public abstract class NotificationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string SourceType { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string RelatedType { get; set; } = null!;
    public string? RelatedId { get; set; }

    public string Priority { get; set; } = null!;
    public string ConcurrencyStamp { get; set; } = null!;
}