using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Workflows;

namespace HC.Controllers.Workflows;

[RemoteService]
[Area("app")]
[ControllerName("Workflow")]
[Route("api/app/workflows")]
public class WorkflowController : WorkflowControllerBase, IWorkflowsAppService
{
    public WorkflowController(IWorkflowsAppService workflowsAppService) : base(workflowsAppService)
    {
    }
}
