using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverUpdateDtoBase : IHasConcurrencyStamp
{
    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public Guid NotificationId { get; set; }

    public Guid IdentityUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}