using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string ResponseStatus { get; set; } = null!;
    public bool Notified { get; set; }

    public Guid CalendarEventId { get; set; }

    public Guid IdentityUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}