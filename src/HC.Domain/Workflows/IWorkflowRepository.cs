using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.Workflows;

public partial interface IWorkflowRepository : IRepository<Workflow, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, Guid? workflowDefinitionId = null, CancellationToken cancellationToken = default);
    Task<WorkflowWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkflowWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, Guid? workflowDefinitionId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<Workflow>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, Guid? workflowDefinitionId = null, CancellationToken cancellationToken = default);
}