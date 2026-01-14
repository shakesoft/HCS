using Volo.Abp.Application.Dtos;
using System;

namespace HC.WorkflowStepAssignments;

public abstract class GetWorkflowStepAssignmentsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public bool? IsPrimary { get; set; }

    public bool? IsActive { get; set; }

    public Guid? StepId { get; set; }

    public Guid? DefaultUserId { get; set; }

    public GetWorkflowStepAssignmentsInputBase()
    {
    }
}