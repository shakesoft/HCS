using HC.WorkflowStepTemplates;
using HC.Documents;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.DocumentAssignments;

namespace HC.DocumentAssignments;

public class DocumentAssignmentsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IDocumentAssignmentRepository _documentAssignmentRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly DocumentsDataSeedContributor _documentsDataSeedContributor;
    private readonly WorkflowStepTemplatesDataSeedContributor _workflowStepTemplatesDataSeedContributor;

    public DocumentAssignmentsDataSeedContributor(IDocumentAssignmentRepository documentAssignmentRepository, IUnitOfWorkManager unitOfWorkManager, DocumentsDataSeedContributor documentsDataSeedContributor, WorkflowStepTemplatesDataSeedContributor workflowStepTemplatesDataSeedContributor)
    {
        _documentAssignmentRepository = documentAssignmentRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _documentsDataSeedContributor = documentsDataSeedContributor;
        _workflowStepTemplatesDataSeedContributor = workflowStepTemplatesDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _documentsDataSeedContributor.SeedAsync(context);
        await _workflowStepTemplatesDataSeedContributor.SeedAsync(context);
        await _documentAssignmentRepository.InsertAsync(new DocumentAssignment(id: Guid.Parse("249be687-15b9-4841-8341-a80f6c606a49"), stepOrder: 10, actionType: "6c1c37b4ab724ad99504", status: "d7964f83dd534915bf57", assignedAt: new DateTime(2012, 8, 5), processedAt: new DateTime(2000, 3, 26), isCurrent: true, documentId: , stepId: , receiverUserId: ));
        await _documentAssignmentRepository.InsertAsync(new DocumentAssignment(id: Guid.Parse("6eb26e9e-1d79-41f6-8b37-59ccf2803923"), stepOrder: 8, actionType: "eba81a1b7fb34a7cb367", status: "e0a014a7efe54fdd810b", assignedAt: new DateTime(2002, 9, 10), processedAt: new DateTime(2014, 8, 17), isCurrent: true, documentId: , stepId: , receiverUserId: ));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}