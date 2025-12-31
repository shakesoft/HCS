using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.Units;

public abstract class UnitBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    public virtual int SortOrder { get; set; }

    public virtual bool IsActive { get; set; }

    protected UnitBase()
    {
    }

    public UnitBase(Guid id, string code, string name, int sortOrder, bool isActive)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), UnitConsts.CodeMaxLength, UnitConsts.CodeMinLength);
        Check.NotNull(name, nameof(name));
        Code = code;
        Name = name;
        SortOrder = sortOrder;
        IsActive = isActive;
    }
}