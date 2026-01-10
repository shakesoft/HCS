using Volo.Abp.Application.Dtos;
using System;

namespace HC.CalendarEventParticipants;

public abstract class GetCalendarEventParticipantsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? ResponseStatus { get; set; }

    public bool? Notified { get; set; }

    public Guid? CalendarEventId { get; set; }

    public Guid? IdentityUserId { get; set; }

    public GetCalendarEventParticipantsInputBase()
    {
    }
}