using HC.Departments;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.Projects;

public abstract class ProjectWithNavigationPropertiesDtoBase
{
    public ProjectDto Project { get; set; } = null!;
    public DepartmentDto? OwnerDepartment { get; set; }

    // Aggregates for list views (avoid loading full collections)
    public int ProjectMemberCount { get; set; }
    public int ProjectTaskCount { get; set; }
}