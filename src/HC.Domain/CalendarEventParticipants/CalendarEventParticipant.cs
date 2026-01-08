using HC.CalendarEvents;
using Volo.Abp.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual ParticipantResponse ResponseStatus { get; set; }

    public virtual bool Notified { get; set; }

    public Guid CalendarEventId { get; set; }

    public Guid IdentityUserId { get; set; }

    protected CalendarEventParticipantBase()
    {
    }

    public CalendarEventParticipantBase(Guid id, Guid calendarEventId, Guid identityUserId, ParticipantResponse responseStatus, bool notified)
    {
        Id = id;
        Check.NotNull(responseStatus, nameof(responseStatus));
        ResponseStatus = responseStatus;
        Notified = notified;
        CalendarEventId = calendarEventId;
        IdentityUserId = identityUserId;
    }
}