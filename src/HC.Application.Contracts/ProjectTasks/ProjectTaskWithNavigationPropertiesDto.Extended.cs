using System.Collections.Generic;
using HC.ProjectTaskAssignments;

namespace HC.ProjectTasks;

public class ProjectTaskWithNavigationPropertiesDto : ProjectTaskWithNavigationPropertiesDtoBase
{
    // Navigation: assignments with IdentityUser info for UI (Kanban avatars).
    public List<ProjectTaskAssignmentWithNavigationPropertiesDto> ProjectTaskAssignments { get; set; } = new();

    // Aggregation: document count for UI (Kanban badge).
    public int ProjectTaskDocumentsCount { get; set; }
}