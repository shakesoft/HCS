namespace HC.DocumentHistories;

public static class DocumentHistoryConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "DocumentHistory." : string.Empty);
    }

    public const int ActionMaxLength = 30;
}