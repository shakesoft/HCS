namespace HC.CalendarEvents;

public static class CalendarEventConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "CalendarEvent." : string.Empty);
    }
}