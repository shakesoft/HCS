using System;

namespace HC.Documents;

public abstract class DocumentExcelDtoBase
{
    public string? No { get; set; }

    public string Title { get; set; } = null!;
    public string? Type { get; set; }

    public string? UrgencyLevel { get; set; }

    public string? SecrecyLevel { get; set; }

    public string? CurrentStatus { get; set; }

    public DateTime CompletedTime { get; set; }
}