using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Documents;

public abstract class DocumentUpdateDtoBase : IHasConcurrencyStamp
{
    [StringLength(DocumentConsts.NoMaxLength)]
    public string? No { get; set; }

    [Required]
    public string Title { get; set; } = null!;
    [StringLength(DocumentConsts.TypeMaxLength)]
    public string? Type { get; set; }

    [StringLength(DocumentConsts.UrgencyLevelMaxLength)]
    public string? UrgencyLevel { get; set; }

    [StringLength(DocumentConsts.SecrecyLevelMaxLength)]
    public string? SecrecyLevel { get; set; }

    [StringLength(DocumentConsts.CurrentStatusMaxLength)]
    public string? CurrentStatus { get; set; }

    public DateTime CompletedTime { get; set; }

    public Guid? FieldId { get; set; }

    public Guid? UnitId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? StatusId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}