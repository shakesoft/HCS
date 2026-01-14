using Volo.Abp.Identity;
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

namespace HC.ProjectMembers;

public abstract class EfCoreProjectMemberRepositoryBase : EfCoreRepository<HCDbContext, ProjectMember, Guid>
{
    public EfCoreProjectMemberRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, memberRole, joinedAtMin, joinedAtMax, projectId, userId);
        var ids = query.Select(x => x.ProjectMember.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<ProjectMemberWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var projectMember = await (await GetDbSetAsync()).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (projectMember == null)
        {
            return null;
        }
        
        // Allow Project and User to be null if they've been deleted (soft delete)
        var project = await dbContext.Set<Project>().FirstOrDefaultAsync(c => c.Id == projectMember.ProjectId && !c.IsDeleted);
        var user = await dbContext.Set<IdentityUser>().FirstOrDefaultAsync(c => c.Id == projectMember.UserId);
        
        return new ProjectMemberWithNavigationProperties
        {
            ProjectMember = projectMember,
            Project = project,
            User = user
        };
    }

    public virtual async Task<List<ProjectMemberWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, memberRole, joinedAtMin, joinedAtMax, projectId, userId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProjectMemberConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<ProjectMemberWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        return from projectMember in (await GetDbSetAsync())
               join project in (await GetDbContextAsync()).Set<Project>().Where(p => !p.IsDeleted) on projectMember.ProjectId equals project.Id into projects
               from project in projects.DefaultIfEmpty()
               join user in (await GetDbContextAsync()).Set<IdentityUser>() on projectMember.UserId equals user.Id into identityUsers
               from user in identityUsers.DefaultIfEmpty()
               select new ProjectMemberWithNavigationProperties
               {
                   ProjectMember = projectMember,
                   Project = project,
                   User = user
               };
    }

    protected virtual IQueryable<ProjectMemberWithNavigationProperties> ApplyFilter(IQueryable<ProjectMemberWithNavigationProperties> query, string? filterText, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null)
    {
        var filterTextLower = filterText?.Trim().ToLower();

        return query
            // Filter by username or name (case-insensitive contains)
            .WhereIf(!string.IsNullOrWhiteSpace(filterTextLower), e =>
                (e.User != null &&
                    (
                        (!string.IsNullOrWhiteSpace(e.User.UserName) && e.User.UserName.ToLower().Contains(filterTextLower!)) ||
                        (!string.IsNullOrWhiteSpace(e.User.Name) && e.User.Name.ToLower().Contains(filterTextLower!))
                    )
                )
            )
            // Keep existing explicit filters
            .WhereIf(!string.IsNullOrWhiteSpace(memberRole), e => e.ProjectMember.MemberRole.Contains(memberRole))
            .WhereIf(joinedAtMin.HasValue, e => e.ProjectMember.JoinedAt >= joinedAtMin!.Value)
            .WhereIf(joinedAtMax.HasValue, e => e.ProjectMember.JoinedAt <= joinedAtMax!.Value)
            .WhereIf(projectId != null && projectId != Guid.Empty, e => e.Project != null && e.Project.Id == projectId)
            .WhereIf(userId != null && userId != Guid.Empty, e => e.User != null && e.User.Id == userId);
    }

    public virtual async Task<List<ProjectMember>> GetListAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, memberRole, joinedAtMin, joinedAtMax);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProjectMemberConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, memberRole, joinedAtMin, joinedAtMax, projectId, userId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<ProjectMember> ApplyFilter(IQueryable<ProjectMember> query, string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.MemberRole!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(memberRole), e => e.MemberRole.Contains(memberRole)).WhereIf(joinedAtMin.HasValue, e => e.JoinedAt >= joinedAtMin!.Value).WhereIf(joinedAtMax.HasValue, e => e.JoinedAt <= joinedAtMax!.Value);
    }
}