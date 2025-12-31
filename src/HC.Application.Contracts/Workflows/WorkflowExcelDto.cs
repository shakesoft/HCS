using System;

namespace HC.Workflows;

public abstract class WorkflowExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}