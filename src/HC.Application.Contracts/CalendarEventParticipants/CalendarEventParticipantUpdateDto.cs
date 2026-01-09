using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    public ParticipantResponse ResponseStatus { get; set; } = ParticipantResponse.INVITED;
    public bool Notified { get; set; }

    public Guid CalendarEventId { get; set; }

    public Guid IdentityUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}