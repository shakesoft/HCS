using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentManagerBase : DomainService
{
    protected IProjectTaskDocumentRepository _projectTaskDocumentRepository;

    public ProjectTaskDocumentManagerBase(IProjectTaskDocumentRepository projectTaskDocumentRepository)
    {
        _projectTaskDocumentRepository = projectTaskDocumentRepository;
    }

    public virtual async Task<ProjectTaskDocument> CreateAsync(Guid projectTaskId, Guid documentId, string documentPurpose)
    {
        Check.NotNull(projectTaskId, nameof(projectTaskId));
        Check.NotNull(documentId, nameof(documentId));
        Check.NotNullOrWhiteSpace(documentPurpose, nameof(documentPurpose));
        Check.Length(documentPurpose, nameof(documentPurpose), ProjectTaskDocumentConsts.DocumentPurposeMaxLength);
        var projectTaskDocument = new ProjectTaskDocument(GuidGenerator.Create(), projectTaskId, documentId, documentPurpose);
        return await _projectTaskDocumentRepository.InsertAsync(projectTaskDocument);
    }

    public virtual async Task<ProjectTaskDocument> UpdateAsync(Guid id, Guid projectTaskId, Guid documentId, string documentPurpose, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(projectTaskId, nameof(projectTaskId));
        Check.NotNull(documentId, nameof(documentId));
        Check.NotNullOrWhiteSpace(documentPurpose, nameof(documentPurpose));
        Check.Length(documentPurpose, nameof(documentPurpose), ProjectTaskDocumentConsts.DocumentPurposeMaxLength);
        var projectTaskDocument = await _projectTaskDocumentRepository.GetAsync(id);
        projectTaskDocument.ProjectTaskId = projectTaskId;
        projectTaskDocument.DocumentId = documentId;
        projectTaskDocument.DocumentPurpose = documentPurpose;
        projectTaskDocument.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _projectTaskDocumentRepository.UpdateAsync(projectTaskDocument);
    }
}