using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Documents;

public abstract class DocumentManagerBase : DomainService
{
    protected IDocumentRepository _documentRepository;

    public DocumentManagerBase(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public virtual async Task<Document> CreateAsync(Guid? fieldId, Guid? unitId, Guid? workflowId, Guid? statusId, string title, DateTime completedTime, string? no = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNull(completedTime, nameof(completedTime));
        Check.Length(no, nameof(no), DocumentConsts.NoMaxLength);
        Check.Length(type, nameof(type), DocumentConsts.TypeMaxLength);
        Check.Length(urgencyLevel, nameof(urgencyLevel), DocumentConsts.UrgencyLevelMaxLength);
        Check.Length(secrecyLevel, nameof(secrecyLevel), DocumentConsts.SecrecyLevelMaxLength);
        Check.Length(currentStatus, nameof(currentStatus), DocumentConsts.CurrentStatusMaxLength);
        var document = new Document(GuidGenerator.Create(), fieldId, unitId, workflowId, statusId, title, completedTime, no, type, urgencyLevel, secrecyLevel, currentStatus);
        return await _documentRepository.InsertAsync(document);
    }

    public virtual async Task<Document> UpdateAsync(Guid id, Guid? fieldId, Guid? unitId, Guid? workflowId, Guid? statusId, string title, DateTime completedTime, string? no = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNull(completedTime, nameof(completedTime));
        Check.Length(no, nameof(no), DocumentConsts.NoMaxLength);
        Check.Length(type, nameof(type), DocumentConsts.TypeMaxLength);
        Check.Length(urgencyLevel, nameof(urgencyLevel), DocumentConsts.UrgencyLevelMaxLength);
        Check.Length(secrecyLevel, nameof(secrecyLevel), DocumentConsts.SecrecyLevelMaxLength);
        Check.Length(currentStatus, nameof(currentStatus), DocumentConsts.CurrentStatusMaxLength);
        var document = await _documentRepository.GetAsync(id);
        document.FieldId = fieldId;
        document.UnitId = unitId;
        document.WorkflowId = workflowId;
        document.StatusId = statusId;
        document.Title = title;
        document.CompletedTime = completedTime;
        document.No = no;
        document.Type = type;
        document.UrgencyLevel = urgencyLevel;
        document.SecrecyLevel = secrecyLevel;
        document.CurrentStatus = currentStatus;
        document.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _documentRepository.UpdateAsync(document);
    }
}