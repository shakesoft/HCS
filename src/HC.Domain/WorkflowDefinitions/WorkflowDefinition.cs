using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    [CanBeNull]
    public virtual string? Description { get; set; }

    public virtual bool IsActive { get; set; }

    protected WorkflowDefinitionBase()
    {
    }

    public WorkflowDefinitionBase(Guid id, string code, string name, bool isActive, string? description = null)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), WorkflowDefinitionConsts.CodeMaxLength, WorkflowDefinitionConsts.CodeMinLength);
        Check.NotNull(name, nameof(name));
        Code = code;
        Name = name;
        IsActive = isActive;
        Description = description;
    }
}