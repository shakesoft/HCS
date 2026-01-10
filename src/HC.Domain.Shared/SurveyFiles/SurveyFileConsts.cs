namespace HC.SurveyFiles;

public static class SurveyFileConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "SurveyFile." : string.Empty);
    }
}