using HC.ProjectTasks;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentWithNavigationPropertiesDtoBase
{
    public ProjectTaskAssignmentDto ProjectTaskAssignment { get; set; } = null!;
    public ProjectTaskDto ProjectTask { get; set; } = null!;
    public IdentityUserDto User { get; set; } = null!;
}