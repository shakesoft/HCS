namespace HC.CalendarEventParticipants;

public static class CalendarEventParticipantConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "CalendarEventParticipant." : string.Empty);
    }
}