namespace HC.UserSignatures;

public static class UserSignatureConsts
{
    private const string DefaultSorting = "{0}CreationTime desc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "UserSignature." : string.Empty);
    }
}