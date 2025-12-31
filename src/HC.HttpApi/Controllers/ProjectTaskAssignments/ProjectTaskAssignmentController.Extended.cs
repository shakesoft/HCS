using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.ProjectTaskAssignments;

namespace HC.Controllers.ProjectTaskAssignments;

[RemoteService]
[Area("app")]
[ControllerName("ProjectTaskAssignment")]
[Route("api/app/project-task-assignments")]
public class ProjectTaskAssignmentController : ProjectTaskAssignmentControllerBase, IProjectTaskAssignmentsAppService
{
    public ProjectTaskAssignmentController(IProjectTaskAssignmentsAppService projectTaskAssignmentsAppService) : base(projectTaskAssignmentsAppService)
    {
    }
}