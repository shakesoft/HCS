using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Notifications;

public abstract class NotificationManagerBase : DomainService
{
    protected INotificationRepository _notificationRepository;

    public NotificationManagerBase(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public virtual async Task<Notification> CreateAsync(string title, string content, string sourceType, string eventType, string relatedType, string priority, string? relatedId = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNullOrWhiteSpace(content, nameof(content));
        Check.NotNullOrWhiteSpace(sourceType, nameof(sourceType));
        Check.NotNullOrWhiteSpace(eventType, nameof(eventType));
        Check.NotNullOrWhiteSpace(relatedType, nameof(relatedType));
        Check.NotNullOrWhiteSpace(priority, nameof(priority));
        var notification = new Notification(GuidGenerator.Create(), title, content, sourceType, eventType, relatedType, priority, relatedId);
        return await _notificationRepository.InsertAsync(notification);
    }

    public virtual async Task<Notification> UpdateAsync(Guid id, string title, string content, string sourceType, string eventType, string relatedType, string priority, string? relatedId = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNullOrWhiteSpace(content, nameof(content));
        Check.NotNullOrWhiteSpace(sourceType, nameof(sourceType));
        Check.NotNullOrWhiteSpace(eventType, nameof(eventType));
        Check.NotNullOrWhiteSpace(relatedType, nameof(relatedType));
        Check.NotNullOrWhiteSpace(priority, nameof(priority));
        var notification = await _notificationRepository.GetAsync(id);
        notification.Title = title;
        notification.Content = content;
        notification.SourceType = sourceType;
        notification.EventType = eventType;
        notification.RelatedType = relatedType;
        notification.Priority = priority;
        notification.RelatedId = relatedId;
        notification.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _notificationRepository.UpdateAsync(notification);
    }
}