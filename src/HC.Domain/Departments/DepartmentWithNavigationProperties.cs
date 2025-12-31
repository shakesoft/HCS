using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.Departments;

namespace HC.Departments;

public abstract class DepartmentWithNavigationPropertiesBase
{
    public Department Department { get; set; } = null!;
    public IdentityUser? LeaderUser { get; set; }
}