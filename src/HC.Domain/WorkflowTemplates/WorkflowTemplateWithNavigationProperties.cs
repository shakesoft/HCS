using HC.Workflows;
using System;
using System.Collections.Generic;
using HC.WorkflowTemplates;

namespace HC.WorkflowTemplates;

public abstract class WorkflowTemplateWithNavigationPropertiesBase
{
    public WorkflowTemplate WorkflowTemplate { get; set; } = null!;
    public Workflow Workflow { get; set; } = null!;
}