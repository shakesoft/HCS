using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.ProjectTaskDocuments;

namespace HC.Controllers.ProjectTaskDocuments;

[RemoteService]
[Area("app")]
[ControllerName("ProjectTaskDocument")]
[Route("api/app/project-task-documents")]
public class ProjectTaskDocumentController : ProjectTaskDocumentControllerBase, IProjectTaskDocumentsAppService
{
    public ProjectTaskDocumentController(IProjectTaskDocumentsAppService projectTaskDocumentsAppService) : base(projectTaskDocumentsAppService)
    {
    }
}