using Volo.Abp.Application.Dtos;
using System;

namespace HC.WorkflowStepTemplates;

public abstract class GetWorkflowStepTemplatesInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public int? OrderMin { get; set; }

    public int? OrderMax { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public int? SLADaysMin { get; set; }

    public int? SLADaysMax { get; set; }

    public bool? IsActive { get; set; }

    public Guid? WorkflowId { get; set; }

    public GetWorkflowStepTemplatesInputBase()
    {
    }
}