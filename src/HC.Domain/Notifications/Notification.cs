using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.Notifications;

public abstract class NotificationBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Title { get; set; }

    [NotNull]
    public virtual string Content { get; set; }

    [NotNull]
    public virtual SourceType SourceType { get; set; }

    [NotNull]
    public virtual EventType EventType { get; set; }

    [NotNull]
    public virtual RelatedType RelatedType { get; set; }

    [CanBeNull]
    public virtual string? RelatedId { get; set; }

    [NotNull]
    public virtual string Priority { get; set; }

    protected NotificationBase()
    {
    }

    public NotificationBase(Guid id, string title, string content, SourceType sourceType, EventType eventType, RelatedType relatedType, string priority, string? relatedId = null)
    {
        Id = id;
        Check.NotNull(title, nameof(title));
        Check.NotNull(content, nameof(content));
        Check.NotNull(sourceType, nameof(sourceType));
        Check.NotNull(eventType, nameof(eventType));
        Check.NotNull(relatedType, nameof(relatedType));
        Check.NotNull(priority, nameof(priority));
        Title = title;
        Content = content;
        SourceType = sourceType;
        EventType = eventType;
        RelatedType = relatedType;
        Priority = priority;
        RelatedId = relatedId;
    }
}