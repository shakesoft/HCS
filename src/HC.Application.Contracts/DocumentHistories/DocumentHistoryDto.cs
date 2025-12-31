using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string? Comment { get; set; }

    public string Action { get; set; }

    public Guid DocumentId { get; set; }

    public Guid? FromUser { get; set; }

    public Guid ToUser { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}