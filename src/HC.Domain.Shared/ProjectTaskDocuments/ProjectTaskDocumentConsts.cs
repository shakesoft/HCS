namespace HC.ProjectTaskDocuments;

public static class ProjectTaskDocumentConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "ProjectTaskDocument." : string.Empty);
    }

    public const int DocumentPurposeMaxLength = 50;
}