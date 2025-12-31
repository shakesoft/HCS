using HC.Projects;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.ProjectMembers;

namespace HC.ProjectMembers;

public abstract class ProjectMemberWithNavigationPropertiesBase
{
    public ProjectMember ProjectMember { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
}