using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.DocumentWorkflowInstances;

namespace HC.Controllers.DocumentWorkflowInstances;

[RemoteService]
[Area("app")]
[ControllerName("DocumentWorkflowInstance")]
[Route("api/app/document-workflow-instances")]
public class DocumentWorkflowInstanceController : DocumentWorkflowInstanceControllerBase, IDocumentWorkflowInstancesAppService
{
    public DocumentWorkflowInstanceController(IDocumentWorkflowInstancesAppService documentWorkflowInstancesAppService) : base(documentWorkflowInstancesAppService)
    {
    }
}