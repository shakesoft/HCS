using System.ComponentModel;

namespace HC.Projects;

/// <summary>
/// Project status enumeration
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Project is in planning phase
    /// </summary>
    [Description("Planning")]
    PLANNING = 0,

    /// <summary>
    /// Project is currently in progress
    /// </summary>
    [Description("In Progress")]
    IN_PROGRESS = 1,

    /// <summary>
    /// Project has been completed
    /// </summary>
    [Description("Completed")]
    COMPLETED = 2,

    /// <summary>
    /// Project has been cancelled
    /// </summary>
    [Description("Cancelled")]
    CANCELLED = 3
}