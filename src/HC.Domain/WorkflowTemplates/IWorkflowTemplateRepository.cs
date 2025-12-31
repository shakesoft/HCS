using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.WorkflowTemplates;

public partial interface IWorkflowTemplateRepository : IRepository<WorkflowTemplate, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? outputFormat = null, Guid? workflowId = null, CancellationToken cancellationToken = default);
    Task<WorkflowTemplateWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkflowTemplateWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? code = null, string? name = null, string? outputFormat = null, Guid? workflowId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<WorkflowTemplate>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? outputFormat = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? outputFormat = null, Guid? workflowId = null, CancellationToken cancellationToken = default);
}