using HC.Projects;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.ProjectMembers;

public abstract class ProjectMemberWithNavigationPropertiesDtoBase
{
    public ProjectMemberDto ProjectMember { get; set; } = null!;
    public ProjectDto Project { get; set; } = null!;
    public IdentityUserDto User { get; set; } = null!;
}