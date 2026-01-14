using HC.Projects;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.ProjectTasks;

public abstract class ProjectTaskWithNavigationPropertiesDtoBase
{
    public ProjectTaskDto ProjectTask { get; set; } = null!;
    public ProjectDto? Project { get; set; }
}