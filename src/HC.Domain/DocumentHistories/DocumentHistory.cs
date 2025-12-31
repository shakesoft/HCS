using HC.Documents;
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

namespace HC.DocumentHistories;

public abstract class DocumentHistoryBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [CanBeNull]
    public virtual string? Comment { get; set; }

    [NotNull]
    public virtual string Action { get; set; }

    public Guid DocumentId { get; set; }

    public Guid? FromUser { get; set; }

    public Guid ToUser { get; set; }

    protected DocumentHistoryBase()
    {
    }

    public DocumentHistoryBase(Guid id, Guid documentId, Guid? fromUser, Guid toUser, string action, string? comment = null)
    {
        Id = id;
        Check.NotNull(action, nameof(action));
        Check.Length(action, nameof(action), DocumentHistoryConsts.ActionMaxLength, 0);
        Action = action;
        Comment = comment;
        DocumentId = documentId;
        FromUser = fromUser;
        ToUser = toUser;
    }
}