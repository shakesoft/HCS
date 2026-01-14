using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public int Order { get; set; }

    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int? SLADays { get; set; }

    public bool AllowReturn { get; set; }

    public bool IsActive { get; set; }

    public Guid WorkflowTemplateId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}