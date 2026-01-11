namespace HC.SurveyCriterias;

public static class SurveyCriteriaConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "SurveyCriteria." : string.Empty);
    }
}