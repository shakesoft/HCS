namespace HC.SignatureSettings;

public static class SignatureSettingConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "SignatureSetting." : string.Empty);
    }
}