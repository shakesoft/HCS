namespace HC.ProjectTasks;

public static class ProjectTaskConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "ProjectTask." : string.Empty);
    }

    public const int CodeMaxLength = 50;
    public const int TitleMaxLength = 255;
    public const int PriorityMaxLength = 20;
    public const int StatusMaxLength = 30;
    public const int ProgressPercentMinLength = 0;
    public const int ProgressPercentMaxLength = 100;
}