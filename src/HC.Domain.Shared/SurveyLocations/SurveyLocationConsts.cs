namespace HC.SurveyLocations;

public static class SurveyLocationConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "SurveyLocation." : string.Empty);
    }
}