using HC.Workflows;
using HC.WorkflowStepTemplates;
using HC.WorkflowTemplates;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentWithNavigationPropertiesDtoBase
{
    public WorkflowStepAssignmentDto WorkflowStepAssignment { get; set; } = null!;
    public WorkflowDto? Workflow { get; set; }

    public WorkflowStepTemplateDto? Step { get; set; }

    public WorkflowTemplateDto? Template { get; set; }

    public IdentityUserDto? DefaultUser { get; set; }
}