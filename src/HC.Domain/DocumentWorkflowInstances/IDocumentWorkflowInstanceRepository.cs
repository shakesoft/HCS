using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.DocumentWorkflowInstances;

public partial interface IDocumentWorkflowInstanceRepository : IRepository<DocumentWorkflowInstance, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? status = null, DateTime? startedAtMin = null, DateTime? startedAtMax = null, DateTime? finishedAtMin = null, DateTime? finishedAtMax = null, Guid? documentId = null, Guid? workflowId = null, Guid? workflowTemplateId = null, Guid? currentStepId = null, CancellationToken cancellationToken = default);
    Task<DocumentWorkflowInstanceWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DocumentWorkflowInstanceWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? status = null, DateTime? startedAtMin = null, DateTime? startedAtMax = null, DateTime? finishedAtMin = null, DateTime? finishedAtMax = null, Guid? documentId = null, Guid? workflowId = null, Guid? workflowTemplateId = null, Guid? currentStepId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<DocumentWorkflowInstance>> GetListAsync(string? filterText = null, string? status = null, DateTime? startedAtMin = null, DateTime? startedAtMax = null, DateTime? finishedAtMin = null, DateTime? finishedAtMax = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? status = null, DateTime? startedAtMin = null, DateTime? startedAtMax = null, DateTime? finishedAtMin = null, DateTime? finishedAtMax = null, Guid? documentId = null, Guid? workflowId = null, Guid? workflowTemplateId = null, Guid? currentStepId = null, CancellationToken cancellationToken = default);
}