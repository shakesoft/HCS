namespace HC.DocumentWorkflowInstances;

public static class DocumentWorkflowInstanceConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "DocumentWorkflowInstance." : string.Empty);
    }

    public const int StatusMaxLength = 20;
}