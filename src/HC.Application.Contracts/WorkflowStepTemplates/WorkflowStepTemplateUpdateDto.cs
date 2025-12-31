using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [Range(WorkflowStepTemplateConsts.OrderMinLength, WorkflowStepTemplateConsts.OrderMaxLength)]
    public int Order { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    [Required]
    [StringLength(WorkflowStepTemplateConsts.TypeMaxLength, MinimumLength = WorkflowStepTemplateConsts.TypeMinLength)]
    public string Type { get; set; } = null!;
    public int? SLADays { get; set; }

    public bool AllowReturn { get; set; }

    public bool IsActive { get; set; }

    public Guid WorkflowId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}