using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentManagerBase : DomainService
{
    protected IWorkflowStepAssignmentRepository _workflowStepAssignmentRepository;

    public WorkflowStepAssignmentManagerBase(IWorkflowStepAssignmentRepository workflowStepAssignmentRepository)
    {
        _workflowStepAssignmentRepository = workflowStepAssignmentRepository;
    }

    public virtual async Task<WorkflowStepAssignment> CreateAsync(Guid? workflowId, Guid? stepId, Guid? templateId, Guid? defaultUserId, bool isPrimary, bool isActive)
    {
        var workflowStepAssignment = new WorkflowStepAssignment(GuidGenerator.Create(), workflowId, stepId, templateId, defaultUserId, isPrimary, isActive);
        return await _workflowStepAssignmentRepository.InsertAsync(workflowStepAssignment);
    }

    public virtual async Task<WorkflowStepAssignment> UpdateAsync(Guid id, Guid? workflowId, Guid? stepId, Guid? templateId, Guid? defaultUserId, bool isPrimary, bool isActive, [CanBeNull] string? concurrencyStamp = null)
    {
        var workflowStepAssignment = await _workflowStepAssignmentRepository.GetAsync(id);
        workflowStepAssignment.WorkflowId = workflowId;
        workflowStepAssignment.StepId = stepId;
        workflowStepAssignment.TemplateId = templateId;
        workflowStepAssignment.DefaultUserId = defaultUserId;
        workflowStepAssignment.IsPrimary = isPrimary;
        workflowStepAssignment.IsActive = isActive;
        workflowStepAssignment.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _workflowStepAssignmentRepository.UpdateAsync(workflowStepAssignment);
    }
}