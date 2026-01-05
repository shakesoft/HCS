namespace HC.NotificationReceivers;

public static class NotificationReceiverConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "NotificationReceiver." : string.Empty);
    }
}