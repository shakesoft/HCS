using System;

namespace HC.WorkflowTemplates;

public abstract class WorkflowTemplateDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}