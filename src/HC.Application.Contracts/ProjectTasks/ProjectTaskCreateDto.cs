using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.ProjectTasks;

public abstract class ProjectTaskCreateDtoBase
{
    public string? ParentTaskId { get; set; }

    [Required]
    [StringLength(ProjectTaskConsts.CodeMaxLength)]
    public string Code { get; set; } = null!;
    [Required]
    [StringLength(ProjectTaskConsts.TitleMaxLength)]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    [Required]
    [StringLength(ProjectTaskConsts.PriorityMaxLength)]
    public string Priority { get; set; } = "LOW";
    [Required]
    [StringLength(ProjectTaskConsts.StatusMaxLength)]
    public string Status { get; set; } = "TODO";
    [Range(ProjectTaskConsts.ProgressPercentMinLength, ProjectTaskConsts.ProgressPercentMaxLength)]
    public int ProgressPercent { get; set; } = 0;
    public Guid ProjectId { get; set; }
}