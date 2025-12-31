using System;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}