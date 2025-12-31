using Volo.Abp.Application.Dtos;
using System;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? AssignmentRole { get; set; }

    public DateTime? AssignedAtMin { get; set; }

    public DateTime? AssignedAtMax { get; set; }

    public string? Note { get; set; }

    public Guid? ProjectTaskId { get; set; }

    public Guid? UserId { get; set; }

    public ProjectTaskAssignmentExcelDownloadDtoBase()
    {
    }
}