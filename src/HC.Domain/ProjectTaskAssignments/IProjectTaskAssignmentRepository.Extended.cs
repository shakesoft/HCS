using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HC.ProjectTaskAssignments;

public partial interface IProjectTaskAssignmentRepository
{
    Task<List<ProjectTaskAssignmentWithNavigationProperties>> GetListWithNavigationPropertiesByProjectTaskIdsAsync(
        List<Guid> projectTaskIds,
        CancellationToken cancellationToken = default);
}