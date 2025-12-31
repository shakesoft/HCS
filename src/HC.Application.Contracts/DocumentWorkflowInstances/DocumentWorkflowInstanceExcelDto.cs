using System;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstanceExcelDtoBase
{
    public string Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }
}