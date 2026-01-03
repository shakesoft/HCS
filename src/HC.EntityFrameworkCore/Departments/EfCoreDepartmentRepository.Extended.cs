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

namespace HC.Departments;

public class EfCoreDepartmentRepository : EfCoreDepartmentRepositoryBase, IDepartmentRepository
{
    public EfCoreDepartmentRepository(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public override async Task<DepartmentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Call base method to get department with navigation properties
        var result = await base.GetWithNavigationPropertiesAsync(id, cancellationToken);
        
        if (result != null)
        {
            // Load children for this department
            var departments = await GetDbSetAsync();
            result.Children = await departments
                .Where(d => d.ParentId == result.Department.Id.ToString())
                .ToListAsync(cancellationToken);
        }
        
        return result;
    }

    public override async Task<List<DepartmentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? code = null, string? name = null, string? parentId = null, int? levelMin = null, int? levelMax = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, Guid? leaderUserId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        // Call base method to get list of departments with navigation properties
        var result = await base.GetListWithNavigationPropertiesAsync(filterText, code, name, parentId, levelMin, levelMax, sortOrderMin, sortOrderMax, isActive, leaderUserId, sorting, maxResultCount, skipCount, cancellationToken);
        
        // Load children for the departments
        await LoadChildrenAsync(result, cancellationToken);
        
        return result;
    }

    // Helper method to load children after materializing the query
    protected virtual async Task LoadChildrenAsync(List<DepartmentWithNavigationProperties> items, CancellationToken cancellationToken = default)
    {
        if (items == null || !items.Any())
            return;
            
        var departments = await GetDbSetAsync();
        var departmentIds = items.Select(x => x.Department.Id.ToString()).ToList();
        
        // Load all children for the departments in the list
        var allChildren = await departments
            .Where(d => d.ParentId != null && departmentIds.Contains(d.ParentId))
            .ToListAsync(cancellationToken);
        
        // Group children by ParentId
        var childrenByParentId = allChildren
            .GroupBy(d => d.ParentId)
            .ToDictionary(g => g.Key!, g => g.ToList());
        
        // Assign children to each department
        foreach (var item in items)
        {
            var parentId = item.Department.Id.ToString();
            if (childrenByParentId.TryGetValue(parentId, out var children))
            {
                item.Children = children;
            }
            else
            {
                item.Children = new List<Department>();
            }
        }
    }
}