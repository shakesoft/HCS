using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Projects;

namespace HC.Controllers.Projects;

[RemoteService]
[Area("app")]
[ControllerName("Project")]
[Route("api/app/projects")]
public class ProjectController : ProjectControllerBase, IProjectsAppService
{
    public ProjectController(IProjectsAppService projectsAppService) : base(projectsAppService)
    {
    }
}