using System;

namespace HC.WorkflowDefinitions;

public abstract class WorkflowDefinitionDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}