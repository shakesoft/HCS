using HC.Projects;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.ProjectTasks;

public abstract class ProjectTaskBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [CanBeNull]
    public virtual string? ParentTaskId { get; set; }

    [NotNull]
    public virtual string Code { get; set; }

    [NotNull]
    public virtual string Title { get; set; }

    [CanBeNull]
    public virtual string? Description { get; set; }

    public virtual DateTime StartDate { get; set; }

    public virtual DateTime DueDate { get; set; }

    [NotNull]
    public virtual string Priority { get; set; }

    [NotNull]
    public virtual string Status { get; set; }

    public virtual int ProgressPercent { get; set; }

    public Guid ProjectId { get; set; }

    protected ProjectTaskBase()
    {
    }

    public ProjectTaskBase(Guid id, Guid projectId, string code, string title, DateTime startDate, DateTime dueDate, string priority, string status, int progressPercent, string? parentTaskId = null, string? description = null)
    {
        Id = id;
        Check.NotNull(code, nameof(code));
        Check.Length(code, nameof(code), ProjectTaskConsts.CodeMaxLength, 0);
        Check.NotNull(title, nameof(title));
        Check.Length(title, nameof(title), ProjectTaskConsts.TitleMaxLength, 0);
        Check.NotNull(priority, nameof(priority));
        Check.Length(priority, nameof(priority), ProjectTaskConsts.PriorityMaxLength, 0);
        Check.NotNull(status, nameof(status));
        Check.Length(status, nameof(status), ProjectTaskConsts.StatusMaxLength, 0);
        if (progressPercent < ProjectTaskConsts.ProgressPercentMinLength)
        {
            throw new ArgumentOutOfRangeException(nameof(progressPercent), progressPercent, "The value of 'progressPercent' cannot be lower than " + ProjectTaskConsts.ProgressPercentMinLength);
        }

        if (progressPercent > ProjectTaskConsts.ProgressPercentMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(progressPercent), progressPercent, "The value of 'progressPercent' cannot be greater than " + ProjectTaskConsts.ProgressPercentMaxLength);
        }

        Code = code;
        Title = title;
        StartDate = startDate;
        DueDate = dueDate;
        Priority = priority;
        Status = status;
        ProgressPercent = progressPercent;
        ParentTaskId = parentTaskId;
        Description = description;
        ProjectId = projectId;
    }
}