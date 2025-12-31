using HC.Workflows;
using HC.WorkflowStepTemplates;
using HC.WorkflowTemplates;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.WorkflowStepAssignments;

namespace HC.WorkflowStepAssignments;

public abstract class WorkflowStepAssignmentWithNavigationPropertiesBase
{
    public WorkflowStepAssignment WorkflowStepAssignment { get; set; } = null!;
    public Workflow? Workflow { get; set; }

    public WorkflowStepTemplate? Step { get; set; }

    public WorkflowTemplate? Template { get; set; }

    public IdentityUser? DefaultUser { get; set; }
}