namespace HC.WorkflowTemplates;

public static class WorkflowTemplateConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "WorkflowTemplate." : string.Empty);
    }

    public const int CodeMinLength = 1;
    public const int CodeMaxLength = 50;
    public const int OutputFormatMaxLength = 20;
    public const int SignModeMaxLength = 20;
}