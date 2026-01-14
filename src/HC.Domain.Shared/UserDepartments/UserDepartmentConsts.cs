namespace HC.UserDepartments;

public static class UserDepartmentConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "UserDepartment." : string.Empty);
    }
}