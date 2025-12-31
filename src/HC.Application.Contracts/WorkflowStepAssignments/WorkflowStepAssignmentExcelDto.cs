using System;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentExcelDtoBase
{
    public bool IsPrimary { get; set; }

    public bool IsActive { get; set; }
}