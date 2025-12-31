using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.ProjectTaskDocuments;

public partial interface IProjectTaskDocumentRepository : IRepository<ProjectTaskDocument, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? documentPurpose = null, Guid? projectTaskId = null, Guid? documentId = null, CancellationToken cancellationToken = default);
    Task<ProjectTaskDocumentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProjectTaskDocumentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? documentPurpose = null, Guid? projectTaskId = null, Guid? documentId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<ProjectTaskDocument>> GetListAsync(string? filterText = null, string? documentPurpose = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? documentPurpose = null, Guid? projectTaskId = null, Guid? documentId = null, CancellationToken cancellationToken = default);
}