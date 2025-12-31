using HC.Documents;
using HC.Workflows;
using HC.WorkflowTemplates;
using HC.WorkflowStepTemplates;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.DocumentWorkflowInstances;

public abstract class DocumentWorkflowInstanceWithNavigationPropertiesDtoBase
{
    public DocumentWorkflowInstanceDto DocumentWorkflowInstance { get; set; } = null!;
    public DocumentDto Document { get; set; } = null!;
    public WorkflowDto Workflow { get; set; } = null!;
    public WorkflowTemplateDto WorkflowTemplate { get; set; } = null!;
    public WorkflowStepTemplateDto CurrentStep { get; set; } = null!;
}