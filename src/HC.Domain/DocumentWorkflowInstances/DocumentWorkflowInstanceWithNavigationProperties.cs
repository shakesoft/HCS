using HC.Documents;
using HC.Workflows;
using HC.WorkflowTemplates;
using HC.WorkflowStepTemplates;
using System;
using System.Collections.Generic;
using HC.DocumentWorkflowInstances;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstanceWithNavigationPropertiesBase
{
    public DocumentWorkflowInstance DocumentWorkflowInstance { get; set; } = null!;
    public Document Document { get; set; } = null!;
    public Workflow Workflow { get; set; } = null!;
    public WorkflowTemplate WorkflowTemplate { get; set; } = null!;
    public WorkflowStepTemplate CurrentStep { get; set; } = null!;
}