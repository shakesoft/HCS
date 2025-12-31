using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.Positions;

public abstract class PositionBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    public virtual int SignOrder { get; set; }

    public virtual bool IsActive { get; set; }

    protected PositionBase()
    {
    }

    public PositionBase(Guid id, string code, string name, int signOrder, bool isActive)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), PositionConsts.CodeMaxLength, PositionConsts.CodeMinLength);
        Check.NotNull(name, nameof(name));
        if (signOrder < PositionConsts.SignOrderMinLength)
        {
            throw new ArgumentOutOfRangeException(nameof(signOrder), signOrder, "The value of 'signOrder' cannot be lower than " + PositionConsts.SignOrderMinLength);
        }

        if (signOrder > PositionConsts.SignOrderMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(signOrder), signOrder, "The value of 'signOrder' cannot be greater than " + PositionConsts.SignOrderMaxLength);
        }

        Code = code;
        Name = name;
        SignOrder = signOrder;
        IsActive = isActive;
    }
}