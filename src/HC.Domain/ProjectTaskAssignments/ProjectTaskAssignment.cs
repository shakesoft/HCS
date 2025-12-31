using HC.ProjectTasks;
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

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string AssignmentRole { get; set; }

    public virtual DateTime AssignedAt { get; set; }

    [CanBeNull]
    public virtual string? Note { get; set; }

    public Guid ProjectTaskId { get; set; }

    public Guid UserId { get; set; }

    protected ProjectTaskAssignmentBase()
    {
    }

    public ProjectTaskAssignmentBase(Guid id, Guid projectTaskId, Guid userId, string assignmentRole, DateTime assignedAt, string? note = null)
    {
        Id = id;
        Check.NotNull(assignmentRole, nameof(assignmentRole));
        Check.Length(assignmentRole, nameof(assignmentRole), ProjectTaskAssignmentConsts.AssignmentRoleMaxLength, 0);
        AssignmentRole = assignmentRole;
        AssignedAt = assignedAt;
        Note = note;
        ProjectTaskId = projectTaskId;
        UserId = userId;
    }
}