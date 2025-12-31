using HC.ProjectTasks;
using HC.Documents;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.ProjectTaskDocuments;

public abstract class ProjectTaskDocumentWithNavigationPropertiesDtoBase
{
    public ProjectTaskDocumentDto ProjectTaskDocument { get; set; } = null!;
    public ProjectTaskDto ProjectTask { get; set; } = null!;
    public DocumentDto Document { get; set; } = null!;
}