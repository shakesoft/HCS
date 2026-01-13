using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Workflows;

public abstract class WorkflowManagerBase : DomainService
{
    protected IWorkflowRepository _workflowRepository;

    public WorkflowManagerBase(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public virtual async Task<Workflow> CreateAsync(Guid workflowDefinitionId, string code, string name, bool isActive, string? description = null)
    {
        Check.NotNull(workflowDefinitionId, nameof(workflowDefinitionId));
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), WorkflowConsts.CodeMaxLength, WorkflowConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var workflow = new Workflow(GuidGenerator.Create(), workflowDefinitionId, code, name, isActive, description);
        return await _workflowRepository.InsertAsync(workflow);
    }

    public virtual async Task<Workflow> UpdateAsync(Guid id, Guid workflowDefinitionId, string code, string name, bool isActive, string? description = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(workflowDefinitionId, nameof(workflowDefinitionId));
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), WorkflowConsts.CodeMaxLength, WorkflowConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var workflow = await _workflowRepository.GetAsync(id);
        workflow.WorkflowDefinitionId = workflowDefinitionId;
        workflow.Code = code;
        workflow.Name = name;
        workflow.IsActive = isActive;
        workflow.Description = description;
        workflow.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _workflowRepository.UpdateAsync(workflow);
    }
}