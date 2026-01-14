using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.UserDepartments;

public partial interface IUserDepartmentRepository : IRepository<UserDepartment, Guid>
{
    Task DeleteAllAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? departmentId = null, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<UserDepartmentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<UserDepartmentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? departmentId = null, Guid? userId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<UserDepartment>> GetListAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? departmentId = null, Guid? userId = null, CancellationToken cancellationToken = default);
}