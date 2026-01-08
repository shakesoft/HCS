using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantCreateDtoBase
{
    [Required]
    public ParticipantResponse ResponseStatus { get; set; } = ParticipantResponse.INVITED;
    public bool Notified { get; set; }

    public Guid CalendarEventId { get; set; }

    public Guid IdentityUserId { get; set; }
}