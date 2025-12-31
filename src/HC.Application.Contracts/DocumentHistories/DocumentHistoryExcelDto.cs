using System;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryExcelDtoBase
{
    public string? Comment { get; set; }

    public string Action { get; set; }
}