using Volo.Abp.Application.Dtos;
using System;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public bool? IsPrimary { get; set; }

    public bool? IsActive { get; set; }

    public Guid? StepId { get; set; }

    public Guid? DefaultUserId { get; set; }

    public WorkflowStepAssignmentExcelDownloadDtoBase()
    {
    }
}