using HC.Documents;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.DocumentHistories;

namespace HC.DocumentHistories;

public class DocumentHistoriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IDocumentHistoryRepository _documentHistoryRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly DocumentsDataSeedContributor _documentsDataSeedContributor;

    public DocumentHistoriesDataSeedContributor(IDocumentHistoryRepository documentHistoryRepository, IUnitOfWorkManager unitOfWorkManager, DocumentsDataSeedContributor documentsDataSeedContributor)
    {
        _documentHistoryRepository = documentHistoryRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _documentsDataSeedContributor = documentsDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _documentsDataSeedContributor.SeedAsync(context);
        await _documentHistoryRepository.InsertAsync(new DocumentHistory(id: Guid.Parse("1a52ebe0-8da8-446e-9fa5-19c062b5fe6c"), comment: "c02b244edbb74c2680eb10bf451648cf3bd22e3f50574f36b50d13d7455f14ce6c8f4c93757b4", action: "9ee45603c1e14f30a1adf649fb9706", documentId: , fromUser: null, toUser: ));
        await _documentHistoryRepository.InsertAsync(new DocumentHistory(id: Guid.Parse("53b386e1-5508-4466-a7b9-abee48fb47b3"), comment: "45922b68640d44a7902e02", action: "072541cc4edd4baca3f26469a52991", documentId: , fromUser: null, toUser: ));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}