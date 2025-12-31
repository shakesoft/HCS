namespace HC.WorkflowDefinitions;

public static class WorkflowDefinitionConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "WorkflowDefinition." : string.Empty);
    }

    public const int CodeMinLength = 1;
    public const int CodeMaxLength = 50;
}