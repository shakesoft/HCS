using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace HC.ProjectMembers;

public abstract class ProjectMemberDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public ProjectMemberRole MemberRole { get; set; }

    public DateTime JoinedAt { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}