using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionCreateDtoBase
{
    [Required]
    [StringLength(WorkflowDefinitionConsts.CodeMaxLength, MinimumLength = WorkflowDefinitionConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}