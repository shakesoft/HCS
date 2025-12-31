using System;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionExcelDtoBase
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}