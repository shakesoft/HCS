using HC.MasterDatas;
using HC.Units;
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

namespace HC.Documents;

public abstract class DocumentBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [CanBeNull]
    public virtual string? No { get; set; }

    [NotNull]
    public virtual string Title { get; set; }

    [CanBeNull]
    public virtual string? Type { get; set; }

    [CanBeNull]
    public virtual string? UrgencyLevel { get; set; }

    [CanBeNull]
    public virtual string? SecrecyLevel { get; set; }

    [CanBeNull]
    public virtual string? CurrentStatus { get; set; }

    public virtual DateTime CompletedTime { get; set; }

    public Guid? FieldId { get; set; }

    public Guid? UnitId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? StatusId { get; set; }

    protected DocumentBase()
    {
    }

    public DocumentBase(Guid id, Guid? fieldId, Guid? unitId, Guid? workflowId, Guid? statusId, string title, DateTime completedTime, string? no = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null)
    {
        Id = id;
        Check.NotNull(title, nameof(title));
        Check.Length(no, nameof(no), DocumentConsts.NoMaxLength, 0);
        Check.Length(type, nameof(type), DocumentConsts.TypeMaxLength, 0);
        Check.Length(urgencyLevel, nameof(urgencyLevel), DocumentConsts.UrgencyLevelMaxLength, 0);
        Check.Length(secrecyLevel, nameof(secrecyLevel), DocumentConsts.SecrecyLevelMaxLength, 0);
        Check.Length(currentStatus, nameof(currentStatus), DocumentConsts.CurrentStatusMaxLength, 0);
        Title = title;
        CompletedTime = completedTime;
        No = no;
        Type = type;
        UrgencyLevel = urgencyLevel;
        SecrecyLevel = secrecyLevel;
        CurrentStatus = currentStatus;
        FieldId = fieldId;
        UnitId = unitId;
        WorkflowId = workflowId;
        StatusId = statusId;
    }
}