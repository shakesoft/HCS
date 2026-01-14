using HC.Departments;
using Volo.Abp.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.UserDepartments;

public abstract class UserDepartmentBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual bool IsPrimary { get; set; }

    public virtual bool IsActive { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid UserId { get; set; }

    protected UserDepartmentBase()
    {
    }

    public UserDepartmentBase(Guid id, Guid departmentId, Guid userId, bool isPrimary, bool isActive)
    {
        Id = id;
        IsPrimary = isPrimary;
        IsActive = isActive;
        DepartmentId = departmentId;
        UserId = userId;
    }
}