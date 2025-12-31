using HC.Projects;
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

namespace HC.ProjectMembers;

public abstract class ProjectMemberBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string MemberRole { get; set; }

    public virtual DateTime JoinedAt { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    protected ProjectMemberBase()
    {
    }

    public ProjectMemberBase(Guid id, Guid projectId, Guid userId, string memberRole, DateTime joinedAt)
    {
        Id = id;
        Check.NotNull(memberRole, nameof(memberRole));
        Check.Length(memberRole, nameof(memberRole), ProjectMemberConsts.MemberRoleMaxLength, 0);
        MemberRole = memberRole;
        JoinedAt = joinedAt;
        ProjectId = projectId;
        UserId = userId;
    }
}