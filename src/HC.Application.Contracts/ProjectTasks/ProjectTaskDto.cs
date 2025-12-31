using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectTasks;

public abstract class ProjectTaskDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
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

    public Guid ProjectId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}