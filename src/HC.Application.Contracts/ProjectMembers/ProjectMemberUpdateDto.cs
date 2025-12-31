using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectMembers;

public abstract class ProjectMemberUpdateDtoBase : IHasConcurrencyStamp
{
    [Required]
    [StringLength(ProjectMemberConsts.MemberRoleMaxLength)]
    public string MemberRole { get; set; }

    public DateTime JoinedAt { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}