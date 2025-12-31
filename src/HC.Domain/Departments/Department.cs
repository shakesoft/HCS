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

namespace HC.Departments;

public abstract class DepartmentBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    [CanBeNull]
    public virtual string? ParentId { get; set; }

    public virtual int Level { get; set; }

    public virtual int SortOrder { get; set; }

    public virtual bool IsActive { get; set; }

    public Guid? LeaderUserId { get; set; }

    protected DepartmentBase()
    {
    }

    public DepartmentBase(Guid id, Guid? leaderUserId, string code, string name, int level, int sortOrder, bool isActive, string? parentId = null)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), DepartmentConsts.CodeMaxLength, DepartmentConsts.CodeMinLength);
        Check.NotNull(name, nameof(name));
        Code = code;
        Name = name;
        Level = level;
        SortOrder = sortOrder;
        IsActive = isActive;
        ParentId = parentId;
        LeaderUserId = leaderUserId;
    }
}