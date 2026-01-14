using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.UserDepartments;

public abstract class UserDepartmentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public bool IsPrimary { get; set; }

    public bool IsActive { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid UserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}