using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.UserDepartments;

public abstract class UserDepartmentCreateDtoBase
{
    public bool IsPrimary { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public Guid DepartmentId { get; set; }

    public Guid UserId { get; set; }
}