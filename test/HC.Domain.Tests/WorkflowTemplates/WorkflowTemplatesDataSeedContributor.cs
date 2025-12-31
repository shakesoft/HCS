using HC.Workflows;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.WorkflowTemplates;

namespace HC.WorkflowTemplates;

public class WorkflowTemplatesDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IWorkflowTemplateRepository _workflowTemplateRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly WorkflowsDataSeedContributor _workflowsDataSeedContributor;

    public WorkflowTemplatesDataSeedContributor(IWorkflowTemplateRepository workflowTemplateRepository, IUnitOfWorkManager unitOfWorkManager, WorkflowsDataSeedContributor workflowsDataSeedContributor)
    {
        _workflowTemplateRepository = workflowTemplateRepository;
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
        await _workflowTemplateRepository.InsertAsync(new WorkflowTemplate(id: Guid.Parse("42ef75ac-fa52-448b-baae-26c32dcc7c76"), code: "d7fae1884d9a4f91bdd34711bb71bfabec91adfeaf994f34be", name: "c7e676cc81c0460883f24982d6fb0ce7d71ce89501c64dff80e5a1c36ef07c880a84f8023f9a45c9a8fa762c772af", wordTemplatePath: "f0588cc7623640be925339e06c4488b9967bba6ece4b4a3e9e01b60", contentSchema: "f0d1647a6023446ab4efd45c029f4121214c12f87b1647488096", outputFormat: "57fcfb2b30f2444f8d45", signMode: "31cce0703d7549f89d0f", workflowId: ));
        await _workflowTemplateRepository.InsertAsync(new WorkflowTemplate(id: Guid.Parse("428b2b94-8f09-41f2-aa9c-e8f75bd8c34a"), code: "4bd6ec4036d34abcacab72ebc7f661b6854ffd4bad8e4e94bb", name: "239097600866466eb1f6fcb2175e97f4badd9b997663473689792609cda6fabf1adbe99632df4da584d6324fc5bdb3d", wordTemplatePath: "4aa187d604aa4065b7", contentSchema: "488b6bd8a8e342afb2545750e3d1d29f4", outputFormat: "f37fc70db56b487f936e", signMode: "fa7446755f274c95bba4", workflowId: ));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}