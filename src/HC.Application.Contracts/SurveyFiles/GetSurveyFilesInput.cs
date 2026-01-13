using Volo.Abp.Application.Dtos;
using System;

namespace HC.SurveyFiles;

public abstract class GetSurveyFilesInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public UploaderType? UploaderType { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public int? FileSizeMin { get; set; }

    public int? FileSizeMax { get; set; }

    public string? MimeType { get; set; }

    public string? FileType { get; set; }

    public Guid? SurveySessionId { get; set; }

    public GetSurveyFilesInputBase()
    {
    }
}