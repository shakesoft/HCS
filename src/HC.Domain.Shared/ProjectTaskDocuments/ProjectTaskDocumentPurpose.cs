using System.ComponentModel;

namespace HC.ProjectTaskDocuments;

/// <summary>
/// Project task document purpose enumeration.
/// </summary>
public enum ProjectTaskDocumentPurpose
{
    /// <summary>
    /// Report/submit document for the task.
    /// </summary>
    [Description("Report")]
    REPORT = 0,

    /// <summary>
    /// Reference document for the task.
    /// </summary>
    [Description("Reference")]
    REFERENCE = 1
}

