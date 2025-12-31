using HC.WorkflowTemplates;
using HC.WorkflowStepTemplates;
using HC.Workflows;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.WorkflowStepAssignments;

namespace HC.WorkflowStepAssignments;

public class WorkflowStepAssignmentsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IWorkflowStepAssignmentRepository _workflowStepAssignmentRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly WorkflowsDataSeedContributor _workflowsDataSeedContributor;
    private readonly WorkflowStepTemplatesDataSeedContributor _workflowStepTemplatesDataSeedContributor;
    private readonly WorkflowTemplatesDataSeedContributor _workflowTemplatesDataSeedContributor;

    public WorkflowStepAssignmentsDataSeedContributor(IWorkflowStepAssignmentRepository workflowStepAssignmentRepository, IUnitOfWorkManager unitOfWorkManager, WorkflowsDataSeedContributor workflowsDataSeedContributor, WorkflowStepTemplatesDataSeedContributor workflowStepTemplatesDataSeedContributor, WorkflowTemplatesDataSeedContributor workflowTemplatesDataSeedContributor)
    {
        _workflowStepAssignmentRepository = workflowStepAssignmentRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _workflowsDataSeedContributor = workflowsDataSeedContributor;
        _workflowStepTemplatesDataSeedContributor = workflowStepTemplatesDataSeedContributor;
        _workflowTemplatesDataSeedContributor = workflowTemplatesDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _workflowsDataSeedContributor.SeedAsync(context);
        await _workflowStepTemplatesDataSeedContributor.SeedAsync(context);
        await _workflowTemplatesDataSeedContributor.SeedAsync(context);
        await _workflowStepAssignmentRepository.InsertAsync(new WorkflowStepAssignment(id: Guid.Parse("9b736dd1-b2d3-4e41-91cc-6d1f33f83ac1"), isPrimary: true, isActive: true, workflowId: null, stepId: null, templateId: null, defaultUserId: null));
        await _workflowStepAssignmentRepository.InsertAsync(new WorkflowStepAssignment(id: Guid.Parse("178db716-2dcb-4306-a6b3-768f37eea54f"), isPrimary: true, isActive: true, workflowId: null, stepId: null, templateId: null, defaultUserId: null));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}