using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyFiles;

public abstract class SurveyFileDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string UploaderType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int FileSize { get; set; }

    public string MimeType { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public Guid SurveySessionId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}