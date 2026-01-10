namespace HC.SurveySessions;

public static class SurveySessionConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "SurveySession." : string.Empty);
    }
}