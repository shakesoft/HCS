using System;

namespace HC.DocumentFiles;

public abstract class DocumentFileExcelDtoBase
{
    public string Name { get; set; } = null!;
    public string? Path { get; set; }

    public string? Hash { get; set; }

    public bool IsSigned { get; set; }

    public DateTime UploadedAt { get; set; }
}