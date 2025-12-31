using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.Departments;

public partial interface IDepartmentRepository : IRepository<Department, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? parentId = null, int? levelMin = null, int? levelMax = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, Guid? leaderUserId = null, CancellationToken cancellationToken = default);
    Task<DepartmentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DepartmentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? code = null, string? name = null, string? parentId = null, int? levelMin = null, int? levelMax = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, Guid? leaderUserId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<Department>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? parentId = null, int? levelMin = null, int? levelMax = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? parentId = null, int? levelMin = null, int? levelMax = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, Guid? leaderUserId = null, CancellationToken cancellationToken = default);
}