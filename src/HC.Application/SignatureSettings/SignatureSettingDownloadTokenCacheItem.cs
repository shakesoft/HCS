using System;

namespace HC.SignatureSettings;

public abstract class SignatureSettingDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}