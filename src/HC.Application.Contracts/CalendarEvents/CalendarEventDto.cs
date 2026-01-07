using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.CalendarEvents;

public abstract class CalendarEventDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool AllDay { get; set; }

    public string EventType { get; set; } = null!;
    public string? Location { get; set; }

    public string RelatedType { get; set; } = null!;
    public string? RelatedId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}