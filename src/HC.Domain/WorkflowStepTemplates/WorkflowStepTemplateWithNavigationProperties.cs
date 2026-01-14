using HC.WorkflowTemplates;
using System;
using System.Collections.Generic;
using HC.WorkflowStepTemplates;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateWithNavigationPropertiesBase
{
    public WorkflowStepTemplate WorkflowStepTemplate { get; set; } = null!;
    public WorkflowTemplate WorkflowTemplate { get; set; } = null!;
}