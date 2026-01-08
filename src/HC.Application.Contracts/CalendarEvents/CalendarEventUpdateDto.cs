using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.CalendarEvents;

public abstract class CalendarEventUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool AllDay { get; set; }

    [Required]
    public EventType EventType { get; set; } = EventType.MEETING;
    public string? Location { get; set; }

    [Required]
    public RelatedType RelatedType { get; set; } = RelatedType.NONE;
    public string? RelatedId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}