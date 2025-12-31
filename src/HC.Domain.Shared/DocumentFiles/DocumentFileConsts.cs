namespace HC.DocumentFiles;

public static class DocumentFileConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "DocumentFile." : string.Empty);
    }
}