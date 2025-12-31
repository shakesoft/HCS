using HC.ProjectTasks;
using HC.Documents;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string DocumentPurpose { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid DocumentId { get; set; }

    protected ProjectTaskDocumentBase()
    {
    }

    public ProjectTaskDocumentBase(Guid id, Guid projectTaskId, Guid documentId, string documentPurpose)
    {
        Id = id;
        Check.NotNull(documentPurpose, nameof(documentPurpose));
        Check.Length(documentPurpose, nameof(documentPurpose), ProjectTaskDocumentConsts.DocumentPurposeMaxLength, 0);
        DocumentPurpose = documentPurpose;
        ProjectTaskId = projectTaskId;
        DocumentId = documentId;
    }
}