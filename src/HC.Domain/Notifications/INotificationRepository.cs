using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.Notifications;

public partial interface INotificationRepository : IRepository<Notification, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetListAsync(string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null, CancellationToken cancellationToken = default);
}