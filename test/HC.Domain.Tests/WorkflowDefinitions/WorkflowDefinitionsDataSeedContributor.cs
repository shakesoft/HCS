using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.WorkflowDefinitions;

namespace HC.WorkflowDefinitions;

public class WorkflowDefinitionsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public WorkflowDefinitionsDataSeedContributor(IWorkflowDefinitionRepository workflowDefinitionRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _workflowDefinitionRepository.InsertAsync(new WorkflowDefinition(id: Guid.Parse("0e604ee9-61cf-473d-baf9-a02f14e32d58"), code: "771b812b72c8432ca2ac0464bd2e9689bf59053399de45c6af", name: "3b8c325064fa42a78956d5e9ec8a227cd2be1a9", description: "57b5c043564b42bfaa7b2f8", isActive: true));
        await _workflowDefinitionRepository.InsertAsync(new WorkflowDefinition(id: Guid.Parse("5707f147-c6d0-4094-b280-16c7db497a05"), code: "c8b258e8e445493d9a31e068b6603576cdf1345c97e54f478b", name: "ace27cf471ea45eeaf4f35deff8f0915a641bd27867e4cd091d6b41fec5920dfaaba708f6bc5484ea039f0", description: "1d8f35a768604be683d7b7a321a813b0782", isActive: true));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}