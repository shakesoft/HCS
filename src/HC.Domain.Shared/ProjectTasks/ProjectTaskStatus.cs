using System.ComponentModel;

namespace HC.ProjectTasks;

/// <summary>
/// Project task status enumeration.
/// </summary>
public enum ProjectTaskStatus
{
    /// <summary>
    /// Task is not started yet.
    /// </summary>
    [Description("To do")]
    TODO = 0,

    /// <summary>
    /// Task is currently in progress.
    /// </summary>
    [Description("In Progress")]
    IN_PROGRESS = 1,
    /// <summary>
    /// Task is waiting for something (e.g., review, dependency, external input).
    /// </summary>
    [Description("Waiting")]
    WAITING = 2,

    /// <summary>
    /// Task is finished.
    /// </summary>
    [Description("Done")]
    DONE = 3,

    /// <summary>
    /// Backward compatible alias for DONE.
    /// </summary>
    [Description("Done")]
    COMPLETED = DONE,

    /// <summary>
    /// Task has been cancelled.
    /// </summary>
    [Description("Cancelled")]
    CANCELLED = 4
}