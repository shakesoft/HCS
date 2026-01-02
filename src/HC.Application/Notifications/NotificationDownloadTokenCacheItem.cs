using System;

namespace HC.Notifications;

public abstract class NotificationDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}