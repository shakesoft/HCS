using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantCreateDtoBase
{
    [Required]
    public string ResponseStatus { get; set; } = null!;
    public bool Notified { get; set; }

    public Guid CalendarEventId { get; set; }

    public Guid IdentityUserId { get; set; }
}