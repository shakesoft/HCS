using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.Departments;

public abstract class DepartmentUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(DepartmentConsts.CodeMaxLength, MinimumLength = DepartmentConsts.CodeMinLength)]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }

    public int Level { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public Guid? LeaderUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}