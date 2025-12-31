using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Departments;

public abstract class DepartmentCreateDtoBase
{
    [Required]
    [StringLength(DepartmentConsts.CodeMaxLength, MinimumLength = DepartmentConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }

    public int Level { get; set; }

    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public Guid? LeaderUserId { get; set; }
}