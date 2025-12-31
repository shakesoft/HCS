namespace HC.WorkflowStepTemplates;

public static class WorkflowStepTemplateConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "WorkflowStepTemplate." : string.Empty);
    }

    public const int OrderMinLength = 1;
    public const int OrderMaxLength = 10000;
    public const int TypeMinLength = 1;
    public const int TypeMaxLength = 20;
}