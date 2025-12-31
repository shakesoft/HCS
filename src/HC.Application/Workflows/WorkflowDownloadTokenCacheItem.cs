using System;

namespace HC.Workflows;

public abstract class WorkflowDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}