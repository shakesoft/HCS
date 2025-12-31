namespace HC.WorkflowStepAssignments;

public static class WorkflowStepAssignmentConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "WorkflowStepAssignment." : string.Empty);
    }
}