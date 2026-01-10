using System.ComponentModel;

namespace HC.ProjectTasks;

/// <summary>
/// Project task priority enumeration.
/// </summary>
public enum ProjectTaskPriority
{
    /// <summary>
    /// Low priority.
    /// </summary>
    [Description("Low")]
    LOW = 0,

    /// <summary>
    /// Medium priority.
    /// </summary>
    [Description("Medium")]
    MEDIUM = 1,

    /// <summary>
    /// High priority.
    /// </summary>
    [Description("High")]
    HIGH = 2,

    /// <summary>
    /// Urgent priority.
    /// </summary>
    [Description("Urgent")]
    URGENT = 3
}
