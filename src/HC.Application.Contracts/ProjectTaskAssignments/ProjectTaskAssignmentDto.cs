using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string AssignmentRole { get; set; }

    public DateTime AssignedAt { get; set; }

    public string? Note { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid UserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}