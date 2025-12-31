using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public bool IsPrimary { get; set; }

    public bool IsActive { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? StepId { get; set; }

    public Guid? TemplateId { get; set; }

    public Guid? DefaultUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}