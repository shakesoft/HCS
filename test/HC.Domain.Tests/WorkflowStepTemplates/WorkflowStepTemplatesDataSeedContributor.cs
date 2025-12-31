using HC.Workflows;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.WorkflowStepTemplates;

namespace HC.WorkflowStepTemplates;

public class WorkflowStepTemplatesDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IWorkflowStepTemplateRepository _workflowStepTemplateRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly WorkflowsDataSeedContributor _workflowsDataSeedContributor;

    public WorkflowStepTemplatesDataSeedContributor(IWorkflowStepTemplateRepository workflowStepTemplateRepository, IUnitOfWorkManager unitOfWorkManager, WorkflowsDataSeedContributor workflowsDataSeedContributor)
    {
        _workflowStepTemplateRepository = workflowStepTemplateRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _workflowsDataSeedContributor = workflowsDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _workflowsDataSeedContributor.SeedAsync(context);
        await _workflowStepTemplateRepository.InsertAsync(new WorkflowStepTemplate(id: Guid.Parse("18f531f8-a877-4be1-97a8-dad9b1e53f0a"), order: 7337, name: "1eb2615fe", type: "9b7d6a19632e483daff1", sLADays: 1436345348, allowReturn: true, isActive: true, workflowId: ));
        await _workflowStepTemplateRepository.InsertAsync(new WorkflowStepTemplate(id: Guid.Parse("e003dd39-1e46-4cd8-9b4d-425be811abe0"), order: 7952, name: "ea3139a2fb024e5997c70b99e1f4126898f34c874", type: "97b04b0b8f544d53a693", sLADays: 177843260, allowReturn: true, isActive: true, workflowId: ));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}