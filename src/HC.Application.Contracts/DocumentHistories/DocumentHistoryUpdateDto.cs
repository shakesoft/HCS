using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryUpdateDtoBase : IHasConcurrencyStamp
{
    public string? Comment { get; set; }

    [Required]
    [StringLength(DocumentHistoryConsts.ActionMaxLength)]
    public string Action { get; set; }

    public Guid DocumentId { get; set; }

    public Guid? FromUser { get; set; }

    public Guid ToUser { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}