namespace HC.Projects;

public static class ProjectConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Project." : string.Empty);
    }

    public const int CodeMaxLength = 50;
    public const int NameMaxLength = 255;
}