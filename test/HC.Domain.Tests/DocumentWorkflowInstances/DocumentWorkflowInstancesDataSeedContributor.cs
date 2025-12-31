using HC.WorkflowStepTemplates;
using HC.WorkflowTemplates;
using HC.Workflows;
using HC.Documents;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.DocumentWorkflowInstances;

namespace HC.DocumentWorkflowInstances;

public class DocumentWorkflowInstancesDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IDocumentWorkflowInstanceRepository _documentWorkflowInstanceRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly DocumentsDataSeedContributor _documentsDataSeedContributor;
    private readonly WorkflowsDataSeedContributor _workflowsDataSeedContributor;
    private readonly WorkflowTemplatesDataSeedContributor _workflowTemplatesDataSeedContributor;
    private readonly WorkflowStepTemplatesDataSeedContributor _workflowStepTemplatesDataSeedContributor;

    public DocumentWorkflowInstancesDataSeedContributor(IDocumentWorkflowInstanceRepository documentWorkflowInstanceRepository, IUnitOfWorkManager unitOfWorkManager, DocumentsDataSeedContributor documentsDataSeedContributor, WorkflowsDataSeedContributor workflowsDataSeedContributor, WorkflowTemplatesDataSeedContributor workflowTemplatesDataSeedContributor, WorkflowStepTemplatesDataSeedContributor workflowStepTemplatesDataSeedContributor)
    {
        _documentWorkflowInstanceRepository = documentWorkflowInstanceRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _documentsDataSeedContributor = documentsDataSeedContributor;
        _workflowsDataSeedContributor = workflowsDataSeedContributor;
        _workflowTemplatesDataSeedContributor = workflowTemplatesDataSeedContributor;
        _workflowStepTemplatesDataSeedContributor = workflowStepTemplatesDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _documentsDataSeedContributor.SeedAsync(context);
        await _workflowsDataSeedContributor.SeedAsync(context);
        await _workflowTemplatesDataSeedContributor.SeedAsync(context);
        await _workflowStepTemplatesDataSeedContributor.SeedAsync(context);
        await _documentWorkflowInstanceRepository.InsertAsync(new DocumentWorkflowInstance(id: Guid.Parse("10bc60a8-1b25-4c98-be34-4237f3cbabe6"), status: "b7817d5648ea42ef9855", startedAt: new DateTime(2010, 8, 15), finishedAt: new DateTime(2024, 11, 13), documentId: , workflowId: , workflowTemplateId: , currentStepId: ));
        await _documentWorkflowInstanceRepository.InsertAsync(new DocumentWorkflowInstance(id: Guid.Parse("a175862c-8312-4f26-abe8-05ba7413bd3a"), status: "e24e7cbe69ba4c25a078", startedAt: new DateTime(2023, 1, 21), finishedAt: new DateTime(2018, 11, 2), documentId: , workflowId: , workflowTemplateId: , currentStepId: ));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}