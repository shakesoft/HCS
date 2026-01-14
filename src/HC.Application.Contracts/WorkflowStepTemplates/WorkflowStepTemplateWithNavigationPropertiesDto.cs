using HC.Workflows;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;
using HC.WorkflowTemplates;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateWithNavigationPropertiesDtoBase
{
    public WorkflowStepTemplateDto WorkflowStepTemplate { get; set; } = null!;
    public WorkflowTemplateDto WorkflowTemplate { get; set; } = null!;
    public WorkflowDto Workflow { get; set; } = null!;
}