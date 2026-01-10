using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.ProjectMembers;

public abstract class ProjectMemberCreateDtoBase
{
    [Required]
    public ProjectMemberRole MemberRole { get; set; } = ProjectMemberRole.MEMBER;
    public DateTime JoinedAt { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }
}