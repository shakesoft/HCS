using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.ProjectTasks;

public abstract class ProjectTaskManagerBase : DomainService
{
    protected IProjectTaskRepository _projectTaskRepository;

    public ProjectTaskManagerBase(IProjectTaskRepository projectTaskRepository)
    {
        _projectTaskRepository = projectTaskRepository;
    }

    public virtual async Task<ProjectTask> CreateAsync(Guid projectId, string code, string title, DateTime startDate, DateTime dueDate, string priority, string status, int progressPercent, string? parentTaskId = null, string? description = null)
    {
        Check.NotNull(projectId, nameof(projectId));
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), ProjectTaskConsts.CodeMaxLength);
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.Length(title, nameof(title), ProjectTaskConsts.TitleMaxLength);
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(dueDate, nameof(dueDate));
        Check.NotNullOrWhiteSpace(priority, nameof(priority));
        Check.Length(priority, nameof(priority), ProjectTaskConsts.PriorityMaxLength);
        Check.NotNullOrWhiteSpace(status, nameof(status));
        Check.Length(status, nameof(status), ProjectTaskConsts.StatusMaxLength);
        Check.Range(progressPercent, nameof(progressPercent), ProjectTaskConsts.ProgressPercentMinLength, ProjectTaskConsts.ProgressPercentMaxLength);
        var projectTask = new ProjectTask(GuidGenerator.Create(), projectId, code, title, startDate, dueDate, priority, status, progressPercent, parentTaskId, description);
        return await _projectTaskRepository.InsertAsync(projectTask);
    }

    public virtual async Task<ProjectTask> UpdateAsync(Guid id, Guid projectId, string code, string title, DateTime startDate, DateTime dueDate, string priority, string status, int progressPercent, string? parentTaskId = null, string? description = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(projectId, nameof(projectId));
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), ProjectTaskConsts.CodeMaxLength);
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.Length(title, nameof(title), ProjectTaskConsts.TitleMaxLength);
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(dueDate, nameof(dueDate));
        Check.NotNullOrWhiteSpace(priority, nameof(priority));
        Check.Length(priority, nameof(priority), ProjectTaskConsts.PriorityMaxLength);
        Check.NotNullOrWhiteSpace(status, nameof(status));
        Check.Length(status, nameof(status), ProjectTaskConsts.StatusMaxLength);
        Check.Range(progressPercent, nameof(progressPercent), ProjectTaskConsts.ProgressPercentMinLength, ProjectTaskConsts.ProgressPercentMaxLength);
        var projectTask = await _projectTaskRepository.GetAsync(id);
        projectTask.ProjectId = projectId;
        projectTask.Code = code;
        projectTask.Title = title;
        projectTask.StartDate = startDate;
        projectTask.DueDate = dueDate;
        projectTask.Priority = priority;
        projectTask.Status = status;
        projectTask.ProgressPercent = progressPercent;
        projectTask.ParentTaskId = parentTaskId;
        projectTask.Description = description;
        projectTask.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _projectTaskRepository.UpdateAsync(projectTask);
    }
}