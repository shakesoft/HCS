using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.NotificationReceivers;

public abstract class NotificationReceiverDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public Guid NotificationId { get; set; }

    public Guid IdentityUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}