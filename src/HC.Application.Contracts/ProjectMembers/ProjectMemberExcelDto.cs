using System;

namespace HC.ProjectMembers;

public abstract class ProjectMemberExcelDtoBase
{
    public string MemberRole { get; set; }

    public DateTime JoinedAt { get; set; }
}