using HC.Workflows;
using System;
using System.Collections.Generic;
using HC.WorkflowStepTemplates;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateWithNavigationPropertiesBase
{
    public WorkflowStepTemplate WorkflowStepTemplate { get; set; } = null!;
    public Workflow Workflow { get; set; } = null!;
}