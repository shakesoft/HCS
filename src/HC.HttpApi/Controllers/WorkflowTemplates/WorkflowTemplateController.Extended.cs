using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.WorkflowTemplates;

namespace HC.Controllers.WorkflowTemplates;

[RemoteService]
[Area("app")]
[ControllerName("WorkflowTemplate")]
[Route("api/app/workflow-templates")]
public class WorkflowTemplateController : WorkflowTemplateControllerBase, IWorkflowTemplatesAppService
{
    public WorkflowTemplateController(IWorkflowTemplatesAppService workflowTemplatesAppService) : base(workflowTemplatesAppService)
    {
    }
}