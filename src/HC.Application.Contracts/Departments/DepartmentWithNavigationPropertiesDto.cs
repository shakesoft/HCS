using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.Departments;

public abstract class DepartmentWithNavigationPropertiesDtoBase
{
    public DepartmentDto Department { get; set; } = null!;
    public IdentityUserDto? LeaderUser { get; set; }
}