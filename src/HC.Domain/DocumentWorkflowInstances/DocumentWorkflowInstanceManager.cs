using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstanceManagerBase : DomainService
{
    protected IDocumentWorkflowInstanceRepository _documentWorkflowInstanceRepository;

    public DocumentWorkflowInstanceManagerBase(IDocumentWorkflowInstanceRepository documentWorkflowInstanceRepository)
    {
        _documentWorkflowInstanceRepository = documentWorkflowInstanceRepository;
    }

    public virtual async Task<DocumentWorkflowInstance> CreateAsync(Guid documentId, Guid workflowId, Guid workflowTemplateId, Guid currentStepId, string status, DateTime startedAt, DateTime finishedAt)
    {
        Check.NotNull(documentId, nameof(documentId));
        Check.NotNull(workflowId, nameof(workflowId));
        Check.NotNull(workflowTemplateId, nameof(workflowTemplateId));
        Check.NotNull(currentStepId, nameof(currentStepId));
        Check.NotNullOrWhiteSpace(status, nameof(status));
        Check.Length(status, nameof(status), DocumentWorkflowInstanceConsts.StatusMaxLength);
        Check.NotNull(startedAt, nameof(startedAt));
        Check.NotNull(finishedAt, nameof(finishedAt));
        var documentWorkflowInstance = new DocumentWorkflowInstance(GuidGenerator.Create(), documentId, workflowId, workflowTemplateId, currentStepId, status, startedAt, finishedAt);
        return await _documentWorkflowInstanceRepository.InsertAsync(documentWorkflowInstance);
    }

    public virtual async Task<DocumentWorkflowInstance> UpdateAsync(Guid id, Guid documentId, Guid workflowId, Guid workflowTemplateId, Guid currentStepId, string status, DateTime startedAt, DateTime finishedAt, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(documentId, nameof(documentId));
        Check.NotNull(workflowId, nameof(workflowId));
        Check.NotNull(workflowTemplateId, nameof(workflowTemplateId));
        Check.NotNull(currentStepId, nameof(currentStepId));
        Check.NotNullOrWhiteSpace(status, nameof(status));
        Check.Length(status, nameof(status), DocumentWorkflowInstanceConsts.StatusMaxLength);
        Check.NotNull(startedAt, nameof(startedAt));
        Check.NotNull(finishedAt, nameof(finishedAt));
        var documentWorkflowInstance = await _documentWorkflowInstanceRepository.GetAsync(id);
        documentWorkflowInstance.DocumentId = documentId;
        documentWorkflowInstance.WorkflowId = workflowId;
        documentWorkflowInstance.WorkflowTemplateId = workflowTemplateId;
        documentWorkflowInstance.CurrentStepId = currentStepId;
        documentWorkflowInstance.Status = status;
        documentWorkflowInstance.StartedAt = startedAt;
        documentWorkflowInstance.FinishedAt = finishedAt;
        documentWorkflowInstance.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _documentWorkflowInstanceRepository.UpdateAsync(documentWorkflowInstance);
    }
}