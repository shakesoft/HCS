using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryManagerBase : DomainService
{
    protected IDocumentHistoryRepository _documentHistoryRepository;

    public DocumentHistoryManagerBase(IDocumentHistoryRepository documentHistoryRepository)
    {
        _documentHistoryRepository = documentHistoryRepository;
    }

    public virtual async Task<DocumentHistory> CreateAsync(Guid documentId, Guid? fromUser, Guid toUser, string action, string? comment = null)
    {
        Check.NotNull(documentId, nameof(documentId));
        Check.NotNull(toUser, nameof(toUser));
        Check.NotNullOrWhiteSpace(action, nameof(action));
        Check.Length(action, nameof(action), DocumentHistoryConsts.ActionMaxLength);
        var documentHistory = new DocumentHistory(GuidGenerator.Create(), documentId, fromUser, toUser, action, comment);
        return await _documentHistoryRepository.InsertAsync(documentHistory);
    }

    public virtual async Task<DocumentHistory> UpdateAsync(Guid id, Guid documentId, Guid? fromUser, Guid toUser, string action, string? comment = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(documentId, nameof(documentId));
        Check.NotNull(toUser, nameof(toUser));
        Check.NotNullOrWhiteSpace(action, nameof(action));
        Check.Length(action, nameof(action), DocumentHistoryConsts.ActionMaxLength);
        var documentHistory = await _documentHistoryRepository.GetAsync(id);
        documentHistory.DocumentId = documentId;
        documentHistory.FromUser = fromUser;
        documentHistory.ToUser = toUser;
        documentHistory.Action = action;
        documentHistory.Comment = comment;
        documentHistory.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _documentHistoryRepository.UpdateAsync(documentHistory);
    }
}