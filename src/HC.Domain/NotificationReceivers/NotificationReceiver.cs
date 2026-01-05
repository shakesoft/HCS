using HC.Notifications;
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

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual bool IsRead { get; set; }

    public virtual DateTime? ReadAt { get; set; }

    public Guid NotificationId { get; set; }

    public Guid IdentityUserId { get; set; }

    protected NotificationReceiverBase()
    {
    }

    public NotificationReceiverBase(Guid id, Guid notificationId, Guid identityUserId, bool isRead, DateTime? readAt = null)
    {
        Id = id;
        IsRead = isRead;
        ReadAt = readAt;
        NotificationId = notificationId;
        IdentityUserId = identityUserId;
    }
}