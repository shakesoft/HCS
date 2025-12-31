using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.ProjectMembers;

namespace HC.Controllers.ProjectMembers;

[RemoteService]
[Area("app")]
[ControllerName("ProjectMember")]
[Route("api/app/project-members")]
public class ProjectMemberController : ProjectMemberControllerBase, IProjectMembersAppService
{
    public ProjectMemberController(IProjectMembersAppService projectMembersAppService) : base(projectMembersAppService)
    {
    }
}