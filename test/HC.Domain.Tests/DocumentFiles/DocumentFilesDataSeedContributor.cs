using HC.Documents;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.DocumentFiles;

namespace HC.DocumentFiles;

public class DocumentFilesDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IDocumentFileRepository _documentFileRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly DocumentsDataSeedContributor _documentsDataSeedContributor;

    public DocumentFilesDataSeedContributor(IDocumentFileRepository documentFileRepository, IUnitOfWorkManager unitOfWorkManager, DocumentsDataSeedContributor documentsDataSeedContributor)
    {
        _documentFileRepository = documentFileRepository;
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
        await _documentFileRepository.InsertAsync(new DocumentFile(id: Guid.Parse("cb5d2b5d-44e9-474b-b1f3-11965ee88c52"), name: "cade3701a08d49a7bf5a7a7a93b156c8f3658c8f21cc4f32b70c739571bef0b2df84ddbd581a4cae9", path: "902c63efc7f9450899d5ce676d70bf7249c02d4fcdd54", hash: "28adf49b11f94f39b1e5bb00e5110bd05c7851692fe8487392da5daae49f84d3a2e03b9", isSigned: true, uploadedAt: new DateTime(2008, 7, 2), documentId: ));
        await _documentFileRepository.InsertAsync(new DocumentFile(id: Guid.Parse("e089ae06-e0d1-49dc-9c9a-d9771cccb6e8"), name: "914443785f9e4a8ca01cf203bc80a1bc06b571ffa82a4c94ab5d7412fecf68d6a0a9ff7da7b94337", path: "7727c5c715a649c1bd3a41385d04ffe12f213d7f6ab54", hash: "2d129b4b48eb44f9a70af254679937ddd1b23e63a3ea4a889", isSigned: true, uploadedAt: new DateTime(2023, 11, 9), documentId: ));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}