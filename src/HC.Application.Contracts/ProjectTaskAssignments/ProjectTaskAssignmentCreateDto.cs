using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentCreateDtoBase
{
    [Required]
    [StringLength(ProjectTaskAssignmentConsts.AssignmentRoleMaxLength)]
    public string AssignmentRole { get; set; } = "MAIN";
    public DateTime AssignedAt { get; set; }

    public string? Note { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid UserId { get; set; }
}