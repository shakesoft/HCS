using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.ProjectMembers;

public abstract class ProjectMemberCreateDtoBase
{
    [Required]
    [StringLength(ProjectMemberConsts.MemberRoleMaxLength)]
    public string MemberRole { get; set; } = "MEMBER";
    public DateTime JoinedAt { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }
}