using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectTasks;

public abstract class ProjectTaskUpdateDtoBase : IHasConcurrencyStamp
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
    public string Priority { get; set; }

    [Required]
    [StringLength(ProjectTaskConsts.StatusMaxLength)]
    public string Status { get; set; }

    [Range(ProjectTaskConsts.ProgressPercentMinLength, ProjectTaskConsts.ProgressPercentMaxLength)]
    public int ProgressPercent { get; set; }

    public Guid ProjectId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}