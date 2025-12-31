using HC.Departments;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.Projects;

public abstract class ProjectWithNavigationPropertiesDtoBase
{
    public ProjectDto Project { get; set; } = null!;
    public DepartmentDto? OwnerDepartment { get; set; }
}