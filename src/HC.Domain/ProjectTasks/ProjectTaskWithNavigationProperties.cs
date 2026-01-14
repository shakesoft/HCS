using HC.Projects;
using System;
using System.Collections.Generic;
using HC.ProjectTasks;

namespace HC.ProjectTasks;

public abstract class ProjectTaskWithNavigationPropertiesBase
{
    public ProjectTask ProjectTask { get; set; } = null!;
    public Project? Project { get; set; }
}