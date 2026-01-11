using System.Collections.Generic;
using HC.ProjectTaskAssignments;

namespace HC.ProjectTasks;

public class ProjectTaskWithNavigationPropertiesDto : ProjectTaskWithNavigationPropertiesDtoBase
{
    // Navigation: assignments with IdentityUser info for UI (Kanban avatars).
    public List<ProjectTaskAssignmentWithNavigationPropertiesDto> ProjectTaskAssignments { get; set; } = new();

    // Aggregation: document count for UI (Kanban badge).
    public int ProjectTaskDocumentsCount { get; set; }

    // Parent task title for UI (child tasks show parent label).
    public string? ParentTaskTitle { get; set; }

    // Aggregation: number of child tasks for UI (parent tasks show child-count icon).
    public int ChildTaskCount { get; set; }
}