using HC.SurveySessions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace HC.SurveyFiles;

public abstract class SurveyFileBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [NotNull]
    public virtual string UploaderType { get; set; }

    [NotNull]
    public virtual string FileName { get; set; }

    [NotNull]
    public virtual string FilePath { get; set; }

    public virtual int FileSize { get; set; }

    [NotNull]
    public virtual string MimeType { get; set; }

    [NotNull]
    public virtual string FileType { get; set; }

    public Guid SurveySessionId { get; set; }

    protected SurveyFileBase()
    {
    }

    public SurveyFileBase(Guid id, Guid surveySessionId, string uploaderType, string fileName, string filePath, int fileSize, string mimeType, string fileType)
    {
        Id = id;
        Check.NotNull(uploaderType, nameof(uploaderType));
        Check.NotNull(fileName, nameof(fileName));
        Check.NotNull(filePath, nameof(filePath));
        Check.NotNull(mimeType, nameof(mimeType));
        Check.NotNull(fileType, nameof(fileType));
        UploaderType = uploaderType;
        FileName = fileName;
        FilePath = filePath;
        FileSize = fileSize;
        MimeType = mimeType;
        FileType = fileType;
        SurveySessionId = surveySessionId;
    }
}