using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionManagerBase : DomainService
{
    protected IWorkflowDefinitionRepository _workflowDefinitionRepository;

    public WorkflowDefinitionManagerBase(IWorkflowDefinitionRepository workflowDefinitionRepository)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
    }

    public virtual async Task<WorkflowDefinition> CreateAsync(string code, string name, bool isActive, string? description = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), WorkflowDefinitionConsts.CodeMaxLength, WorkflowDefinitionConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var workflowDefinition = new WorkflowDefinition(GuidGenerator.Create(), code, name, isActive, description);
        return await _workflowDefinitionRepository.InsertAsync(workflowDefinition);
    }

    public virtual async Task<WorkflowDefinition> UpdateAsync(Guid id, string code, string name, bool isActive, string? description = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), WorkflowDefinitionConsts.CodeMaxLength, WorkflowDefinitionConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var workflowDefinition = await _workflowDefinitionRepository.GetAsync(id);
        workflowDefinition.Code = code;
        workflowDefinition.Name = name;
        workflowDefinition.IsActive = isActive;
        workflowDefinition.Description = description;
        workflowDefinition.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _workflowDefinitionRepository.UpdateAsync(workflowDefinition);
    }
}