using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(ProjectTaskAssignmentConsts.AssignmentRoleMaxLength)]
    public string AssignmentRole { get; set; }

    public DateTime AssignedAt { get; set; }

    public string? Note { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid UserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}