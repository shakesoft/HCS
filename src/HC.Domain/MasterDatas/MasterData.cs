using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.MasterDatas;

public abstract class MasterDataBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Type { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    public virtual int SortOrder { get; set; }

    public virtual bool IsActive { get; set; }

    protected MasterDataBase()
    {
    }

    public MasterDataBase(Guid id, string type, string code, string name, int sortOrder, bool isActive)
    {
        Id = id;
        Check.NotNull(type, nameof(type));
        Check.Length(type, nameof(type), MasterDataConsts.TypeMaxLength, MasterDataConsts.TypeMinLength);
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), MasterDataConsts.CodeMaxLength, MasterDataConsts.CodeMinLength);
        Check.NotNull(name, nameof(name));
        if (sortOrder < MasterDataConsts.SortOrderMinLength)
        {
            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, "The value of 'sortOrder' cannot be lower than " + MasterDataConsts.SortOrderMinLength);
        }

        if (sortOrder > MasterDataConsts.SortOrderMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, "The value of 'sortOrder' cannot be greater than " + MasterDataConsts.SortOrderMaxLength);
        }

        Type = type;
        Code = code;
        Name = name;
        SortOrder = sortOrder;
        IsActive = isActive;
    }
}