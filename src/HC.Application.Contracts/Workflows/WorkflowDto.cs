using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.Workflows;

public abstract class WorkflowDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public Guid WorkflowDefinitionId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}