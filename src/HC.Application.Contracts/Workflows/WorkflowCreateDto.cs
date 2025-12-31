using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Workflows;

public abstract class WorkflowCreateDtoBase
{
    [Required]
    [StringLength(WorkflowConsts.CodeMaxLength, MinimumLength = WorkflowConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}