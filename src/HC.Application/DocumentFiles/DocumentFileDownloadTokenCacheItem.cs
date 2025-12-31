using System;

namespace HC.DocumentFiles;

public abstract class DocumentFileDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}