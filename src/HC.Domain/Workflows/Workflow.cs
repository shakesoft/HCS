using HC.WorkflowDefinitions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.Workflows;

public abstract class WorkflowBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    [CanBeNull]
    public virtual string? Description { get; set; }

    public virtual bool IsActive { get; set; }

    public Guid WorkflowDefinitionId { get; set; }

    protected WorkflowBase()
    {
    }

    public WorkflowBase(Guid id, Guid workflowDefinitionId, string code, string name, bool isActive, string? description = null)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), WorkflowConsts.CodeMaxLength, WorkflowConsts.CodeMinLength);
        Check.NotNull(name, nameof(name));
        Code = code;
        Name = name;
        IsActive = isActive;
        Description = description;
        WorkflowDefinitionId = workflowDefinitionId;
    }
}