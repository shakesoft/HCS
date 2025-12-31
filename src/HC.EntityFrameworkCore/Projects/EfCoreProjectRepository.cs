using HC.Departments;
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

namespace HC.Projects;

public abstract class EfCoreProjectRepositoryBase : EfCoreRepository<HCDbContext, Project, Guid>
{
    public EfCoreProjectRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? endDateMin = null, DateTime? endDateMax = null, string? status = null, Guid? ownerDepartmentId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, code, name, description, startDateMin, startDateMax, endDateMin, endDateMax, status, ownerDepartmentId);
        var ids = query.Select(x => x.Project.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<ProjectWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return (await GetDbSetAsync()).Where(b => b.Id == id).Select(project => new ProjectWithNavigationProperties { Project = project, OwnerDepartment = dbContext.Set<Department>().FirstOrDefault(c => c.Id == project.OwnerDepartmentId) }).FirstOrDefault();
    }

    public virtual async Task<List<ProjectWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? endDateMin = null, DateTime? endDateMax = null, string? status = null, Guid? ownerDepartmentId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, code, name, description, startDateMin, startDateMax, endDateMin, endDateMax, status, ownerDepartmentId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProjectConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<ProjectWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        return from project in (await GetDbSetAsync())
               join ownerDepartment in (await GetDbContextAsync()).Set<Department>() on project.OwnerDepartmentId equals ownerDepartment.Id into departments
               from ownerDepartment in departments.DefaultIfEmpty()
               select new ProjectWithNavigationProperties
               {
                   Project = project,
                   OwnerDepartment = ownerDepartment
               };
    }

    protected virtual IQueryable<ProjectWithNavigationProperties> ApplyFilter(IQueryable<ProjectWithNavigationProperties> query, string? filterText, string? code = null, string? name = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? endDateMin = null, DateTime? endDateMax = null, string? status = null, Guid? ownerDepartmentId = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Project.Code!.Contains(filterText!) || e.Project.Name!.Contains(filterText!) || e.Project.Description!.Contains(filterText!) || e.Project.Status!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Project.Code.Contains(code)).WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Project.Name.Contains(name)).WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Project.Description.Contains(description)).WhereIf(startDateMin.HasValue, e => e.Project.StartDate >= startDateMin!.Value).WhereIf(startDateMax.HasValue, e => e.Project.StartDate <= startDateMax!.Value).WhereIf(endDateMin.HasValue, e => e.Project.EndDate >= endDateMin!.Value).WhereIf(endDateMax.HasValue, e => e.Project.EndDate <= endDateMax!.Value).WhereIf(!string.IsNullOrWhiteSpace(status), e => e.Project.Status.Contains(status)).WhereIf(ownerDepartmentId != null && ownerDepartmentId != Guid.Empty, e => e.OwnerDepartment != null && e.OwnerDepartment.Id == ownerDepartmentId);
    }

    public virtual async Task<List<Project>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? endDateMin = null, DateTime? endDateMax = null, string? status = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, code, name, description, startDateMin, startDateMax, endDateMin, endDateMax, status);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProjectConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? endDateMin = null, DateTime? endDateMax = null, string? status = null, Guid? ownerDepartmentId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, code, name, description, startDateMin, startDateMax, endDateMin, endDateMax, status, ownerDepartmentId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Project> ApplyFilter(IQueryable<Project> query, string? filterText = null, string? code = null, string? name = null, string? description = null, DateTime? startDateMin = null, DateTime? startDateMax = null, DateTime? endDateMin = null, DateTime? endDateMax = null, string? status = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code!.Contains(filterText!) || e.Name!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.Status!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code)).WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name)).WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description)).WhereIf(startDateMin.HasValue, e => e.StartDate >= startDateMin!.Value).WhereIf(startDateMax.HasValue, e => e.StartDate <= startDateMax!.Value).WhereIf(endDateMin.HasValue, e => e.EndDate >= endDateMin!.Value).WhereIf(endDateMax.HasValue, e => e.EndDate <= endDateMax!.Value).WhereIf(!string.IsNullOrWhiteSpace(status), e => e.Status.Contains(status));
    }
}