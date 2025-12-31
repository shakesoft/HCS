using HC.Workflows;
using HC.Units;
using HC.MasterDatas;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.Documents;

namespace HC.Documents;

public class DocumentsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly MasterDatasDataSeedContributor _masterDatasDataSeedContributor;
    private readonly UnitsDataSeedContributor _unitsDataSeedContributor;
    private readonly WorkflowsDataSeedContributor _workflowsDataSeedContributor;

    public DocumentsDataSeedContributor(IDocumentRepository documentRepository, IUnitOfWorkManager unitOfWorkManager, MasterDatasDataSeedContributor masterDatasDataSeedContributor, UnitsDataSeedContributor unitsDataSeedContributor, WorkflowsDataSeedContributor workflowsDataSeedContributor)
    {
        _documentRepository = documentRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _masterDatasDataSeedContributor = masterDatasDataSeedContributor;
        _unitsDataSeedContributor = unitsDataSeedContributor;
        _workflowsDataSeedContributor = workflowsDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _masterDatasDataSeedContributor.SeedAsync(context);
        await _unitsDataSeedContributor.SeedAsync(context);
        await _workflowsDataSeedContributor.SeedAsync(context);
        await _documentRepository.InsertAsync(new Document(id: Guid.Parse("af8c4d9b-85e8-4de0-9dd3-c40768b59e9c"), no: "5a5922bb89b24e77bd043d1b57e7afbf63a778306a27497cbc", title: "c0a3bcf33ce14b68ba16ad6412a6595d9e6704ff4e9d46299bf675b515359ddfd95bb", type: "d617f936da2144ad9bc12371357df84a52368073ca0941389d", urgencyLevel: "aeb4b5104fba49d6aff9", secrecyLevel: "919cda906cd04731a64a", currentStatus: "5541555bf3da45e983f43f4bbe7442", completedTime: new DateTime(2013, 11, 16), fieldId: null, unitId: null, workflowId: null, statusId: null));
        await _documentRepository.InsertAsync(new Document(id: Guid.Parse("510c0e51-2439-4c74-b85f-567ed4319cae"), no: "8ff5bb65fdb7403b9d61e93749340fac6580cec65d1e4fc093", title: "0844145422ed4e5c8ae278d46e73217f085eac81e68e43429dbc8f5c97778d", type: "da7ef1a704bf4ca38c44d96de3ceaa3d0e3a17a4ffd5450dbe", urgencyLevel: "8aad68edcea149919b0c", secrecyLevel: "de9c3b5c2c2f47058f39", currentStatus: "730c7a91a9da440aa25f02b8017191", completedTime: new DateTime(2014, 8, 18), fieldId: null, unitId: null, workflowId: null, statusId: null));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}