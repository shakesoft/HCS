using System.ComponentModel;

namespace HC.ProjectTaskAssignments;

/// <summary>
/// Project task assignment role enumeration.
/// </summary>
public enum ProjectTaskAssignmentRole
{
    /// <summary>
    /// Main assignee.
    /// </summary>
    [Description("Main")]
    MAIN = 0,

    /// <summary>
    /// Supporting assignee.
    /// </summary>
    [Description("Support")]
    SUPPORT = 1,

    /// <summary>
    /// Reviewer.
    /// </summary>
    [Description("Review")]
    REVIEW = 2
}

