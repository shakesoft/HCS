using System;

namespace HC.ProjectTaskAssignments;

public abstract class ProjectTaskAssignmentExcelDtoBase
{
    public string AssignmentRole { get; set; }

    public DateTime AssignedAt { get; set; }

    public string? Note { get; set; }
}