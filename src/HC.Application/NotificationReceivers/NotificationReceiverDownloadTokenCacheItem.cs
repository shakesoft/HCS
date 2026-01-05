using System;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}