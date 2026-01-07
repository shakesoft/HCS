using System;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverExcelDtoBase
{
    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
}