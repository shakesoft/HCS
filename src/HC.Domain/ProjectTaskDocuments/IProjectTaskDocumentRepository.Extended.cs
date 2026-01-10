using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HC.ProjectTaskDocuments;

public partial interface IProjectTaskDocumentRepository
{
    Task<Dictionary<Guid, int>> GetCountByProjectTaskIdsAsync(
        List<Guid> projectTaskIds,
        CancellationToken cancellationToken = default);
}