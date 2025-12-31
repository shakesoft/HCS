using Volo.Abp.Application.Dtos;
using System;

namespace HC.ProjectMembers;

public abstract class ProjectMemberExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? MemberRole { get; set; }

    public DateTime? JoinedAtMin { get; set; }

    public DateTime? JoinedAtMax { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? UserId { get; set; }

    public ProjectMemberExcelDownloadDtoBase()
    {
    }
}