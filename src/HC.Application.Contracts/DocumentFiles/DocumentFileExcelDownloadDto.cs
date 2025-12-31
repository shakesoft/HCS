using Volo.Abp.Application.Dtos;
using System;

namespace HC.DocumentFiles;

public abstract class DocumentFileExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public string? Path { get; set; }

    public string? Hash { get; set; }

    public bool? IsSigned { get; set; }

    public DateTime? UploadedAtMin { get; set; }

    public DateTime? UploadedAtMax { get; set; }

    public Guid? DocumentId { get; set; }

    public DocumentFileExcelDownloadDtoBase()
    {
    }
}