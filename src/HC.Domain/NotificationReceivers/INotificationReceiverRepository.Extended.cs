using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HC.NotificationReceivers;

public partial interface INotificationReceiverRepository
{
    Task<List<NotificationReceiverWithNavigationProperties>> GetNotificationsByUserAndReadStatusAsync(
        Guid userId,
        bool isRead,
        string? filterText = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountByUserAndReadStatusAsync(
        Guid userId,
        bool isRead,
        string? filterText = null,
        CancellationToken cancellationToken = default);
}