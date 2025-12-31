namespace HC.Positions;

public static class PositionConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Position." : string.Empty);
    }

    public const int CodeMinLength = 1;
    public const int CodeMaxLength = 50;
    public const int SignOrderMinLength = 0;
    public const int SignOrderMaxLength = 100;
}