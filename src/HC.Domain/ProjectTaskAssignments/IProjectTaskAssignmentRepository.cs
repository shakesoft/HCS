using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.ProjectTaskAssignments;

public partial interface IProjectTaskAssignmentRepository : IRepository<ProjectTaskAssignment, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? assignmentRole = null, DateTime? assignedAtMin = null, DateTime? assignedAtMax = null, string? note = null, Guid? projectTaskId = null, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ProjectTaskAssignmentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProjectTaskAssignmentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? assignmentRole = null, DateTime? assignedAtMin = null, DateTime? assignedAtMax = null, string? note = null, Guid? projectTaskId = null, Guid? userId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<ProjectTaskAssignment>> GetListAsync(string? filterText = null, string? assignmentRole = null, DateTime? assignedAtMin = null, DateTime? assignedAtMax = null, string? note = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? assignmentRole = null, DateTime? assignedAtMin = null, DateTime? assignedAtMax = null, string? note = null, Guid? projectTaskId = null, Guid? userId = null, CancellationToken cancellationToken = default);
}