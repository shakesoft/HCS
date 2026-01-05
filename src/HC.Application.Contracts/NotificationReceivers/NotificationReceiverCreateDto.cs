using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverCreateDtoBase
{
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public Guid NotificationId { get; set; }

    public Guid IdentityUserId { get; set; }
}