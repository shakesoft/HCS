using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.Workflows;

namespace HC.Workflows;

public class WorkflowsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public WorkflowsDataSeedContributor(IWorkflowRepository workflowRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _workflowRepository = workflowRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _workflowRepository.InsertAsync(new Workflow(id: Guid.Parse("66523a7a-3dcb-4880-a5c2-ae55ac3c4656"), code: "81aef58849b7485eb9950c9d99f50cd3fee2a3eb68604888a6", name: "53fb3a1b7fd54964acee37309878c5aacd9c71227a63467bb89f4ebeeef64af669b3fd48a59d4d198ed3", description: "e2cd60d381e54d528ac0cc72f944c60bc9ab58b386b4483ba303ed96d22be132bb233a405317", isActive: true));
        await _workflowRepository.InsertAsync(new Workflow(id: Guid.Parse("0e294610-d894-4dd6-bc3a-823341ce02fb"), code: "0192a12fc1534966af7a3b25994447a764747fa9e48e4317a2", name: "c99630793d6946c0960ee31a1f3b7ca", description: "c35eb022e2a744ccb870db2cc36c963b2a1cd8b", isActive: true));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}