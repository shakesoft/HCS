using System;

namespace HC.ProjectTasks;

public abstract class ProjectTaskExcelDtoBase
{
    public string? ParentTaskId { get; set; }

    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Priority { get; set; }

    public string Status { get; set; }

    public int ProgressPercent { get; set; }
}