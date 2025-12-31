using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.Departments;

public abstract class DepartmentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }

    public int Level { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public Guid? LeaderUserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}