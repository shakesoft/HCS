using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.SurveyFiles;

public abstract class SurveyFileManagerBase : DomainService
{
    protected ISurveyFileRepository _surveyFileRepository;

    public SurveyFileManagerBase(ISurveyFileRepository surveyFileRepository)
    {
        _surveyFileRepository = surveyFileRepository;
    }

    public virtual async Task<SurveyFile> CreateAsync(Guid surveySessionId, string uploaderType, string fileName, string filePath, int fileSize, string mimeType, string fileType)
    {
        Check.NotNull(surveySessionId, nameof(surveySessionId));
        Check.NotNullOrWhiteSpace(uploaderType, nameof(uploaderType));
        Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
        Check.NotNullOrWhiteSpace(filePath, nameof(filePath));
        Check.NotNullOrWhiteSpace(mimeType, nameof(mimeType));
        Check.NotNullOrWhiteSpace(fileType, nameof(fileType));
        var surveyFile = new SurveyFile(GuidGenerator.Create(), surveySessionId, uploaderType, fileName, filePath, fileSize, mimeType, fileType);
        return await _surveyFileRepository.InsertAsync(surveyFile);
    }

    public virtual async Task<SurveyFile> UpdateAsync(Guid id, Guid surveySessionId, string uploaderType, string fileName, string filePath, int fileSize, string mimeType, string fileType, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(surveySessionId, nameof(surveySessionId));
        Check.NotNullOrWhiteSpace(uploaderType, nameof(uploaderType));
        Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
        Check.NotNullOrWhiteSpace(filePath, nameof(filePath));
        Check.NotNullOrWhiteSpace(mimeType, nameof(mimeType));
        Check.NotNullOrWhiteSpace(fileType, nameof(fileType));
        var surveyFile = await _surveyFileRepository.GetAsync(id);
        surveyFile.SurveySessionId = surveySessionId;
        surveyFile.UploaderType = uploaderType;
        surveyFile.FileName = fileName;
        surveyFile.FilePath = filePath;
        surveyFile.FileSize = fileSize;
        surveyFile.MimeType = mimeType;
        surveyFile.FileType = fileType;
        surveyFile.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _surveyFileRepository.UpdateAsync(surveyFile);
    }
}