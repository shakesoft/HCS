using System;
using System.Collections.Generic;

namespace HC.Notifications;

/// <summary>
/// Event published when a notification is created
/// </summary>
[Serializable]
public class NotificationCreatedEto
{
    public Guid NotificationId { get; set; }
    public List<Guid> ReceiverUserIds { get; set; } = new();
}
