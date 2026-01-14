using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.SurveyFiles;

public abstract class SurveyFileUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    public UploaderType UploaderType { get; set; } = UploaderType.PATIENT;
    [Required]
    public string FileName { get; set; } = null!;
    [Required]
    public string FilePath { get; set; } = null!;
    public int FileSize { get; set; }

    [Required]
    public string MimeType { get; set; } = null!;
    [Required]
    public string FileType { get; set; } = null!;
    public Guid SurveySessionId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}