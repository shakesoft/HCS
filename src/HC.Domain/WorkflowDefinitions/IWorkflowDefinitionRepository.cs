using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.WorkflowDefinitions;

public partial interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<List<WorkflowDefinition>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, CancellationToken cancellationToken = default);
}