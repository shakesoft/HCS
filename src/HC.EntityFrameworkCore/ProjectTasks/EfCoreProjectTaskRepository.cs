using HC.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using HC.EntityFrameworkCore;

namespace HC.ProjectTasks;

public abstract class EfCoreProjectTaskRepositoryBase : EfCoreRepository<HCDbContext, ProjectTask, Guid>
{
    public EfCoreProjectTaskRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? parentTaskId = null, string? code = null, string? title = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? dueDateMin = null, DateTime? dueDateMax = null, string? priority = null, string? status = null, int? progressPercentMin = null, int? progressPercentMax = null, Guid? projectId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, parentTaskId, code, title, description, startDateMin, startDateMax, dueDateMin, dueDateMax, priority, status, progressPercentMin, progressPercentMax, projectId);
        var ids = query.Select(x => x.ProjectTask.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<ProjectTaskWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var projectTask = await (await GetDbSetAsync()).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (projectTask == null)
        {
            return null;
        }
        
        var project = await dbContext.Set<Project>().FirstOrDefaultAsync(c => c.Id == projectTask.ProjectId && !c.IsDeleted);
        if (project == null)
        {
            return null;
        }
        
        return new ProjectTaskWithNavigationProperties 
        { 
            ProjectTask = projectTask, 
            Project = project 
        };
    }

    public virtual async Task<List<ProjectTaskWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? parentTaskId = null, string? code = null, string? title = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? dueDateMin = null, DateTime? dueDateMax = null, string? priority = null, string? status = null, int? progressPercentMin = null, int? progressPercentMax = null, Guid? projectId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, parentTaskId, code, title, description, startDateMin, startDateMax, dueDateMin, dueDateMax, priority, status, progressPercentMin, progressPercentMax, projectId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProjectTaskConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<ProjectTaskWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        return from projectTask in (await GetDbSetAsync()).Where(x => !x.IsDeleted)
               join project in (await GetDbContextAsync()).Set<Project>().Where(p => !p.IsDeleted) on projectTask.ProjectId equals project.Id into projects
               from project in projects.DefaultIfEmpty()
               select new ProjectTaskWithNavigationProperties
               {
                   ProjectTask = projectTask,
                   Project = project
               };
    }

    protected virtual IQueryable<ProjectTaskWithNavigationProperties> ApplyFilter(IQueryable<ProjectTaskWithNavigationProperties> query, string? filterText, string? parentTaskId = null, string? code = null, string? title = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? dueDateMin = null, DateTime? dueDateMax = null, string? priority = null, string? status = null, int? progressPercentMin = null, int? progressPercentMax = null, Guid? projectId = null)
    {
        return query.Where(e => !e.ProjectTask.IsDeleted && (e.Project == null || !e.Project.IsDeleted))
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ProjectTask.ParentTaskId!.Contains(filterText!) || e.ProjectTask.Code!.Contains(filterText!) || e.ProjectTask.Title!.Contains(filterText!) || e.ProjectTask.Description!.Contains(filterText!) || e.ProjectTask.Priority!.Contains(filterText!) || e.ProjectTask.Status!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(parentTaskId), e => e.ProjectTask.ParentTaskId.Contains(parentTaskId)).WhereIf(!string.IsNullOrWhiteSpace(code), e => e.ProjectTask.Code.Contains(code)).WhereIf(!string.IsNullOrWhiteSpace(title), e => e.ProjectTask.Title.Contains(title)).WhereIf(!string.IsNullOrWhiteSpace(description), e => e.ProjectTask.Description.Contains(description)).WhereIf(startDateMin.HasValue, e => e.ProjectTask.StartDate >= startDateMin!.Value).WhereIf(startDateMax.HasValue, e => e.ProjectTask.StartDate <= startDateMax!.Value).WhereIf(dueDateMin.HasValue, e => e.ProjectTask.DueDate >= dueDateMin!.Value).WhereIf(dueDateMax.HasValue, e => e.ProjectTask.DueDate <= dueDateMax!.Value).WhereIf(!string.IsNullOrWhiteSpace(priority), e => e.ProjectTask.Priority.Contains(priority)).WhereIf(!string.IsNullOrWhiteSpace(status), e => e.ProjectTask.Status.Contains(status)).WhereIf(progressPercentMin.HasValue, e => e.ProjectTask.ProgressPercent >= progressPercentMin!.Value).WhereIf(progressPercentMax.HasValue, e => e.ProjectTask.ProgressPercent <= progressPercentMax!.Value).WhereIf(projectId != null && projectId != Guid.Empty, e => e.Project != null && e.Project.Id == projectId);
    }

    public virtual async Task<List<ProjectTask>> GetListAsync(string? filterText = null, string? parentTaskId = null, string? code = null, string? title = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? dueDateMin = null, DateTime? dueDateMax = null, string? priority = null, string? status = null, int? progressPercentMin = null, int? progressPercentMax = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, parentTaskId, code, title, description, startDateMin, startDateMax, dueDateMin, dueDateMax, priority, status, progressPercentMin, progressPercentMax);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProjectTaskConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? parentTaskId = null, string? code = null, string? title = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? dueDateMin = null, DateTime? dueDateMax = null, string? priority = null, string? status = null, int? progressPercentMin = null, int? progressPercentMax = null, Guid? projectId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, parentTaskId, code, title, description, startDateMin, startDateMax, dueDateMin, dueDateMax, priority, status, progressPercentMin, progressPercentMax, projectId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<ProjectTask> ApplyFilter(IQueryable<ProjectTask> query, string? filterText = null, string? parentTaskId = null, string? code = null, string? title = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? dueDateMin = null, DateTime? dueDateMax = null, string? priority = null, string? status = null, int? progressPercentMin = null, int? progressPercentMax = null)
    {
        return query.Where(e => !e.IsDeleted)
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ParentTaskId!.Contains(filterText!) || e.Code!.Contains(filterText!) || e.Title!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.Priority!.Contains(filterText!) || e.Status!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(parentTaskId), e => e.ParentTaskId.Contains(parentTaskId)).WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code)).WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title)).WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description)).WhereIf(startDateMin.HasValue, e => e.StartDate >= startDateMin!.Value).WhereIf(startDateMax.HasValue, e => e.StartDate <= startDateMax!.Value).WhereIf(dueDateMin.HasValue, e => e.DueDate >= dueDateMin!.Value).WhereIf(dueDateMax.HasValue, e => e.DueDate <= dueDateMax!.Value).WhereIf(!string.IsNullOrWhiteSpace(priority), e => e.Priority.Contains(priority)).WhereIf(!string.IsNullOrWhiteSpace(status), e => e.Status.Contains(status)).WhereIf(progressPercentMin.HasValue, e => e.ProgressPercent >= progressPercentMin!.Value).WhereIf(progressPercentMax.HasValue, e => e.ProgressPercent <= progressPercentMax!.Value);
    }
}