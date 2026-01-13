using Volo.Abp.Application.Dtos;
using System;

namespace HC.Workflows;

public abstract class WorkflowExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public Guid? WorkflowDefinitionId { get; set; }

    public WorkflowExcelDownloadDtoBase()
    {
    }
}