using Volo.Abp.Application.Dtos;
using System;

namespace HC.WorkflowTemplates;

public abstract class GetWorkflowTemplatesInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? OutputFormat { get; set; }

    public Guid? WorkflowId { get; set; }

    public GetWorkflowTemplatesInputBase()
    {
    }
}