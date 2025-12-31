using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateManagerBase : DomainService
{
    protected IWorkflowStepTemplateRepository _workflowStepTemplateRepository;

    public WorkflowStepTemplateManagerBase(IWorkflowStepTemplateRepository workflowStepTemplateRepository)
    {
        _workflowStepTemplateRepository = workflowStepTemplateRepository;
    }

    public virtual async Task<WorkflowStepTemplate> CreateAsync(Guid workflowId, int order, string name, string type, bool allowReturn, bool isActive, int? sLADays = null)
    {
        Check.NotNull(workflowId, nameof(workflowId));
        Check.Range(order, nameof(order), WorkflowStepTemplateConsts.OrderMinLength, WorkflowStepTemplateConsts.OrderMaxLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(type, nameof(type));
        Check.Length(type, nameof(type), WorkflowStepTemplateConsts.TypeMaxLength, WorkflowStepTemplateConsts.TypeMinLength);
        var workflowStepTemplate = new WorkflowStepTemplate(GuidGenerator.Create(), workflowId, order, name, type, allowReturn, isActive, sLADays);
        return await _workflowStepTemplateRepository.InsertAsync(workflowStepTemplate);
    }

    public virtual async Task<WorkflowStepTemplate> UpdateAsync(Guid id, Guid workflowId, int order, string name, string type, bool allowReturn, bool isActive, int? sLADays = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(workflowId, nameof(workflowId));
        Check.Range(order, nameof(order), WorkflowStepTemplateConsts.OrderMinLength, WorkflowStepTemplateConsts.OrderMaxLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(type, nameof(type));
        Check.Length(type, nameof(type), WorkflowStepTemplateConsts.TypeMaxLength, WorkflowStepTemplateConsts.TypeMinLength);
        var workflowStepTemplate = await _workflowStepTemplateRepository.GetAsync(id);
        workflowStepTemplate.WorkflowId = workflowId;
        workflowStepTemplate.Order = order;
        workflowStepTemplate.Name = name;
        workflowStepTemplate.Type = type;
        workflowStepTemplate.AllowReturn = allowReturn;
        workflowStepTemplate.IsActive = isActive;
        workflowStepTemplate.SLADays = sLADays;
        workflowStepTemplate.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _workflowStepTemplateRepository.UpdateAsync(workflowStepTemplate);
    }
}