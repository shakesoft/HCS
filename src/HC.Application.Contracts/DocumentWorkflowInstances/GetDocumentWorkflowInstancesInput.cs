using Volo.Abp.Application.Dtos;
using System;

namespace HC.DocumentWorkflowInstances;

public abstract class GetDocumentWorkflowInstancesInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Status { get; set; }

    public DateTime? StartedAtMin { get; set; }

    public DateTime? StartedAtMax { get; set; }

    public DateTime? FinishedAtMin { get; set; }

    public DateTime? FinishedAtMax { get; set; }

    public Guid? DocumentId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? WorkflowTemplateId { get; set; }

    public Guid? CurrentStepId { get; set; }

    public GetDocumentWorkflowInstancesInputBase()
    {
    }
}