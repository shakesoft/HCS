using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentManagerBase : DomainService
{
    protected IProjectTaskAssignmentRepository _projectTaskAssignmentRepository;

    public ProjectTaskAssignmentManagerBase(IProjectTaskAssignmentRepository projectTaskAssignmentRepository)
    {
        _projectTaskAssignmentRepository = projectTaskAssignmentRepository;
    }

    public virtual async Task<ProjectTaskAssignment> CreateAsync(Guid projectTaskId, Guid userId, string assignmentRole, DateTime assignedAt, string? note = null)
    {
        Check.NotNull(projectTaskId, nameof(projectTaskId));
        Check.NotNull(userId, nameof(userId));
        Check.NotNullOrWhiteSpace(assignmentRole, nameof(assignmentRole));
        Check.Length(assignmentRole, nameof(assignmentRole), ProjectTaskAssignmentConsts.AssignmentRoleMaxLength);
        Check.NotNull(assignedAt, nameof(assignedAt));
        var projectTaskAssignment = new ProjectTaskAssignment(GuidGenerator.Create(), projectTaskId, userId, assignmentRole, assignedAt, note);
        return await _projectTaskAssignmentRepository.InsertAsync(projectTaskAssignment);
    }

    public virtual async Task<ProjectTaskAssignment> UpdateAsync(Guid id, Guid projectTaskId, Guid userId, string assignmentRole, DateTime assignedAt, string? note = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(projectTaskId, nameof(projectTaskId));
        Check.NotNull(userId, nameof(userId));
        Check.NotNullOrWhiteSpace(assignmentRole, nameof(assignmentRole));
        Check.Length(assignmentRole, nameof(assignmentRole), ProjectTaskAssignmentConsts.AssignmentRoleMaxLength);
        Check.NotNull(assignedAt, nameof(assignedAt));
        var projectTaskAssignment = await _projectTaskAssignmentRepository.GetAsync(id);
        projectTaskAssignment.ProjectTaskId = projectTaskId;
        projectTaskAssignment.UserId = userId;
        projectTaskAssignment.AssignmentRole = assignmentRole;
        projectTaskAssignment.AssignedAt = assignedAt;
        projectTaskAssignment.Note = note;
        projectTaskAssignment.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _projectTaskAssignmentRepository.UpdateAsync(projectTaskAssignment);
    }
}