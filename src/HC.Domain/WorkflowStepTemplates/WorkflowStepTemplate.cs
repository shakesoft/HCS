using HC.Workflows;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual int Order { get; set; }

    [NotNull]
    public virtual string Name { get; set; }

    [NotNull]
    public virtual string Type { get; set; }

    public virtual int? SLADays { get; set; }

    public virtual bool AllowReturn { get; set; }

    public virtual bool IsActive { get; set; }

    public Guid WorkflowId { get; set; }

    protected WorkflowStepTemplateBase()
    {
    }

    public WorkflowStepTemplateBase(Guid id, Guid workflowId, int order, string name, string type, bool allowReturn, bool isActive, int? sLADays = null)
    {
        Id = id;
        if (order < WorkflowStepTemplateConsts.OrderMinLength)
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, "The value of 'order' cannot be lower than " + WorkflowStepTemplateConsts.OrderMinLength);
        }

        if (order > WorkflowStepTemplateConsts.OrderMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, "The value of 'order' cannot be greater than " + WorkflowStepTemplateConsts.OrderMaxLength);
        }

        Check.NotNull(name, nameof(name));
        Check.NotNull(type, nameof(type));
        Check.Length(type, nameof(type), WorkflowStepTemplateConsts.TypeMaxLength, WorkflowStepTemplateConsts.TypeMinLength);
        Order = order;
        Name = name;
        Type = type;
        AllowReturn = allowReturn;
        IsActive = isActive;
        SLADays = sLADays;
        WorkflowId = workflowId;
    }
}