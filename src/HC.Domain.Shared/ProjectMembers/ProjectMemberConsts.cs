namespace HC.ProjectMembers;

public static class ProjectMemberConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "ProjectMember." : string.Empty);
    }

    public const int MemberRoleMaxLength = 30;
}