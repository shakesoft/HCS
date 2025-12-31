using Volo.Abp.Application.Dtos;
using System;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public WorkflowDefinitionExcelDownloadDtoBase()
    {
    }
}