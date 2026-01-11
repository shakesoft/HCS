using System;

namespace HC.SurveyFiles;

public abstract class SurveyFileExcelDtoBase
{
    public string UploaderType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int FileSize { get; set; }

    public string MimeType { get; set; } = null!;
    public string FileType { get; set; } = null!;
}