using HC.Notifications;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.NotificationReceivers;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverWithNavigationPropertiesBase
{
    public NotificationReceiver NotificationReceiver { get; set; } = null!;
    public Notification Notification { get; set; } = null!;
    public IdentityUser IdentityUser { get; set; } = null!;
}