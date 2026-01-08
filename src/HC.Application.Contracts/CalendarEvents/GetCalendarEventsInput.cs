using Volo.Abp.Application.Dtos;
using System;

namespace HC.CalendarEvents;

public abstract class GetCalendarEventsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? StartTimeMin { get; set; }

    public DateTime? StartTimeMax { get; set; }

    public DateTime? EndTimeMin { get; set; }

    public DateTime? EndTimeMax { get; set; }

    public bool? AllDay { get; set; }

    public EventType? EventType { get; set; }

    public string? Location { get; set; }

    public RelatedType? RelatedType { get; set; }

    public string? RelatedId { get; set; }

    public GetCalendarEventsInputBase()
    {
    }
}