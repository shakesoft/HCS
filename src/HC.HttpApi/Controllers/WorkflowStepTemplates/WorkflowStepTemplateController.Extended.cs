using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.WorkflowStepTemplates;

namespace HC.Controllers.WorkflowStepTemplates;

[RemoteService]
[Area("app")]
[ControllerName("WorkflowStepTemplate")]
[Route("api/app/workflow-step-templates")]
public class WorkflowStepTemplateController : WorkflowStepTemplateControllerBase, IWorkflowStepTemplatesAppService
{
    public WorkflowStepTemplateController(IWorkflowStepTemplatesAppService workflowStepTemplatesAppService) : base(workflowStepTemplatesAppService)
    {
    }
}