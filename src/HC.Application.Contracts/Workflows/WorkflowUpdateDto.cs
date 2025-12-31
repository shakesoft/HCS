using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Workflows;

public abstract class WorkflowUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(WorkflowConsts.CodeMaxLength, MinimumLength = WorkflowConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}