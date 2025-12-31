using System;

namespace HC.DocumentAssignments;

public abstract class DocumentAssignmentExcelDtoBase
{
    public int StepOrder { get; set; }

    public string ActionType { get; set; }

    public string Status { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime ProcessedAt { get; set; }

    public bool IsCurrent { get; set; }
}