using System;

namespace HC.UserSignatures;

public abstract class UserSignatureDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}