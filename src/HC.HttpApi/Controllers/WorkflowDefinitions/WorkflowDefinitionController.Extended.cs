using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.WorkflowDefinitions;

namespace HC.Controllers.WorkflowDefinitions;

[RemoteService]
[Area("app")]
[ControllerName("WorkflowDefinition")]
[Route("api/app/workflow-definitions")]
public class WorkflowDefinitionController : WorkflowDefinitionControllerBase, IWorkflowDefinitionsAppService
{
    public WorkflowDefinitionController(IWorkflowDefinitionsAppService workflowDefinitionsAppService) : base(workflowDefinitionsAppService)
    {
    }
}