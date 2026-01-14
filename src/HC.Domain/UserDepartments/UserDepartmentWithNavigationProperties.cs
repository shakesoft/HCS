using HC.Departments;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.UserDepartments;

namespace HC.UserDepartments;

public abstract class UserDepartmentWithNavigationPropertiesBase
{
    public UserDepartment UserDepartment { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
}