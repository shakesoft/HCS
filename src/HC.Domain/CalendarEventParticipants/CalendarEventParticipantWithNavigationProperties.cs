using HC.CalendarEvents;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.CalendarEventParticipants;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantWithNavigationPropertiesBase
{
    public CalendarEventParticipant CalendarEventParticipant { get; set; } = null!;
    public CalendarEvent CalendarEvent { get; set; } = null!;
    public IdentityUser IdentityUser { get; set; } = null!;
}