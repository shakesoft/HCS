using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.WorkflowStepTemplates;

public partial interface IWorkflowStepTemplateRepository : IRepository<WorkflowStepTemplate, Guid>
{
    Task DeleteAllAsync(string? filterText = null, int? orderMin = null, int? orderMax = null, string? name = null, string? type = null, int? sLADaysMin = null, int? sLADaysMax = null, bool? isActive = null, Guid? workflowId = null, CancellationToken cancellationToken = default);
    Task<WorkflowStepTemplateWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkflowStepTemplateWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, int? orderMin = null, int? orderMax = null, string? name = null, string? type = null, int? sLADaysMin = null, int? sLADaysMax = null, bool? isActive = null, Guid? workflowId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<WorkflowStepTemplate>> GetListAsync(string? filterText = null, int? orderMin = null, int? orderMax = null, string? name = null, string? type = null, int? sLADaysMin = null, int? sLADaysMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, int? orderMin = null, int? orderMax = null, string? name = null, string? type = null, int? sLADaysMin = null, int? sLADaysMax = null, bool? isActive = null, Guid? workflowId = null, CancellationToken cancellationToken = default);
}