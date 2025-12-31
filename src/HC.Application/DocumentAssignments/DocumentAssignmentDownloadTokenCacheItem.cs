using System;

namespace HC.DocumentAssignments;

public abstract class DocumentAssignmentDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}