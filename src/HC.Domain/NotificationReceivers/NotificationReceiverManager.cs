using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverManagerBase : DomainService
{
    protected INotificationReceiverRepository _notificationReceiverRepository;

    public NotificationReceiverManagerBase(INotificationReceiverRepository notificationReceiverRepository)
    {
        _notificationReceiverRepository = notificationReceiverRepository;
    }

    public virtual async Task<NotificationReceiver> CreateAsync(Guid notificationId, Guid identityUserId, bool isRead, DateTime? readAt = null)
    {
        Check.NotNull(notificationId, nameof(notificationId));
        Check.NotNull(identityUserId, nameof(identityUserId));
        var notificationReceiver = new NotificationReceiver(GuidGenerator.Create(), notificationId, identityUserId, isRead, readAt);
        return await _notificationReceiverRepository.InsertAsync(notificationReceiver);
    }

    public virtual async Task<NotificationReceiver> UpdateAsync(Guid id, Guid notificationId, Guid identityUserId, bool isRead, DateTime? readAt = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(notificationId, nameof(notificationId));
        Check.NotNull(identityUserId, nameof(identityUserId));
        var notificationReceiver = await _notificationReceiverRepository.GetAsync(id);
        notificationReceiver.NotificationId = notificationId;
        notificationReceiver.IdentityUserId = identityUserId;
        notificationReceiver.IsRead = isRead;
        notificationReceiver.ReadAt = readAt;
        notificationReceiver.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _notificationReceiverRepository.UpdateAsync(notificationReceiver);
    }
}