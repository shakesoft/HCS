using Volo.Abp.Application.Dtos;
using System;

namespace HC.WorkflowTemplates;

public abstract class WorkflowTemplateExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? OutputFormat { get; set; }

    public Guid? WorkflowId { get; set; }

    public WorkflowTemplateExcelDownloadDtoBase()
    {
    }
}