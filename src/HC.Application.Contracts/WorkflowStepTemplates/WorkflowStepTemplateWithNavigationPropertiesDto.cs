using HC.WorkflowTemplates;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.WorkflowStepTemplates;

public abstract class WorkflowStepTemplateWithNavigationPropertiesDtoBase
{
    public WorkflowStepTemplateDto WorkflowStepTemplate { get; set; } = null!;
    public WorkflowTemplateDto WorkflowTemplate { get; set; } = null!;
}