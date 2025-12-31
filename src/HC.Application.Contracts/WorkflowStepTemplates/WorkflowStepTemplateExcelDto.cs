using System;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateExcelDtoBase
{
    public int Order { get; set; }

    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int? SLADays { get; set; }

    public bool AllowReturn { get; set; }

    public bool IsActive { get; set; }
}