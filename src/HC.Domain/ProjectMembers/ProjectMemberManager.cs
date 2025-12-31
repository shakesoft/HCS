using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.ProjectMembers;

public abstract class ProjectMemberManagerBase : DomainService
{
    protected IProjectMemberRepository _projectMemberRepository;

    public ProjectMemberManagerBase(IProjectMemberRepository projectMemberRepository)
    {
        _projectMemberRepository = projectMemberRepository;
    }

    public virtual async Task<ProjectMember> CreateAsync(Guid projectId, Guid userId, string memberRole, DateTime joinedAt)
    {
        Check.NotNull(projectId, nameof(projectId));
        Check.NotNull(userId, nameof(userId));
        Check.NotNullOrWhiteSpace(memberRole, nameof(memberRole));
        Check.Length(memberRole, nameof(memberRole), ProjectMemberConsts.MemberRoleMaxLength);
        Check.NotNull(joinedAt, nameof(joinedAt));
        var projectMember = new ProjectMember(GuidGenerator.Create(), projectId, userId, memberRole, joinedAt);
        return await _projectMemberRepository.InsertAsync(projectMember);
    }

    public virtual async Task<ProjectMember> UpdateAsync(Guid id, Guid projectId, Guid userId, string memberRole, DateTime joinedAt, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(projectId, nameof(projectId));
        Check.NotNull(userId, nameof(userId));
        Check.NotNullOrWhiteSpace(memberRole, nameof(memberRole));
        Check.Length(memberRole, nameof(memberRole), ProjectMemberConsts.MemberRoleMaxLength);
        Check.NotNull(joinedAt, nameof(joinedAt));
        var projectMember = await _projectMemberRepository.GetAsync(id);
        projectMember.ProjectId = projectId;
        projectMember.UserId = userId;
        projectMember.MemberRole = memberRole;
        projectMember.JoinedAt = joinedAt;
        projectMember.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _projectMemberRepository.UpdateAsync(projectMember);
    }
}