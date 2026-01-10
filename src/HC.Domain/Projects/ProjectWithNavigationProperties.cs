using HC.Departments;
using System;
using System.Collections.Generic;
using HC.Projects;

namespace HC.Projects;

public abstract class ProjectWithNavigationPropertiesBase
{
    public Project Project { get; set; } = null!;
    public Department? OwnerDepartment { get; set; }

    // Navigation aggregates for list views (avoid loading full collections)
    public int ProjectMemberCount { get; set; }
    public int ProjectTaskCount { get; set; }
}