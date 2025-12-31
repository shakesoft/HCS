using HC.ProjectTasks;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.ProjectTaskAssignments;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentWithNavigationPropertiesBase
{
    public ProjectTaskAssignment ProjectTaskAssignment { get; set; } = null!;
    public ProjectTask ProjectTask { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
}