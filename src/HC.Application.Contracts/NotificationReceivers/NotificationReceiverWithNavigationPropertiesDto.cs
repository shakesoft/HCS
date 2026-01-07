using HC.Notifications;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverWithNavigationPropertiesDtoBase
{
    public NotificationReceiverDto NotificationReceiver { get; set; } = null!;
    public NotificationDto Notification { get; set; } = null!;
    public IdentityUserDto IdentityUser { get; set; } = null!;
}