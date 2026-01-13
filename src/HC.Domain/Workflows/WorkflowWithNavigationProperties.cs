using HC.WorkflowDefinitions;
using System;
using System.Collections.Generic;
using HC.Workflows;

namespace HC.Workflows;

public abstract class WorkflowWithNavigationPropertiesBase
{
    public Workflow Workflow { get; set; } = null!;
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;
}