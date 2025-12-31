using HC.ProjectTasks;
using HC.Documents;
using System;
using System.Collections.Generic;
using HC.ProjectTaskDocuments;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentWithNavigationPropertiesBase
{
    public ProjectTaskDocument ProjectTaskDocument { get; set; } = null!;
    public ProjectTask ProjectTask { get; set; } = null!;
    public Document Document { get; set; } = null!;
}