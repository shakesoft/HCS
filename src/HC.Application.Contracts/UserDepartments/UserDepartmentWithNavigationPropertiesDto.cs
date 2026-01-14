using HC.Departments;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.UserDepartments;

public abstract class UserDepartmentWithNavigationPropertiesDtoBase
{
    public UserDepartmentDto UserDepartment { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
    public IdentityUserDto User { get; set; } = null!;
}