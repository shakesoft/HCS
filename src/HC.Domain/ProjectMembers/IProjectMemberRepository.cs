using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.ProjectMembers;

public partial interface IProjectMemberRepository : IRepository<ProjectMember, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ProjectMemberWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProjectMemberWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<ProjectMember>> GetListAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? memberRole = null, DateTime? joinedAtMin = null, DateTime? joinedAtMax = null, Guid? projectId = null, Guid? userId = null, CancellationToken cancellationToken = default);
}