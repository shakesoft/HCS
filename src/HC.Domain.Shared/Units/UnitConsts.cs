namespace HC.Units;

public static class UnitConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Unit." : string.Empty);
    }

    public const int CodeMinLength = 1;
    public const int CodeMaxLength = 50;
}