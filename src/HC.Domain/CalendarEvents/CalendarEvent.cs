using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.CalendarEvents;

public abstract class CalendarEventBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Title { get; set; }

    [CanBeNull]
    public virtual string? Description { get; set; }

    public virtual DateTime StartTime { get; set; }

    public virtual DateTime EndTime { get; set; }

    public virtual bool AllDay { get; set; }

    [NotNull]
    public virtual string EventType { get; set; }

    [CanBeNull]
    public virtual string? Location { get; set; }

    [NotNull]
    public virtual string RelatedType { get; set; }

    [CanBeNull]
    public virtual string? RelatedId { get; set; }

    protected CalendarEventBase()
    {
    }

    public CalendarEventBase(Guid id, string title, DateTime startTime, DateTime endTime, bool allDay, string eventType, string relatedType, string? description = null, string? location = null, string? relatedId = null)
    {
        Id = id;
        Check.NotNull(title, nameof(title));
        Check.NotNull(eventType, nameof(eventType));
        Check.NotNull(relatedType, nameof(relatedType));
        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        AllDay = allDay;
        EventType = eventType;
        RelatedType = relatedType;
        Description = description;
        Location = location;
        RelatedId = relatedId;
    }
}