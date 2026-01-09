using HC.CalendarEvents;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantWithNavigationPropertiesDtoBase
{
    public CalendarEventParticipantDto CalendarEventParticipant { get; set; } = null!;
    public CalendarEventDto CalendarEvent { get; set; } = null!;
    public IdentityUserDto IdentityUser { get; set; } = null!;
}