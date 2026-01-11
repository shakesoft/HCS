namespace HC.Documents;

public static class DocumentConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Document." : string.Empty);
    }

    public const int NoMaxLength = 50;
    public const int CurrentStatusMaxLength = 30;
    public const int StorageNumberMaxLength = 50;
}