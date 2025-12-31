using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.WorkflowStepAssignments;

public partial interface IWorkflowStepAssignmentRepository : IRepository<WorkflowStepAssignment, Guid>
{
    Task DeleteAllAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null, CancellationToken cancellationToken = default);
    Task<WorkflowStepAssignmentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkflowStepAssignmentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<WorkflowStepAssignment>> GetListAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null, CancellationToken cancellationToken = default);
}