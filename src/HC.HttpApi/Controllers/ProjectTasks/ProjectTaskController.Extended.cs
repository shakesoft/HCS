using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.ProjectTasks;

namespace HC.Controllers.ProjectTasks;

[RemoteService]
[Area("app")]
[ControllerName("ProjectTask")]
[Route("api/app/project-tasks")]
public class ProjectTaskController : ProjectTaskControllerBase, IProjectTasksAppService
{
    public ProjectTaskController(IProjectTasksAppService projectTasksAppService) : base(projectTasksAppService)
    {
    }
}