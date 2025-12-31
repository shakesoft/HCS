namespace HC.ProjectTaskAssignments;

public static class ProjectTaskAssignmentConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "ProjectTaskAssignment." : string.Empty);
    }

    public const int AssignmentRoleMaxLength = 20;
}