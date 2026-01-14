using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateCreateDtoBase
{
    [Required]
    [Range(WorkflowStepTemplateConsts.OrderMinLength, WorkflowStepTemplateConsts.OrderMaxLength)]
    public int Order { get; set; } = 1;
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    [StringLength(WorkflowStepTemplateConsts.TypeMaxLength, MinimumLength = WorkflowStepTemplateConsts.TypeMinLength)]
    public string Type { get; set; } = null!;
    public int? SLADays { get; set; }

    public bool AllowReturn { get; set; }

    public bool IsActive { get; set; }

    public Guid WorkflowTemplateId { get; set; }
}