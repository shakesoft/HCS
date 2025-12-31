using HC.Workflows;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.WorkflowTemplates;

public abstract class WorkflowTemplateWithNavigationPropertiesDtoBase
{
    public WorkflowTemplateDto WorkflowTemplate { get; set; } = null!;
    public WorkflowDto Workflow { get; set; } = null!;
}