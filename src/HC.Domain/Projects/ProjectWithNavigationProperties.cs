using HC.Departments;
using System;
using System.Collections.Generic;
using HC.Projects;

namespace HC.Projects;

public abstract class ProjectWithNavigationPropertiesBase
{
    public Project Project { get; set; } = null!;
    public Department? OwnerDepartment { get; set; }
}